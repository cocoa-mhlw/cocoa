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
    public class ThankYouNotifyOtherPageViewModel : ViewModelBase
    {
        public ThankYouNotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
        }
        public Command OnClickShareApp => new Command(async () =>
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.AppStoreUrl,
                    Title = AppConstants.AppName
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.GooglePlayUrl,
                    Title = AppConstants.AppName
                });
            }
        });

    }
}
