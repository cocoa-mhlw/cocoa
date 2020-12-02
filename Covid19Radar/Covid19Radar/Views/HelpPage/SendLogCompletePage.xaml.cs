using Covid19Radar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendLogCompletePage : ContentPage
    {
        public SendLogCompletePage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = BindingContext as SendLogCompletePageViewModel;
            vm.OnBackButtonPressedCommand.Execute(null);
            return true;
        }
    }
}
