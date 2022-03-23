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
    public partial class Omakysymys : ContentPage
    {
        public Omakysymys()
        {
            InitializeComponent();

            BindingContext = this;
        }

        async void JatkaButtonClicked(object sender, EventArgs e)
        {
            //kyselyn johdatuslause asetetaan.
            //SurveyManager.GetInstance().GetSurvey().introMessage = selectedItem;


            //siirrytään "luo uus kysely 2/3" sivulle 
            await Navigation.PushAsync(new LuoKyselyEmojit());
        }

        async void EdellinenButtonClicked(object sender, EventArgs e) //menee vielä kokonaan alkuun
        {
            //survey resetoidaan
            SurveyManager.GetInstance().ResetSurvey();

            //Jos ollaan edit tilassa, niin siirrytään takaisin kyselyntarkastelu sivulle, muutoin main menuun
            if (Main.GetInstance().GetMainState() == Main.MainState.Editing)
            {
                Main.GetInstance().BrowseSurveys();
                await Navigation.PopAsync();
            }
            else
            {
                // siirrytään etusivulle
                await Navigation.PopToRootAsync();
            }
        }
    }
}