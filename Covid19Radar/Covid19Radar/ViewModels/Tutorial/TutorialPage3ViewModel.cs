/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage3ViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataService _userDataService;
        private readonly IUserDataRepository _userDataRepository;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public TutorialPage3ViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataService userDataService,
            IUserDataRepository userDataRepository
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _userDataService = userDataService;
            _userDataRepository = userDataRepository;
        }
        public Command OnClickAgree => new Command(async () =>
        {
            _loggerService.StartMethod();

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);

            var resultStatusCode = await _userDataService.RegisterUserAsync();
            if (resultStatusCode != HttpStatusCode.OK)
            {
                UserDialogs.Instance.HideLoading();
                if (resultStatusCode == HttpStatusCode.Forbidden)
                {
                    _loggerService.Error("Failed register for requests from overseas");
                    await UserDialogs.Instance.AlertAsync(
                        Resources.AppResources.DialogNetworkConnectionErrorFromOverseasMessage,
                        Resources.AppResources.DialogNetworkConnectionErrorTitle,
                        Resources.AppResources.ButtonOk);
                    return;
                }

                _loggerService.Error("Failed register");
                await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, Resources.AppResources.DialogNetworkConnectionErrorTitle, Resources.AppResources.ButtonOk);
                _loggerService.EndMethod();
                return;
            }

            _userDataRepository.SaveLastUpdateDate(TermsType.TermsOfService, DateTime.Now);
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
            _loggerService.EndMethod();
        });
    }
}
