using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public TutorialPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "使い方";
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("HomePage");
        }));
    }
}
