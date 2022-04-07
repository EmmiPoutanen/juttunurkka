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
    public partial class Esikatselu : ContentPage
    {
        public string introMessage { get; set; }
        public string RoomCode { get; set; }
        public string Name { get; set; }
        public IList<CollectionItem> Emojit { get; private set; } = null;

        public class CollectionItem
        {
            public Emoji Item { get; set; } = null;
            public IList<string> ActivityChoises { get; set; } = null;
        }

        public Esikatselu()
        {
            InitializeComponent();
            Survey s = SurveyManager.GetInstance().GetSurvey();
            introMessage += s.introMessage;
            RoomCode = s.RoomCode;
            Name = s.Name;

            Emojit = new List<CollectionItem>();
            List<Emoji> temp = s.emojis;

            foreach (var item in temp)
            {
                CollectionItem i = new CollectionItem();
                i.Item = item;
                i.ActivityChoises = item.activities;
                Emojit.Add(i);
            }

            BindingContext = this;
        }


     /*   async void KeskeytäButtonClicked(object sender, EventArgs e)
        {
            var res=await DisplayAlert("Oletko varma että haluat keskeyttää juttunurkan luomisen?", "", "Kyllä", "Ei");
            if (res == true)
            {
                await Navigation.PushAsync(new Opettajanhuone());
            }
            else
                return;
        }*/

        async void ValmisButtonClicked(object sender, EventArgs e)
        {
            var res = await DisplayAlert("Juttunurkka tallennettu! Haluatko avata juttunurkan heti?", "", "Kyllä", "Ei");

            if (res == true)
            {
                //siirrytään odottamaan osallistujia
                await Navigation.PushAsync(new OdotetaanOsallistujiaOpettaja());

            }
            else
            {
                await Navigation.PushAsync(new Opettajanhuone());
            }

        }
    }
}