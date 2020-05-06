using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System;

namespace Covid19Radar.ViewModels
{
    public class UserStatusPickerItem
    {
        public int UserStatusCode { get; set; }
        public string UserStatusName { get; set; }
    }

    public class UserStatusPageViewModel : ViewModelBase
    {
        private readonly UserDataService _userDataService;
        private UserDataModel _userData;

        public UserStatusPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            _userDataService = App.Current.Container.Resolve<UserDataService>();
            _userData = _userDataService.Get();
        }

        public Command OnClickSave => (new Command(async () =>
        {
            await _userDataService.SetAsync(_userData);
            await NavigationService.NavigateAsync("HomePage");
        }));
    }
}
