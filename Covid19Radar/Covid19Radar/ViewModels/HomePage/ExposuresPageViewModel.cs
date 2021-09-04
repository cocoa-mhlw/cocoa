/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Resources;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly IUserDataRepository _userDataRepository;

        public ObservableCollection<ExposureSummary> _exposures;
        public ObservableCollection<ExposureSummary> Exposures
        {
            get { return _exposures; }
            set { SetProperty(ref _exposures, value); }
        }

        public ExposuresPageViewModel(
            INavigationService navigationService,
            IUserDataRepository userDataRepository
            ) : base(navigationService)
        {
            Title = AppResources.MainExposures;
            _userDataRepository = userDataRepository;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _exposures = new ObservableCollection<ExposureSummary>();

            var exposureInformationList = _userDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);
            if (exposureInformationList != null)
            {
                foreach (var en in exposureInformationList.GroupBy(eni => eni.Timestamp))
                {
                    var ens = new ExposureSummary();
                    ens.ExposureDate = en.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
                    ens.SetExposureCount(en.Count());
                    _exposures.Add(ens);
                }
            }
        }
    }

    public class ExposureSummary
    {
        public string ExposureDate { get; set; }

        private string _exposurePluralizeCount;

        public string ExposurePluralizeCount => _exposurePluralizeCount;

        public void SetExposureCount(int value) {
            _exposurePluralizeCount = PluralizeCount(value);
        }

        private static string PluralizeCount(int count)
        {
            return count switch
            {
                1 => AppResources.ExposuresPageExposureUnitPluralOnce,
                _ => string.Format(AppResources.ExposuresPageExposureUnitPlural, count),
            };
        }
    }
}
