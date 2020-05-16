using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DescriptionPage2ViewModel : ViewModelBase
    {
        public DescriptionPage2ViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }

        public Command OnClickNext => new Command(async () =>
        {
            await NavigationService.NavigateAsync("DescriptionPage3");
        });

    }
}