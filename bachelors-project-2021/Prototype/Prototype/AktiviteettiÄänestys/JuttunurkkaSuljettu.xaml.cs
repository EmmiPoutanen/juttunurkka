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
    public partial class JuttunurkkaSuljettu : ContentPage
    {
        public JuttunurkkaSuljettu()
        {
            InitializeComponent();
            
        }

        //Device back button disabled
        protected override bool OnBackButtonPressed()
        {
            return true;

        }

        async void AlkuunClicked(object sender, EventArgs e)
        {
            //palataan etusivulle
            await Navigation.PopToRootAsync();
        }

        //hae lukko ja kirja yms peukku kuvat
    }
}