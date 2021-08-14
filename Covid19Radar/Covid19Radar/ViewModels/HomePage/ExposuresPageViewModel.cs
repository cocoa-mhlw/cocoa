/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
            _userDataRepository = userDataRepository;

            Title = Resources.AppResources.MainExposures;
            _exposures = new ObservableCollection<ExposureSummary>();

        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            var (userExposureSummary, userExposureInformationList)
                = await _userDataRepository.GetUserExposureDataAsync(AppConstants.DaysOfExposureInformationToDisplay);

            if (userExposureInformationList.Count() > 0)
            {
                foreach (var en in userExposureInformationList.GroupBy(userExposureInformation => userExposureInformation.Timestamp))
                {
                    var ens = new ExposureSummary()
                    {
                        ExposureDate = en.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                        ExposureCount = en.Count().ToString()
                    };
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
