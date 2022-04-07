using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prototype
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OdotetaanVastauksiaOpe : ContentPage
    {
        public string RoomCode { get; set; }
        public OdotetaanVastauksiaOpe()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            Survey s = SurveyManager.GetInstance().GetSurvey();
            RoomCode = s.RoomCode;
            BindingContext = this;

            Host();
        }

        private async void Host()
        {
            //hakee osallistujien määrän?
            //clientCount = Main.GetInstance().host.clientCount;

            if (!await Main.GetInstance().HostSurvey())
            {
                //host survey ended in a fatal unexpected error, aborting survey.
                //pop to root and display error
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti3", "Tapahtui odottamaton virhe.", "OK");
            }
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            await UpdateProgressBar(0, 60000);
        }

        async Task UpdateProgressBar(double Progress, uint time)
        {
            await progressBar.ProgressTo(Progress, time, Easing.Linear);
            //siirtyy eteenpäin automaattisesti 60 sekunnin jälkeen
            if(progressBar.Progress == 0)
            {
                await Main.GetInstance().host.CloseSurvey();
                await Navigation.PushAsync(new LisätiedotHost());
            }
        }

        private async void LopetaClicked(object sender, EventArgs e)
        {
            //Back to main and error, if nobody joined the survey!
            /*if (Main.GetInstance().host.clientCount == 0)
            {
                Main.GetInstance().host.DestroyHost();
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti", "Kyselyyn ei saatu yhtään vastausta", "OK");
                return;
            }*/

            await Main.GetInstance().host.CloseSurvey();
            await Navigation.PushAsync(new LisätiedotHost());
        }
    }
    }
