using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    [QueryProperty(nameof(ExposureInfoParameter), "info")]
    public class ExposureDetailPageViewModel : ViewModelBase
    {
        public ExposureDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.MainExposures;
        }

        public string ExposureInfoParameter
        {
            set
            {
                var json = Uri.UnescapeDataString(value ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    ExposureInfo = JsonConvert.DeserializeObject<ExposureInfo>(json);
                    OnPropertyChanged(nameof(ExposureInfo));
                }
            }
        }

        public Command CancelCommand
            => new Command(() => {
                // Cancel
            });

        public ExposureInfo ExposureInfo { get; set; } = new ExposureInfo(DateTime.Now, TimeSpan.FromMinutes(30), 70, 6, RiskLevel.High);

        public DateTime When
            => ExposureInfo.Timestamp;

        public TimeSpan Duration
            => ExposureInfo.Duration;

    }
}
