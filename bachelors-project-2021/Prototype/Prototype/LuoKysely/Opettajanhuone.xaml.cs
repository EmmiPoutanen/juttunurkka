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
    public partial class Opettajanhuone : ContentPage
    {
        public string SelectedSurvey { get; set; }
        public IList<string> Surveys { get; set; }

        public Opettajanhuone()
        {
            InitializeComponent();
            TallennetutKyselyt();
        }

        void TallennetutKyselyt()
        {
            Surveys = new List<String>();
            SurveyManager manager = SurveyManager.GetInstance();

            //NavigationPage.SetHasBackButton(this, false);

            Surveys = manager.GetTemplates();
            Surveys.Insert(0, "Oletuskysely");

            BindingContext = this;
        }

        async void UusiJuttunurkkaClicked(object sender, EventArgs e)
        {

            //siirrytään "luo uus kysely 1/3" sivulle 
            await Navigation.PushAsync(new LuoKyselyJohdatus());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            PickerList.Focus();
        }

        async void OletusClicked(object sender, EventArgs e)
        {
            SurveyManager.GetInstance().SetDefaultSurvey();
            KyselynTarkastelu.canDelete = false;
            KyselynTarkastelu.canEdit = false;
            await Navigation.PushAsync(new KyselynTarkastelu());
        }

        async void AvaaClicked(object sender, EventArgs e)
        {
            string surveyName = SelectedSurvey + ".txt";
            SurveyManager manager = SurveyManager.GetInstance();
            manager.LoadSurvey(surveyName);

            Console.WriteLine(surveyName);

            KyselynTarkastelu.canDelete = true;
            KyselynTarkastelu.canEdit = true;
            await Navigation.PushAsync(new KyselynTarkastelu());

        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            SelectedSurvey = picker.Items[picker.SelectedIndex];


            if (selectedIndex == 0)
            {
                OletusClicked(sender, e);  //avaa oletuskyselyn
            }
            else
            {
                AvaaClicked(sender, e);   //avaa tallennetun kyselyn
            }

            //avaa tallennetun kyselyn
            /*if (selectedIndex != 0)
            {
                AvaaClicked(sender, e);
            }*/
        }


    }
}