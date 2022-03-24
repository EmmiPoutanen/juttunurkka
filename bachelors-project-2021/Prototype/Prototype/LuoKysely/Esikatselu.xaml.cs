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
        public Esikatselu()
        {
            InitializeComponent();
            Survey s = SurveyManager.GetInstance().GetSurvey();
            introMessage += s.introMessage;
            BindingContext = this;
        }
    }
}