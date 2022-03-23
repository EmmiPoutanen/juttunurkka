
/*
Copyright 2021 Emma Kemppainen, Jesse Huttunen, Tanja Kultala, Niklas Arjasmaa

This file is part of "Mieliala kysely".

Mieliala kysely is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

Mieliala kysely is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Mieliala kysely.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Prototype
{
	/// <summary>
	/// Provides functionality to communicate with clients in order to host a survey
	/// </summary>

	// DISCLAIMER!
	// Throughout this project I was *learning* networking and async programming nearly from the ground up
	// I cannot guarantee the safety of this code and I apologize for any bad approaches
	// No sensitive data is exchanged luckily!
	// Many of the functions here can be converted to generic ReceiveData and SendData functions to improve efficiency and error tolerance

	public class SurveyHost
	{
		/// <value>
		/// Instance of Survey containing the details of the hosted survey
		/// </value>
		private Survey survey;

		/// <value>
		/// Instance of SurveyData containing the data of the survey results
		/// </value>
		public SurveyData data { get; private set; }

		/// <value>
		/// Integer value depicting how many clients are currently connected
		/// </value>
		public int clientCount { get; private set; }

		/// <value>
		/// List of TcpClient instances for each connected client
		/// </value>
		private List<TcpClient> clients;

		/// <value>
		/// List of IP adresses which have joined the survey. Entries remain even if the client in question disconnects.
		/// </value>
		private List<IPAddress> clientHistory;

		/// <value>
		/// List of running Tasks which can be cancelled
		/// </value>
		private List<Task> cancellableTasks;

		/// <value>
		/// Instance of CancellationTokenSource which can be used to call cancellation of tasks in the cancellableTasks list
		/// </value>
		private CancellationTokenSource tokenSource;

		/// <value>
		/// Instance of CancellationToken fed to the cancellable tasks
		/// </value>
		private CancellationToken token;

		/// <value>
		/// Instance of ActivityVote to serve activity voting
		/// </value>
		public ActivityVote voteCalc { get; private set; } = null;

		/// <value>
		/// Boolean indicating whether the voting has concluded
		/// </value>
		public bool isVoteConcluded { get; private set; } = false;

		/// <summary>
		/// Default constructor
		/// <remarks>
		/// The instance created does not start running any tasks automatically
		/// </remarks>
		/// </summary>
		public SurveyHost() {
			data = new SurveyData();
			survey = SurveyManager.GetInstance().GetSurvey();
			clientCount = 0;
			clients = new List<TcpClient>();
			clientHistory = new List<IPAddress>();
			cancellableTasks = new List<Task>();
			tokenSource = new CancellationTokenSource();
			token = tokenSource.Token;
		}

		/// <summary>
		/// The main sequence of running a survey
		/// </summary>
		/// <remarks>
		/// CloseSurvey method must be called to advance this task from the initial phase of accepting new clients
		/// </remarks>
		/// <returns>
		/// Task object resulting in a boolean indicating whether a fatal error occured in the process terminating hosting as a whole
		/// </returns>
		public async Task<bool> RunSurvey()
		{

			//Phase 1 - making client connections and collecting emojis
			Task<bool> task1 = ReplyBroadcast();
			Task<bool> task2 = AcceptClient();
			cancellableTasks.Add(task1);
			cancellableTasks.Add(task2);

			await Task.WhenAll(cancellableTasks.ToArray());
			if (task1.Result == false || task2.Result == false)
			{
				//Fatal unexpected error do something...
				return false;
			}

			//Phase 2 - time after the survey has concluded in which users view results
			Console.WriteLine($"Results: {data}");
			Console.WriteLine("Sending results to clients");
			SendToAllClients(data);

			return true;
		}

		/// <summary>
		/// The main sequence of running activity vote after running the survey
		/// </summary>
		/// <returns>
		/// Task object of the running process
		/// </returns>
		public async Task RunActivityVote()
		{
			//send first vote to all candidates
			SendToAllClients(voteCalc.GetVote1Candidates());
			SendToAllClients(voteCalc.vote1Timer.ToString());

			//after first vote duration try get replies
			await Task.Delay(1000 * (voteCalc.vote1Timer + voteCalc.coolDown));
			//read each client for their answer
			List<Task<Dictionary<int, string>>> clientVotes1 = new List<Task<Dictionary<int, string>>>();
			foreach (var client in clients)
			{
				clientVotes1.Add(
					AcceptVote1(client)
				);
			}
			//wait for all tasks to complete before returning
			await Task.WhenAll(clientVotes1);
			Console.WriteLine("Stopped accepting votes in phase 1");

			//record all answers
			foreach (var item in clientVotes1)
			{
				if (item.Result != null)
				{
					data.AddVote1Results(item.Result);
				}
			}

			//prepare second vote and send it to all clients
			voteCalc.calcVote2Candidates(data.GetVote1Results());
			SendToAllClients(voteCalc.GetVote2Candidates());
			SendToAllClients(voteCalc.vote2Timer.ToString());

			//after vote 2 duration try get replies
			await Task.Delay(1000 * (voteCalc.vote2Timer + voteCalc.coolDown));

			//read each client for their answer
			List<Task<string>> clientVotes2 = new List<Task<string>>();
			foreach (var client in clients)
			{
				clientVotes2.Add(
					AcceptVote2(client)
				);
			}
			//wait for all tasks to complete before returning
			await Task.WhenAll(clientVotes2);
			Console.WriteLine("Stopped reading votes in phase 2");

			//record all answers
			foreach (var item in clientVotes2)
			{
				if (item.Result != null)
				{
					data.AddVote2Results(item.Result);
				}
			}


			Console.WriteLine("Stopped reading votes in phase 2");

			//prepare result and send it to all clients
			string result = voteCalc.calcFinalResult(data.GetVote2Results());
			data.voteResult = result;
			SendToAllClients(result);

			isVoteConcluded = true;
		}

		/// <summary>
		/// Blocks further clients from entering and answering the survey
		/// <remarks>
		/// Call after the RunSurvey Task has been started to move on in the process
		/// </remarks>
		/// </summary>
		/// <returns>
		/// Task object representing the work
		/// </returns>
		public async Task CloseSurvey() {
			tokenSource.Cancel();
			await Task.WhenAll(cancellableTasks.ToArray());
			return;
		}

		/// <summary>
		/// Starts activity vote with the connected clients
		/// </summary>
		/// <remarks>
		/// RunSurvey task must have been concluded before starting this task
		/// </remarks>
		public void StartActivityVote() {
			//prepare first vote
			isVoteConcluded = false;
			voteCalc = new ActivityVote();
			voteCalc.calcVote1Candidates(survey.emojis, data.GetEmojiResults());
			Task.Run(() =>
			{
				Task voteTask = RunActivityVote();
			});
		}

		/// <summary>
		/// Looping task replying to server discovery broadcasts from the clients so that they can learn the host's address
		/// </summary>
		/// <returns>
		/// Task resulting in a boolean indicating whether the task ended in a fatal error
		/// </returns>
		private async Task<bool> ReplyBroadcast() {

			try
			{
				UdpClient listener = new UdpClient(Const.Network.ServerUDPClientPort);
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				//loop to serve all broadcasts
				while (true)
				{
					Console.WriteLine("Waiting for broadcast");
					Task<UdpReceiveResult> broadcast = listener.ReceiveAsync();

					//allow cancellation of this task between each udp message
					do
					{
						if (token.IsCancellationRequested)
						{
							listener.Close();
							s.Close();
							token.ThrowIfCancellationRequested();
						}
						await Task.WhenAny(new Task[] { Task.Delay(1000), broadcast });
					} while (broadcast.Status != TaskStatus.RanToCompletion);

					//message received
					string message = Encoding.Unicode.GetString(broadcast.Result.Buffer, 0, broadcast.Result.Buffer.Length);
					Console.WriteLine($"Received broadcast from {broadcast.Result.RemoteEndPoint} :");
					Console.WriteLine($" {message}");

					if (message == survey.RoomCode)
					{
						//has this client answered the survey already?
						if (!clientHistory.Contains(broadcast.Result.RemoteEndPoint.Address))
						{
							//prepare message and destination
							byte[] sendbuf = Encoding.Unicode.GetBytes("Connect please");
							IPEndPoint ep = new IPEndPoint(broadcast.Result.RemoteEndPoint.Address, Const.Network.ClientUDPClientPort);

							//reply
							Console.WriteLine($"Replying... EP: {ep}");
							s.SendTo(sendbuf, ep);

						} else
						{
							Console.WriteLine("Old client tried to connect again");
						}
					}
					else
					{
						Console.WriteLine("Received invalid Room Code");
					}
				}
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("ReplyBroadcast task was cancelled gracefully");
				return true;
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine(e);
			}
			catch (SocketException e)
			{
				Console.WriteLine(e);
			}

			//handle unexpected errors
			Console.WriteLine("Fatal error occured, aborting survey.");
			tokenSource.Cancel();
			return false;
		}

		/// <summary>
		/// Looping task allowing new tcp connections to be built to the host
		/// </summary>
		/// <remarks>
		/// Clients are not permanently added to the list of connected clients unless an answer to the survey is received before CloseSurvey call
		/// </remarks>
		/// <returns>
		/// Task object resulting in a boolean indicating whether the task ended in a fatal error
		/// </returns>
		private async Task<bool> AcceptClient() {

			try
			{
				TcpListener listener = new TcpListener(IPAddress.Any, Const.Network.ServerTCPListenerPort);
				listener.Start();

				while (true)
				{
					Console.WriteLine("Waiting to accept tcp client");
					Task<TcpClient> newClient = null;
					newClient = listener.AcceptTcpClientAsync();

					//Allow cancellation of task between adding each client
					do
					{
						if (token.IsCancellationRequested)
						{
							listener.Stop();
							token.ThrowIfCancellationRequested();
						}
						await Task.WhenAny(new Task[] { Task.Delay(1000), newClient });
					} while (newClient.Status != TaskStatus.RanToCompletion);

					//Child task to communicate with new client
					Task childtask = Task.Run(() => ServeNewClient(newClient.Result, token));
				}				
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("AcceptClient task was cancelled gracefully");
				return true;
			}
			catch (InvalidOperationException e)
			{
				Console.WriteLine(e);
			}
			catch (SocketException e)
			{
				Console.WriteLine(e);
			}

			//handle unexpected errors
			Console.WriteLine("Fatal error occured, aborting survey.");
			tokenSource.Cancel();
			return false;
		}

		/// <summary>
		/// Services a freshly joined client by sending initial data and waiting for their emoji answer
		/// </summary>
		/// <param name="client">
		/// The client to serve
		/// </param>
		/// <param name="token">
		/// The cancellation token
		/// </param>
		private async void ServeNewClient(TcpClient client, CancellationToken token)
		{
			try
			{
				//send intro message
				byte[] sendBuffer = Encoding.Unicode.GetBytes(survey.introMessage);
				NetworkStream ns = client.GetStream();
				await ns.WriteAsync(sendBuffer, 0, sendBuffer.Length);

				//wait for emoji from client, expecting 1 int
				byte[] buffer = new byte[4];
				Task<int> emojiReply = ns.ReadAsync(buffer, 0, buffer.Length);

				//allow cancellation of task here.
				do
				{
					if (token.IsCancellationRequested)
					{
						client.Close();
						token.ThrowIfCancellationRequested();
					}
					await Task.WhenAny(new Task[] { Task.Delay(1000), emojiReply });
				} while (emojiReply.Status != TaskStatus.RanToCompletion);

				if (emojiReply.Result == 0)
				{
					//we read nothing out of disconnected network, nice
					//we don't want this client
					return;
				}

				//process reply
				string reply = Encoding.Unicode.GetString(buffer, 0, emojiReply.Result);
				Console.WriteLine($"Bytes read: {emojiReply.Result}");
				Console.WriteLine($"Client sent: {reply}");

				//add to surveydata
				data.AddEmojiResults(int.Parse(reply));

				//add this client to list of clients
				clientCount++;
				clients.Add(client);
				clientHistory.Add(((IPEndPoint)client.Client.RemoteEndPoint).Address);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("Cancelling task, Client was dropped for being slow poke");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Something went wrong in first communication with client: {client.Client.RemoteEndPoint}");
				Console.WriteLine(e);
				client.Close();
			}
		}

		/// <summary>
		/// Tries to read the first phase vote answer from a client
		/// </summary>
		/// <remarks>
		/// Not all clients send a full answer, or an answer at all
		/// </remarks>
		/// <param name="client">
		/// The client to try get the answer from
		/// </param>
		/// <returns>
		/// Task representing the work
		/// </returns>
		private async Task<Dictionary<int, string>> AcceptVote1(TcpClient client) {

			try
			{
				//waiting for vote for limited time by setting network stream read timeout
				byte[] buffer = new byte[2048];
				NetworkStream ns = client.GetStream();
				int bytesRead = 0;
				Console.WriteLine("Reading client vote 1");

				//if client does not reply anything, data is not available.
				if (!ns.DataAvailable)
				{
					return null;
				}

				bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);

				Console.WriteLine($"DEBUG: AcceptVotes 1 read {bytesRead} bytes from: {client}");

				//if client has exited read results in 0 bytes read
				if (bytesRead <= 0)
				{
					return null;
				}

				//read was successful, expecting JSON string containing Dictionary<int, string>
				return JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.Unicode.GetString(buffer, 0, bytesRead));

			}
			catch (JsonException e)
			{
				Console.WriteLine("Received bad JSON");
				Console.WriteLine(e);
				
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Connection lost to client");
				Console.WriteLine(e);
			}
			catch (System.IO.IOException e)
			{
				Console.WriteLine("Error reading socket or network");
				Console.WriteLine(e);
			}

			//so... you have chosen death
			clients.Remove(client);
			clientCount--;
			return null;
		}

		/// <summary>
		/// Tries to read the second phase vote answer from the client
		/// </summary>
		/// <remarks>
		/// Not all clients send a full answer, or an answer at all
		/// </remarks>
		/// <param name="client">
		/// The client to try get the answer from
		/// </param>
		/// <returns>
		/// Task representing the work
		/// </returns>
		private async Task<string> AcceptVote2(TcpClient client) {
			try
			{
				//waiting for vote for limited time by setting network stream read timeout
				byte[] buffer = new byte[128];
				NetworkStream ns = client.GetStream();
				int bytesRead = 0;
				Console.WriteLine("Reading client vote 2");

				//if client does not reply anything, data is not available.
				if (!ns.DataAvailable)
				{
					return null;
				}

				bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
				Console.WriteLine($"DEBUG: AcceptVotes 2 read {bytesRead} bytes from: {client}");

				//if client has exited read results in 0 bytes read
				if (bytesRead == 0)
				{
					return null;
				}

				//read was successful, expecting string containing final activity vote
				return Encoding.Unicode.GetString(buffer, 0, bytesRead);

			}
			catch (JsonException e)
			{
				Console.WriteLine("Received bad JSON");
				Console.WriteLine(e);
				
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Connection lost to client");
				Console.WriteLine(e);
				
			}
			catch (System.IO.IOException e)
			{
				Console.WriteLine("Error reading socket or network");
				Console.WriteLine(e);
			}

			//we don't do that here
			clients.Remove(client);
			clientCount--;

			return null;
		}

		/// <summary>
		/// Sends all clients a serialized JSON string of an object
		/// </summary>
		/// <param name="obj">
		/// The object representing the data to be sent
		/// </param>
		private void SendToAllClients(object obj) {

			//prepare data for transmission
			byte[] message;
			message = Encoding.Unicode.GetBytes(
				JsonConvert.SerializeObject(obj)
			);

			//iterate each recorded client
			foreach (var client in clients)
			{
				//catch errors per client
				try
				{
					NetworkStream ns = client.GetStream();

					if (ns.CanWrite)
					{
						ns.Write(message, 0, message.Length);
					}
				}
				catch (ObjectDisposedException e)
				{
					Console.WriteLine($"Connection lost with client: {client.Client.RemoteEndPoint}. Dropping client");
					Console.WriteLine(e);
					clients.Remove(client);
					clientCount--;
				}
				catch (System.IO.IOException e)
				{
					Console.WriteLine("Error reading socket or network");
					Console.WriteLine(e);
					clients.Remove(client);
					clientCount--;
				}
			}
		}

		/// <summary>
		/// Sends all clients a message
		/// </summary>
		/// <param name="text">
		/// The message to be sent
		/// </param>
		private void SendToAllClients(string text)
		{

			//prepare data for transmission
			byte[] message;
			message = Encoding.Unicode.GetBytes( text );

			//iterate each recorded client
			foreach (var client in clients)
			{
				//catch errors per client
				try
				{
					NetworkStream ns = client.GetStream();

					if (ns.CanWrite)
					{
						ns.Write(message, 0, message.Length);
					}
				}
				catch (ObjectDisposedException e)
				{
					Console.WriteLine($"Connection lost with client: {client.Client.RemoteEndPoint}. Dropping client");
					Console.WriteLine(e);
					clients.Remove(client);
					clientCount--;
				}
				catch (System.IO.IOException e)
				{
					Console.WriteLine("Error reading socket or network");
					Console.WriteLine(e);
					clients.Remove(client);
					clientCount--;
				}
			}
		}

		/// <summary>
		/// Sufficiently terminates the host processes and client connections when the survey concludes or is aborted
		/// </summary>
		public void DestroyHost() {
			//cancel tasks
			tokenSource.Cancel();
			//close all connections
			foreach (var item in clients)
			{
				item.Close();
			}
		}
	}
}
