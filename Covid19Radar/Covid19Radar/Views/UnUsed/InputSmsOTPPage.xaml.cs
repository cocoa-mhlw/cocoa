using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class InputSmsOTPPage : ContentPage
    {
        public InputSmsOTPPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OtpForm.SubscribeEvents();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            OtpForm.UnsubscribeEvents();
        }
    }
}