using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using Prism.Navigation.Xaml;

namespace Covid19Radar.ViewModels
{
    public class UserStatusPickerItem
    {
        public int UserStatusCode { get; set; }
        public string UserStatusName { get; set; }
    }

    public class NotifyOtherPageViewModel : ViewModelBase
    {
        private readonly UserDataService _userDataService;
        private UserDataModel _userData;
        public int statusCode;
        public HttpDataService httpDataService;

        public NotifyOtherPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            _userDataService = App.Current.Container.Resolve<UserDataService>();
            _userData = _userDataService.Get();
        }

        public Command OnClickSave => (new Command(async () =>
        {
            await _userDataService.SetAsync(_userData);
            await NavigationService.NavigateAsync("MainPage");
        }));

        public Command LearnMoreCommande => (new Command(async () =>
        {
            await Xamarin.Essentials.Browser.OpenAsync(Resources.AppResources.NotifyOthersLearnMoreUrl);
        }));
    }
}
