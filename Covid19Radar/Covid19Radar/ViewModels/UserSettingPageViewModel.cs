using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UserSettingPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private UserDataService _userDataService;
        private UserDataModel _userData;
        public UserSettingPageViewModel(INavigationService navigationService, UserDataService userdataservice)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "UserSettingPage";
        }
        public Command OnChangeStatusOverInfection => (new Command(() =>
        {
            _navigationService.NavigateAsync("SmsVerificationPage");
        }));
    }
}
