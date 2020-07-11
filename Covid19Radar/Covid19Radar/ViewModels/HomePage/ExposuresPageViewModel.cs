using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;
        public ObservableCollection<ExposureSummary> _exposures;
        public ObservableCollection<ExposureSummary> Exposures
        {
            get { return _exposures; }
            set { SetProperty(ref _exposures, value); }
        }

        public ExposuresPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.MainExposures;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            _exposures = new ObservableCollection<ExposureSummary>();

            foreach (var en in userData.ExposureInformation.GroupBy(eni => eni.Timestamp))
            {
                var ens = new ExposureSummary();
                ens.ExposureDate =  en.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
                ens.ExposureCount = en.Count().ToString();
                _exposures.Add(ens);
            }
        }
    }

    public class ExposureSummary
    {
        public string ExposureDate { get; set; }
        public string ExposureCount { get; set; }

    }
}
