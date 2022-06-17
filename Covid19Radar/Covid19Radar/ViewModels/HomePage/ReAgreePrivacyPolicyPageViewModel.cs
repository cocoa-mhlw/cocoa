/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ReAgreePrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private DateTime UpdateDateTimeUtc { get; set; }
        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        private INavigationParameters _navigationParameters;

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;

        public ReAgreePrivacyPolicyPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ISendEventLogStateRepository sendEventLogStateRepository
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
        }

        public Command OpenWebView => new Command(async () =>
        {
            _loggerService.StartMethod();

            var url = Resources.AppResources.UrlPrivacyPolicy;
            await BrowserOpenAsync(url, BrowserLaunchMode.SystemPreferred);

            _loggerService.EndMethod();
        });

        public Command OnClickReAgreeCommand => new Command(async () =>
        {
            _loggerService.StartMethod();

            _userDataRepository.SaveLastUpdateDate(TermsType.PrivacyPolicy, UpdateDateTimeUtc);

            Destination destination = Destination.HomePage;
            if (_navigationParameters.ContainsKey(ReAgreePrivacyPolicyPage.DestinationKey))
            {
                destination = _navigationParameters.GetValue<Destination>(ReAgreePrivacyPolicyPage.DestinationKey);
            }

            if (ISendEventLogStateRepository.IsExistNotSetEventType(_sendEventLogStateRepository))
            {
                _loggerService.Info($"Transition to SendLogSettingsPage");

                var navigationParams = SendLogSettingsPage.BuildNavigationParams(destination, _navigationParameters);
                _ = await NavigationService.NavigateAsync(Destination.SendLogSettingsPage.ToPath(), navigationParams);
            }
            else
            {
                _ = await NavigationService.NavigateAsync(destination.ToPath(), _navigationParameters);
            }

            _loggerService.EndMethod();
        });

        public override void Initialize(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.Initialize(parameters);
            TermsUpdateInfoModel.Detail updateInfo = (TermsUpdateInfoModel.Detail) parameters[ReAgreePrivacyPolicyPage.UpdatePrivacyPolicyInfoKey];
            UpdateDateTimeUtc = updateInfo.UpdateDateTimeUtc;
            UpdateText = updateInfo.Text;

            _navigationParameters = parameters;

            _loggerService.EndMethod();
        }
    }
}
