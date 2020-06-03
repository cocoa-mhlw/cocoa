using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage2ViewModel : ViewModelBase
    {
        public HelpPage2ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage2Title;
        }

        public Command OnClickNext => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(HelpPage4));
        });
    }
}