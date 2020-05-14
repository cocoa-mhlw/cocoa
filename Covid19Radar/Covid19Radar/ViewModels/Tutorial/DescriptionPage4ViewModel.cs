using System.Collections.Generic;
using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DescriptionPage4ViewModel : ViewModelBase
    {
        public DescriptionPage4ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleHowItWorks;
        }

        public Command OnClickNext => new Command(async () =>
        {
            await NavigationService.NavigateAsync("ConsentByUserPage");
        });
    }
}