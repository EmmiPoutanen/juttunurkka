
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace Prototype
{
	/// <summary>
	/// Provides functionality to communicate with a survey host in order to answer a survey
	/// </summary>

	// DISCLAIMER!
	// Throughout this project I was *learning* networking and async programming nearly from the ground up
	// I cannot guarantee the safety of this code and I apologize for the amount of duplicate code
	// No sensitive data is exchanged luckily!
	// Many of the functions here can be converted to generic ReceiveData and SendData functions to improve efficiency and error tolerance

	public class SurveyClient
	{
		/// <value>
		/// Instance of TcpClient object connected to a node hosting the survey in the local network
		/// </value>
		private TcpClient client = null;

		/// <value>
		/// Attended survey's intro message
		/// </value>
		public string intro { get; private set; } = "";

		/// <value>
		/// Instance of SurveyData object containing the concluded survey's results
		/// </value>
		public SurveyData summary { get; private set; } = null;

		/// <value>
		/// Dictionary of candidates in the first phase of vote. Key: emojiID, Value: list of activity choises
		/// </value>
		public Dictionary<int, IList<string>> voteCandidates1 { get; private set; } = null;

		/// <value>
		/// Integer value containing seconds for the first phase of vote timer
		/// </value>
		public int vote1Time { get; private set; } = 0;

		/// <value>
		/// List of candidates in the second phase of vote
		/// </value>
		public List<string> voteCandidates2 { get; private set; } = null;

		/// <value>
		/// Integer value containing seconds for the second phase of vote timer
		/// </value>
		public int vote2Time = 0;

		/// <value>
		/// Attended survey's final vote result
		/// </value>
		public string voteResult = null;

		/// <value>
		/// List of running Task instances which can be cancelled
		/// </value>
		private List<Task> cancellableTasks;

		/// <value>
		/// Instance of CancellationTokenSource to call for cancellation of tasks in the cancellableTasks list
		/// </value>
		private CancellationTokenSource tokenSource;

		/// <value>
		/// Instance of CancellationToken fed to the Tasks in the cancellableTasks list
		/// </value>
		private CancellationToken token;

		/// <summary>
		/// Default constructor
		/// <remarks>
		/// The created instance does not start running any tasks or connect to a host automatically
		/// </remarks>
		/// </summary>
		public SurveyClient() {
			cancellableTasks = new List<Task>();
			tokenSource = new CancellationTokenSource();
			token = tokenSource.Token;
		}

		/// <summary>
		/// Tries to find a host in the local network which hosts a survey with the given room code
		/// </summary>
		/// <remarks>
		/// Upon success the SurveyClient receives values for class parameters client and intro
		/// </remarks>
		/// <param name="RoomCode">
		/// The room code of the hosted survey the client intends to join
		/// </param>
		/// <returns>
		/// Task object resulting in a boolean indicating whether connection to a host was built
		/// </returns>
		public async Task<bool> LookForHost(string RoomCode) {

			try
			{
				byte[] message = Encoding.Unicode.GetBytes(RoomCode);

				Socket sendOut = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				sendOut.EnableBroadcast = true;

				UdpClient listener = new UdpClient() { EnableBroadcast = true };
				listener.Client.Bind(new IPEndPoint(IPAddress.Any, Const.Network.ClientUDPClientPort));
				
				Task<UdpReceiveResult> reply = listener.ReceiveAsync();

				for (int i = 0; i < 3; i++)
				{	
					//broadcast and wait
					int bytesSent = sendOut.SendTo(message, new IPEndPoint(IPAddress.Broadcast, Const.Network.ServerUDPClientPort));
					Console.WriteLine($"Bytes sent: {bytesSent}");
					await Task.Delay(1000);

					//did we get a reply?
					Console.WriteLine($"Reply Status: {reply.Status}");
					if (reply.Status == TaskStatus.RanToCompletion)
					{
						Console.WriteLine($"Received reply to broadcast from: {reply.Result.RemoteEndPoint}");
						string replyMessage = Encoding.Unicode.GetString(reply.Result.Buffer, 0, reply.Result.Buffer.Length);
						Console.WriteLine($"Message: {replyMessage}");

						try
						{	
							//attempt to connect
							client = new TcpClient();
							client.Connect(new IPEndPoint(reply.Result.RemoteEndPoint.Address, Const.Network.ServerTCPListenerPort));

							//receive intro message
							NetworkStream ns = client.GetStream();
							byte[] readBuffer = new byte[128];
							int bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

							if (bytesRead == 0)
							{
								Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
								return false;
							}
							intro = Encoding.Unicode.GetString(readBuffer, 0, bytesRead);

							//if no error occurs return success
							return true;

						}
						catch (ObjectDisposedException e)
						{
							Console.WriteLine("Host abruptly closed connection, most likely");
							Console.WriteLine(e);
						}
						catch (NotSupportedException e)
						{
							Console.WriteLine("Stream does not support that operation");
							Console.WriteLine(e);
						}
						finally 
						{
							//received garbage, lets try that again.
							reply = listener.ReceiveAsync();
						}
					}
				}

				listener.Close();

			}
			catch (SocketException e)
			{
				Console.WriteLine("Socket exception occured in LookForHost");
				Console.WriteLine(e);
			}
			

			return false;
		}

		/// <summary>
		/// Tries to send emoji answer to host
		/// </summary>
		/// <param name="emojiID">
		/// Integer ID of the emoji sent to the host
		/// </param>
		/// <returns>
		/// Task object resulting in a boolean indicating whether message was sent successfully
		/// </returns>
		public async Task<bool> SendResult(string emojiID) {

			try
			{
				//prepare message
				byte[] bytes = Encoding.Unicode.GetBytes(emojiID);

				//send
				NetworkStream ns = client.GetStream();
				await ns.WriteAsync(bytes, 0, bytes.Length);

				//no error, returning success
				return true;
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Host abruptly closed connection, most likely");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;
		}

		/// <summary>
		/// Tries to send an answer to the first phase of the vote to the host
		/// </summary>
		/// <param name="answer">
		/// Dictionary containing the answer. Key: emojiID, Value: the chosen activity
		/// </param>
		/// <returns>
		/// Task object resulting in a boolean indicating whether message was sent successfully
		/// </returns>
		public async Task<bool> SendVote1Result(Dictionary<int, string> answer)
		{

			try
			{
				//prepare message
				byte[] bytes = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(answer));

				//send
				NetworkStream ns = client.GetStream();
				await ns.WriteAsync(bytes, 0, bytes.Length);

				//no error, returning success
				return true;
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Host abruptly closed connection, most likely");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;
		}

		/// <summary>
		/// Tries to send an answer to the second phase of the vote to the host
		/// </summary>
		/// <param name="answer">
		/// The chosen activity
		/// </param>
		/// <returns>
		/// Task object resulting in a boolean indicating whether the message was sent successfully
		/// </returns>
		public async Task<bool> SendVote2Result(string answer)
		{

			try
			{
				//prepare message
				byte[] bytes = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(answer));

				//send
				NetworkStream ns = client.GetStream();
				await ns.WriteAsync(bytes, 0, bytes.Length);

				//no error, returning success
				return true;
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Host abruptly closed connection, most likely");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;
		}

		/// <summary>
		/// Tries to receive and parse a JSON string containing the summary of the concluded survey
		/// </summary>
		/// <returns>
		/// Task resulting in a boolean indicating whether the message was received successfully
		/// </returns>
		public async Task<bool> ReceiveSurveyDataAsync() {

			try
			{
				NetworkStream ns = client.GetStream();
				byte[] readBuffer = new byte[8192];
				int bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				if (bytesRead == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				summary = JsonConvert.DeserializeObject<SurveyData>(Encoding.Unicode.GetString(readBuffer, 0, bytesRead));
				Console.WriteLine($"Received summary: {summary}");
				return true;
			}
			catch (JsonException e)
			{
				Console.WriteLine("Received bad Json");
				Console.WriteLine(e);
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"Connection closed or lost to server at: {client.Client.RemoteEndPoint}");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}
			catch (IOException e)
			{
				Console.WriteLine(e);
			}

			return false;
		}

		/// <summary>
		/// Tries to receive and parse a JSON string containing candidates for the first phase of the vote
		/// </summary>
		/// <returns>
		/// Task resulting in a boolean indicating whether the message was received successfully
		/// </returns>
		public async Task<bool> ReceiveVote1Candidates()  {

			try
			{
				NetworkStream ns = client.GetStream();
				byte[] readBuffer = new byte[2048];
				Console.WriteLine("Waiting for activity vote");
				Task<int> bytesReadTask = ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				//allow cancellation of this task
				do
				{
					if (token.IsCancellationRequested)
					{
						return false;
					}
					await Task.Delay(1000);
				} while (bytesReadTask.Status != TaskStatus.RanToCompletion);

				if (bytesReadTask.Result == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				Console.WriteLine($"Bytes read: {bytesReadTask}");

				//expecting JSON string containing Dictionary<int, IList<string>>
				voteCandidates1 = JsonConvert.DeserializeObject<Dictionary<int, IList<string>>>(Encoding.Unicode.GetString(readBuffer, 0, bytesReadTask.Result));
				Console.WriteLine("Received vote 1 candidates");

				//next, receive vote time
				readBuffer = new byte[64];
				Console.WriteLine("Waiting for vote 1 timer");
				int bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				if (bytesRead == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				Console.WriteLine($"Bytes read: {bytesRead}");

				//expecting string containing int
				vote1Time = int.Parse(Encoding.Unicode.GetString(readBuffer, 0, bytesRead));
				return true;
			}
			catch (JsonException e) 
			{
				Console.WriteLine("Received bad JSON");
				Console.WriteLine(e);
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"Connection closed or lost to server at: {client.Client.RemoteEndPoint}");
				Console.WriteLine(e);
			}
			catch (FormatException e)
			{
				Console.WriteLine("Received bad int");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;

		}

		/// <summary>
		/// Tries to receive and parse a JSON string containing candidates for the second phase of the vote
		/// </summary>
		/// <returns>
		/// Task resulting in a boolean indicating whether the message was received successfully
		/// </returns>
		public async Task<bool> ReceiveVote2Candidates()
		{

			try
			{
				NetworkStream ns = client.GetStream();
				byte[] readBuffer = new byte[2048];
				Console.WriteLine("Waiting for activity vote 2");
				int bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				if (bytesRead == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				Console.WriteLine($"Bytes read: {bytesRead}");

				//expecting JSON string containing List<string>
				voteCandidates2 = JsonConvert.DeserializeObject<List<string>>(Encoding.Unicode.GetString(readBuffer, 0, bytesRead));
				Console.WriteLine("Received vote 2 candidates");

				//next, receive vote time
				readBuffer = new byte[64];
				Console.WriteLine("Waiting for vote 2 timer");
				bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				if (bytesRead == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				Console.WriteLine($"Bytes read: {bytesRead}");

				//expecting string containing int
				vote2Time = int.Parse(Encoding.Unicode.GetString(readBuffer, 0, bytesRead));
				return true;
			}
			catch (JsonException e)
			{
				Console.WriteLine("Received bad JSON");
				Console.WriteLine(e);
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"Connection closed or lost to server at: {client.Client.RemoteEndPoint}");
				Console.WriteLine(e);
			}
			catch (FormatException e)
			{
				Console.WriteLine("Received bad int");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;

		}

		/// <summary>
		/// Tries to receive a message containing the final result of the vote
		/// </summary>
		/// <returns>
		/// Task resulting in a boolean indicating whether the message was received successfully
		/// </returns>
		public async Task<bool> ReceiveVoteResult()
		{

			try
			{
				NetworkStream ns = client.GetStream();
				byte[] readBuffer = new byte[256];
				Console.WriteLine("Waiting for vote result");
				int bytesRead = await ns.ReadAsync(readBuffer, 0, readBuffer.Length);

				if (bytesRead == 0)
				{
					Console.WriteLine("Somehow we just read something from disconnected network, this is fine.");
					return false;
				}

				Console.WriteLine($"Bytes read: {bytesRead}");

				//expecting string containing voteResult
				voteResult = Encoding.Unicode.GetString(readBuffer, 0, bytesRead);
				Console.WriteLine("Received vote result");

				return true;
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"Connection closed or lost to server at: {client.Client.RemoteEndPoint}");
				Console.WriteLine(e);
			}
			catch (NotSupportedException e)
			{
				Console.WriteLine("Stream does not support that operation");
				Console.WriteLine(e);
			}

			return false;

		}

		/// <summary>
		/// Sufficiently terminates the client instance by closing the TCP connection and cancelling cancellable tasks
		/// </summary>
		public async void DestroyClient() {

			//cancel all cancellable tasks
			tokenSource.Cancel();
			await Task.WhenAll(cancellableTasks.ToArray());
			client.Close();
		}
	}
}
