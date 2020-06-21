using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage3 : ContentPage
    {
        public TutorialPage3()
        {
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await activity_indicator.ProgressTo(1.0, 900, Easing.SpringIn);

        }

        public void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            activity_indicator.IsVisible = true;


        }

        public void OnNavigated(object sender, WebNavigatedEventArgs e)
        {

            activity_indicator.IsVisible = false;

        }
    }
}