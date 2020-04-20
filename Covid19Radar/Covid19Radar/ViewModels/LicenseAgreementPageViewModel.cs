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
    public class LicenseAgreementPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public LicenseAgreementPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "License Agreement";
        }

        public Command OnClickPrev => (new Command(() =>
        {
            _navigationService.NavigateAsync("HomePage");
        }));


    }
}
