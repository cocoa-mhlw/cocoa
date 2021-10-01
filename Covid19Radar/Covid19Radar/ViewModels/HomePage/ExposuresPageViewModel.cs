/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        public ObservableCollection<ExposureSummary> _exposures;
        public ObservableCollection<ExposureSummary> Exposures
        {
            get { return _exposures; }
            set { SetProperty(ref _exposures, value); }
        }

        public ExposuresPageViewModel(INavigationService navigationService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = AppResources.MainExposures;
            _exposures = new ObservableCollection<ExposureSummary>();

            var exposureInformationList = exposureNotificationService.GetExposureInformationListToDisplay();
            if (exposureInformationList != null)
            {
                foreach (var en in exposureInformationList.GroupBy(eni => eni.Timestamp))
                {
                    var ens = new ExposureSummary();
                    ens.ExposureDate = en.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
                    ens.ExposureCountInt = en.Count();
                    _exposures.Add(ens);
                }
            }
        }
    }

    public class ExposureSummary
    {
        public string ExposureDate { get; set; }

        private string _exposureCount;

        public string ExposureCount { get => _exposureCount; }
        public int ExposureCountInt { set => _exposureCount = PluralizeCount(value); }

        private string PluralizeCount(int count)
        {
            return count switch
            {
                1 => AppResources.ExposuresPageExposureUnitPluralOnce,
                _ => string.Format(AppResources.ExposuresPageExposureUnitPlural, count),
            };
        }
    }
}
