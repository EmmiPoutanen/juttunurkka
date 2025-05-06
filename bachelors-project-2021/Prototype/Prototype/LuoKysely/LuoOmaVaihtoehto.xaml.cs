using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;

namespace Prototype.LuoKysely
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LuoOmaVaihtoehto : ContentPage
    {
        private readonly Action<string> _callback;

        public LuoOmaVaihtoehto(Action<string> callback)
        {
            InitializeComponent();
            _callback = callback;
        }

        private async void TallennaButtonClicked(object sender, EventArgs e)
        {
            string input = OmaTeksti.Text?.Trim() ?? "";
            _callback?.Invoke(input);
            await Navigation.PopAsync();
        }

        private async void EdellinenButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
