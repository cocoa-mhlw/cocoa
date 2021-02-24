using Covid19Radar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyOtherPage : ContentPage
    {
        public NotifyOtherPage()
        {
            InitializeComponent();
        }

        void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var button = (RadioButton)sender;
            if (button.IsChecked)
            {
                // Xamarin Forms 5 change Text to Content Prop
                (BindingContext as NotifyOtherPageViewModel).OnClickRadioButtonIsTrueCommand(button.Content.ToString());
            }
        }
    }
}