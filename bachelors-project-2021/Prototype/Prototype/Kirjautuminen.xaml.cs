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
    public partial class Kirjautuminen : ContentPage
    {
        public Kirjautuminen()
        {

            InitializeComponent();
        }

        private void MyClick(object sender, EventArgs e)
        {
            if(txtUsername.Text =="opettaja" && txtPassword.Text == "opehuone")
            { 
            
            }
            else
            {
                DisplayAlert("VIRHE", "Vaara salasana tai kayttajatunnus", "OK");
            }
        }

        async void KirjauduClicked(object sender, EventArgs e)
        {
            if (txtUsername.Text == "opettaja" && txtPassword.Text == "opehuone")
            {
                await Navigation.PushAsync(new Opettajanhuone());
            }
            else
            {
                await DisplayAlert("VIRHE", "Vaara salasana tai kayttajatunnus", "OK");
            }
            
        }
    }
}