using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage5ViewModel : ViewModelBase
    {
        public TutorialPage5ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }

        public Command OnClickEnable => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });
        public Command OnClickDisable => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });

    }
}