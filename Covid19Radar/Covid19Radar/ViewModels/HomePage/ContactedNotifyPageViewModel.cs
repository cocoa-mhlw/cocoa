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
using Acr.UserDialogs;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Xamarin.Essentials;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        public ContactedNotifyPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
        }
        public Command OnClickByForm => new Command(async () =>
        {
            var uri = "http://bing.com";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });
        public Command OnClickByPhone => new Command(async () =>
        {
            var uri = "http://bing.com";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });

    }
}
