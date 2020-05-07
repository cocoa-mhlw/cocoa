using Covid19Radar.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase, IDisposable
    {
        public ExposuresPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.MainExposures;
            MessagingCenter.Instance.Subscribe<ExposureNotificationHandler>(this, "exposure_info_changed", h =>
                Device.BeginInvokeOnMainThread(() =>
                {
                    ExposureInformation.Clear();
                    foreach (var i in LocalStateManager.Instance.ExposureInformation)
                        ExposureInformation.Add(i);
                }));

        }
        public bool EnableNotifications
        {
            get => LocalStateManager.Instance.EnableNotifications;
            set
            {
                LocalStateManager.Instance.EnableNotifications = value;
                LocalStateManager.Save();
            }
        }

        public ObservableCollection<Xamarin.ExposureNotifications.ExposureInfo> ExposureInformation
            => new ObservableCollection<Xamarin.ExposureNotifications.ExposureInfo>
            {
#if DEBUG
                new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-7), TimeSpan.FromMinutes(30), 70, 6, Xamarin.ExposureNotifications.RiskLevel.High),
                new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(10), 40, 3, Xamarin.ExposureNotifications.RiskLevel.Low),
                new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(40), 20, 6, Xamarin.ExposureNotifications.RiskLevel.Medium),
                new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(60), 60, 6, Xamarin.ExposureNotifications.RiskLevel.Highest),
#endif
			};

        public void Dispose()
            => MessagingCenter.Instance.Unsubscribe<ExposureNotificationHandler>(this, "exposure_info_changed");

    }
}
