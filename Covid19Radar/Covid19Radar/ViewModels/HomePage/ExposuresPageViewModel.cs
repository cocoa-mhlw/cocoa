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
        public ExposuresPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.MainExposures;
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
            => LocalStateManager.Instance.ExposureInformation;

        /*
        public Command<ExposureInfo> ExposureSelectedCommand => new Command<ExposureInfo>(
            (info) =>GoToAsync($"{nameof(ExposureDetailsPage)}?info={JsonConvert.SerializeObject(info)}"));
        */

    }
}
