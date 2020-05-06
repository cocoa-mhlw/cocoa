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
    public class ExposuresPageViewModel : ViewModelBase
    {
        public ExposuresPageViewModel()
        {
            Title = Resources.AppResources.MainExposures;
            //Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync()
            //	.ContinueWith(t =>
            //	{
            //		IsEnabled = t.Result;
            //	});

        }

        public bool IsEnabled
        {
            get => LocalStateManager.Instance.LastIsEnabled;
            set
            {
                LocalStateManager.Instance.LastIsEnabled = value;
                LocalStateManager.Save();
                NotifyPropertyChanged(nameof(IsEnabled));
            }
        }

        public bool IsWelcomed
        {
            get => LocalStateManager.Instance.IsWelcomed;
            set
            {
                LocalStateManager.Instance.IsWelcomed = value;
                LocalStateManager.Save();
                NotifyPropertyChanged(nameof(IsWelcomed));
            }
        }

        public Command EnableDisableCommand
            => new Command(async () =>
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (enabled)
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                else
                    await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
            });

        public Command GetStartedCommand
            => new Command(() => IsWelcomed = true);

        public Command NotNowCommand
            => new Command(() => IsWelcomed = false);
    }
}
