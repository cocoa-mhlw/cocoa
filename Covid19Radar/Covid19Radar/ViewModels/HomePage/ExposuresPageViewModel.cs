/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
            _userDataRepository = userDataRepository;

            Title = AppResources.MainExposures;
            _exposures = new ObservableCollection<ExposureSummary>();

        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await InitExposures();
        }

        public async Task InitExposures()
        {
            var exposureWindowList
                = await _userDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

            var userExposureInformationList
                = _userDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            if (exposureWindowList.Count() > 0)
            {
                foreach (var ew in exposureWindowList.GroupBy(exposureWindow => exposureWindow.GetDateTime()))
                {
                    var ens = new ExposureSummary()
                    {
                        Timestamp = ew.Key,
                        ExposureDate = ew.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                    };
                    ens.SetExposureCount(ew.Count());
                    _exposures.Add(ens);
                }
            }

            if (userExposureInformationList.Count() > 0)
            {
                foreach (var ei in userExposureInformationList.GroupBy(userExposureInformation => userExposureInformation.Timestamp))
                {
                    var ens = new ExposureSummary()
                    {
                        Timestamp = ei.Key,
                        ExposureDate = ei.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                    };
                    ens.SetExposureCount(ei.Count());
                    _exposures.Add(ens);
                }
            }

            Exposures = new ObservableCollection<ExposureSummary>(
                _exposures.OrderByDescending(exposureSummary => exposureSummary.Timestamp)
                );

        }
    }


    public class ExposureSummary
    {
        public DateTime Timestamp { get; set; }

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
