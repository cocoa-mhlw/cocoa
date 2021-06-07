﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Covid19Radar.Services;
using Prism.Navigation;

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
            Title = Resources.AppResources.MainExposures;
            _exposures = new ObservableCollection<ExposureSummary>();

            var exposureInformationList = exposureNotificationService.GetExposureInformationListToDisplay();
            if (exposureInformationList != null)
            {
                foreach (var en in exposureInformationList.GroupBy(eni => eni.Timestamp))
                {
                    var ens = new ExposureSummary();
                    ens.ExposureDate = en.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
                    ens.ExposureCount = en.Count().ToString();
                    _exposures.Add(ens);
                }
            }
        }
    }

    public class ExposureSummary
    {
        public string ExposureDate { get; set; }
        public string ExposureCount { get; set; }
    }
}
