using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
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
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public ExposuresPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.MainExposures;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }


        public ObservableCollection<ExposureInfo> ExposureInformation
            => userData.ExposureInformation;
/*
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
*/
    }
}
