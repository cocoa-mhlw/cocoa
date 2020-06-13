using System;
using System.Collections.Generic;
using System.Threading;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;

        public TutorialPage4ViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            userData = this.userDataService.Get();
        }

        public Command OnClickEnable => new Command(async () =>
        {
            // Platform Split
            if (Device.RuntimePlatform == Device.iOS)
            {
                try
                {
                    UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);
                    await ExposureNotification.StartAsync();
                    while (true)
                    {
                        Thread.Sleep(1000);
                        await ExposureNotification.StartAsync();

                        Status status = await ExposureNotification.GetStatusAsync();
                        if (status == Status.Active)
                        {
                            userData.IsExposureNotificationEnabled = true;
                            await userDataService.SetAsync(userData);
                            break;
                        }
                    }
                    UserDialogs.Instance.HideLoading();
                }
                catch (Exception)
                {
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync("Exposure Notificationを起動できませんでした", "Error", Resources.AppResources.ButtonOk);
                    Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();
                    return;
                }
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                userData.IsExposureNotificationEnabled = true;
                await userDataService.SetAsync(userData).ConfigureAwait(true);
                await exposureNotificationService.StartExposureNotification().ConfigureAwait(true);
            }

            await NavigationService.NavigateAsync(nameof(TutorialPage5));

        });
        public Command OnClickDisable => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage5));
        });
    }
}