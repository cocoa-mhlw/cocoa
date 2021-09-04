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

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILoggerService _loggerService;

        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        public ContactedNotifyPageViewModel(
            INavigationService navigationService,
            IUserDataRepository userDataRepository,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            _userDataRepository = userDataRepository;
            _loggerService = loggerService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            ExposureCount = _userDataRepository.GetExposureCount(AppConstants.DaysOfExposureInformationToDisplay).ToString();
        }

        public Command OnClickByForm => new Command(async () =>
        {
            _loggerService.StartMethod();

            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            _loggerService.EndMethod();
        });
    }
}
