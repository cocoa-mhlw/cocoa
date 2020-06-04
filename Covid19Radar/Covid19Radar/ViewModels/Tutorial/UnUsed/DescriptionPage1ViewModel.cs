using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DescriptionPage1ViewModel : ViewModelBase
    {
        public DescriptionPage1ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }

        public Command OnClickNext => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(DescriptionPage3));
        });
    }
}