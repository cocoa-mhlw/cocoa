/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;
using Covid19Radar.Repository;
using Covid19Radar.Common;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IExposureDataRepository _exposureDataRepository;

        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        public ContactedNotifyPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IExposureDataRepository exposureDataRepository
            ) : base(navigationService)
        {
            this.loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;

            Title = AppResources.TitileUserStatusSettings;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            var dailySummaryList = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
            var userExposureInformationList = _exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);
            ExposureCount = (dailySummaryList.Count() + userExposureInformationList.Count()).ToString();
        }

        public Command OnClickByForm => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });
    }
}
