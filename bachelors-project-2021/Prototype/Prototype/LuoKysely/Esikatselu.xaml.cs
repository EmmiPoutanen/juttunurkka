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
        public Esikatselu()
        {
            InitializeComponent();
            Survey s = SurveyManager.GetInstance().GetSurvey();
            introMessage += s.introMessage;
            RoomCode = s.RoomCode;
            Name = s.Name;  
            BindingContext = this;
        }

        async void KeskeytäButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Opettajanhuone());
        }

        async void ValmisButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Opettajanhuone());
        }
    }
}