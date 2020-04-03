using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class StartTutorialPage : ContentPage
    {
        public StartTutorialPage()
        {
            InitializeComponent();
            TopMainImage.Source = ImageSource.FromResource("Covid19Radar.Images.TopMainImg.png");
        }
    }
}