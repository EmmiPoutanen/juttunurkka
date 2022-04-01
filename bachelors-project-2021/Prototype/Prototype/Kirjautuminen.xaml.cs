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
    }
}