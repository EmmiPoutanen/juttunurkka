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
                await DisplayAlert("Kysely suljettiin automaattisesti", "Tapahtui odottamaton virhe.", "OK");
            }
        }

        private async void LopetaClicked(object sender, EventArgs e)
        {
            //Back to main and error, if nobody joined the survey!
            if (Main.GetInstance().host.clientCount == 0)
            {
                Main.GetInstance().host.DestroyHost();
                await Navigation.PopToRootAsync();
                await DisplayAlert("Kysely suljettiin automaattisesti", "Kyselyyn ei saatu yhtään vastausta", "OK");
                return;
            }

            await Main.GetInstance().host.CloseSurvey();
            await Navigation.PushAsync(new TabbedViewHost());
        }
    }
    }
}