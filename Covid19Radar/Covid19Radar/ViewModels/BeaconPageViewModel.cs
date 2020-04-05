using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
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
    public class BeaconPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public BeaconPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Beacon Page";
            AppUtils.CheckPermission();

            // Create User
            // TODO Check Register User (UUID.Major.Minor) or New
            // POST New User and Store local properities
            if (!Application.Current.Properties.ContainsKey("UserData"))
            {
                // Access REST API and new id case
                UserData userData = new UserData();
                userData.Uuid = AppConstants.AppUUID;
                userData.Major = "23";
                userData.Minor = "45";
                Application.Current.Properties["UserData"] = userData;
            }
        }

    }
}
