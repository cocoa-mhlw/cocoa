using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class HelpPage1ViewModel : ViewModelBase
    {
        public HelpPage1ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage1Title;
        }

    }
}