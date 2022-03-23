/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Covid19Radar.Views;
using System.Text.RegularExpressions;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chino;
using System.Net;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase, IExposureNotificationEventCallback
    {
        private const ReportType DEFAULT_REPORT_TYPE = ReportType.ConfirmedTest;

        public string RadioButtonYesReadText => Device.RuntimePlatform == Device.iOS ? $"{AppResources.NotifyOtherPageRadioButtonYes} {AppResources.Button}" : "";
        public string RadioButtonNoReadText => Device.RuntimePlatform == Device.iOS ? $"{AppResources.NotifyOtherPageRadioButtonNo} {AppResources.Button}" : "";
        public string HowToReceiveProcessingNumberReadText => $"{AppResources.NotifyOtherPageLabel} {AppResources.Button}";

        private readonly ILoggerService loggerService;

        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly IDiagnosisKeyRegisterServer diagnosisKeyRegisterServer;
        private readonly IEssentialsService _essentialsService;
        private readonly int _delayForErrorMillis;

        private string _processingNumber;
        public string ProcessingNumber
        {
            get { return _processingNumber; }
            set
            {
                SetProperty(ref _processingNumber, value);
                IsNextButtonEnabled = CheckRegisterButtonEnable();
            }
        }

        private bool _isDeepLink = false;
        public bool IsDeepLink
        {
            get => _isDeepLink;
            set => SetProperty(ref _isDeepLink, value);
        }

        private bool _isConsentLinkVisible;
        public bool IsConsentLinkVisible
        {
            get => _isConsentLinkVisible;
            set => SetProperty(ref _isConsentLinkVisible, value);
        }

        private bool _isProcessingNumberReadOnly = false;
        public bool IsProcessingNumberReadOnly
        {
            get => _isProcessingNumberReadOnly;
            set => SetProperty(ref _isProcessingNumberReadOnly, value);
        }

        private bool _isHowToObtainProcessingNumberVisible = true;
        public bool IsHowToObtainProcessingNumberVisible
        {
            get => _isHowToObtainProcessingNumberVisible;
            set => SetProperty(ref _isHowToObtainProcessingNumberVisible, value);
        }

        public string InqueryTelephoneNumber => AppResources.InquiryAboutRegistrationPhoneNumber;

        private bool _isInqueryTelephoneNumberVisible;
        public bool IsInqueryTelephoneNumberVisible
        {
            get => _isInqueryTelephoneNumberVisible;
            set => SetProperty(ref _isInqueryTelephoneNumberVisible, value);
        }

        private bool _isNextButtonEnabled;
        public bool IsNextButtonEnabled
        {
            get => _isNextButtonEnabled;
            set => SetProperty(ref _isNextButtonEnabled, value);
        }

        private bool _isVisibleWithSymptomsLayout;
        public bool IsVisibleWithSymptomsLayout
        {
            get => _isVisibleWithSymptomsLayout;
            set
            {
                SetProperty(ref _isVisibleWithSymptomsLayout, value);
                IsNextButtonEnabled = CheckRegisterButtonEnable();
            }
        }

        private bool _isVisibleNoSymptomsLayout;
        public bool IsVisibleNoSymptomsLayout
        {
            get => _isVisibleNoSymptomsLayout;
            set
            {
                SetProperty(ref _isVisibleNoSymptomsLayout, value);
                IsNextButtonEnabled = CheckRegisterButtonEnable();
            }
        }

        private DateTime _diagnosisDate;

        public DateTime DiagnosisDate
        {
            get => _diagnosisDate;
            set => SetProperty(ref _diagnosisDate, value);
        }

        private int errorCount { get; set; }

        // TODO: Save and use for revoke operation.
        private string idempotencyKey = Guid.NewGuid().ToString();

        public NotifyOtherPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IDiagnosisKeyRegisterServer diagnosisKeyRegisterServer,
            IEssentialsService essentialsService,
            int delayForErrorMillis = AppConstants.DelayForRegistrationErrorMillis
            ) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;

            this.loggerService = loggerService;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.diagnosisKeyRegisterServer = diagnosisKeyRegisterServer;
            _delayForErrorMillis = delayForErrorMillis;
            _essentialsService = essentialsService;
            errorCount = 0;
            ProcessingNumber = "";
            DiagnosisDate = DateTime.Today;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters != null && parameters.ContainsKey(NotifyOtherPage.ProcessingNumberKey))
            {
                ProcessingNumber = parameters.GetValue<string>(NotifyOtherPage.ProcessingNumberKey);
                IsDeepLink = true;
                IsHowToObtainProcessingNumberVisible = false;
                IsProcessingNumberReadOnly = true;
                IsConsentLinkVisible = true;
                IsInqueryTelephoneNumberVisible = true;
            }
        }

        public Command OnInqueryTelephoneNumberClicked => new Command(() =>
        {
            loggerService.StartMethod();

            try
            {
                var phoneNumber = Regex.Replace(InqueryTelephoneNumber, "[^0-9]", "");
                _essentialsService.PhoneDialerOpen(phoneNumber);
            }
            catch (Exception exception)
            {
                loggerService.Exception("Exception occurred: PhoneDialer", exception);
            }

            loggerService.EndMethod();
        });

        public Command OnShowConsentPageClicked => new Command(async () =>
        {
            loggerService.StartMethod();

            INavigationParameters param = new NavigationParameters();
            param = SubmitConsentPage.BuildNavigationParams(true, ProcessingNumber, param);

            var result = await NavigationService.NavigateAsync("SubmitConsentPage", param);

            loggerService.EndMethod();
        });

        public Command OnClickRegister => (new Command(async () =>
        {
            loggerService.StartMethod();


            var result = await UserDialogs.Instance.ConfirmAsync(
                AppResources.NotifyOtherPageDiag1Message,
                AppResources.NotifyOtherPageDiag1Title,
                AppResources.ButtonRegister,
                AppResources.ButtonCancel);
            if (!result)
            {
                await UserDialogs.Instance.AlertAsync(
                    null,
                    AppResources.NotifyOtherPageDiag2Title,
                    AppResources.ButtonOk
                    );

                loggerService.Info($"Canceled by user.");
                loggerService.EndMethod();
                return;
            }

            // Check helthcare authority positive api check here!!
            try
            {
                if (errorCount >= AppConstants.MaxErrorCount)
                {
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.NotifyOtherPageDiagReturnHome,
                        AppResources.NotifyOtherPageDiagReturnHomeTitle,
                        AppResources.ButtonOk
                    );
                    await NavigationService.NavigateAsync(Destination.HomePage.ToPath());

                    loggerService.Error($"Exceeded the number of trials.");
                    return;
                }

                loggerService.Info($"Number of attempts to submit diagnostic number. ({errorCount + 1} of {AppConstants.MaxErrorCount})");

                // UserDialogs.Instance.Loading must be executed in MainThread.
                UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);

                if (errorCount > 0)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.NotifyOtherPageDiag3Message,
                        AppResources.NotifyOtherPageDiag3Title,
                        AppResources.ButtonOk
                        );
                    await Task.Delay(errorCount * _delayForErrorMillis);
                }

                loggerService.Info($"Number of attempts to submit diagnostic number. ({errorCount + 1} of {AppConstants.MaxErrorCount})");

                // Init Dialog
                if (string.IsNullOrEmpty(ProcessingNumber))
                {
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.NotifyOtherPageDiag4Message,
                        AppResources.ProcessingNumberErrorDiagTitle,
                        AppResources.ButtonOk
                    );
                    errorCount++;
                    loggerService.Error($"No diagnostic number entered.");
                    UserDialogs.Instance.HideLoading();
                    return;
                }

                if (!Validator.IsValidProcessingNumber(ProcessingNumber))
                {
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.NotifyOtherPageDiag5Message,
                        AppResources.ProcessingNumberErrorDiagTitle,
                        AppResources.ButtonOk
                    );
                    errorCount++;
                    loggerService.Error($"Incorrect process number format.");
                    UserDialogs.Instance.HideLoading();
                    return;
                }

                // EN Enabled Check
                var enabled = await exposureNotificationApiService.IsEnabledAsync();

                if (!enabled)
                {
                    await UserDialogs.Instance.AlertAsync(
                       AppResources.NotifyOtherPageDiag6Message,
                       AppResources.NotifyOtherPageDiag6Title,
                       AppResources.ButtonOk
                    );
                    await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

                    loggerService.Warning($"Exposure notification is disable.");
                    UserDialogs.Instance.HideLoading();
                    return;
                }

                HttpStatusCode httpResult = await SubmitDiagnosisKeys();

                UserDialogs.Instance.HideLoading();

                ShowResult(httpResult);

                if (httpResult != HttpStatusCode.OK)
                {
                    errorCount++;
                }
            }
            catch (ENException exception)
            {
                loggerService.Exception("GetTemporaryExposureKeyHistoryAsync", exception);

                if (exception.Code == ENException.Code_iOS.NotAuthorized)
                {
                    loggerService.Info("GetTekHistory request is declined by user.");

                    UserDialogs.Instance.HideLoading();

                    await UserDialogs.Instance.AlertAsync(
                        null,
                        AppResources.NotifyOtherPageDiag2Title,
                        AppResources.ButtonOk
                        );
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                }

            }
            catch (Exception ex)
            {
                errorCount++;

                UserDialogs.Instance.HideLoading();

                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOther_Dialog_NoConnection,
                    AppResources.NotifyOtherPageDialogExceptionTitle,
                    AppResources.ButtonOk
                    );

                loggerService.Exception("Failed to submit DiagnosisKeys.", ex);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }));

        private async Task<HttpStatusCode> SubmitDiagnosisKeys()
        {
            loggerService.Info($"Submit DiagnosisKeys.");

            List<TemporaryExposureKey> temporaryExposureKeyList
                = await exposureNotificationApiService.GetTemporaryExposureKeyHistoryAsync();

            loggerService.Info($"TemporaryExposureKeys-count: {temporaryExposureKeyList.Count()}");

            IList<TemporaryExposureKey> filteredTemporaryExposureKeyList
                = TemporaryExposureKeyUtils.FiilterTemporaryExposureKeys(
                    temporaryExposureKeyList,
                    _diagnosisDate,
                    AppConstants.DaysToSendTek,
                    loggerService
                    );

            loggerService.Info($"FilteredTemporaryExposureKeys-count: {filteredTemporaryExposureKeyList.Count()}");

            // Set reportType
            foreach (var tek in filteredTemporaryExposureKeyList)
            {
                tek.ReportType = DEFAULT_REPORT_TYPE;
            }

            return await diagnosisKeyRegisterServer.SubmitDiagnosisKeysAsync(
                _diagnosisDate,
                filteredTemporaryExposureKeyList,
                ProcessingNumber,
                idempotencyKey
                );
        }

        private async void ShowResult(HttpStatusCode httpStatusCode)
        {
            loggerService.Info($"HTTP status is {httpStatusCode}({(int)httpStatusCode}).");

            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                    // Success
                    loggerService.Info($"Successfully submit DiagnosisKeys.");

                    await UserDialogs.Instance.AlertAsync(
                        null,
                        AppResources.NotifyOtherPageDialogSubmittedTitle,
                        AppResources.ButtonOk
                    );
                    await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                    break;

                case HttpStatusCode.NotAcceptable:
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.ExposureNotificationHandler1ErrorMessage,
                        AppResources.ProcessingNumberErrorDiagTitle,
                        AppResources.ButtonOk);
                    loggerService.Error($"The process number is incorrect.");
                    break;

                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.ServiceUnavailable:
                    await UserDialogs.Instance.AlertAsync(
                        null,
                        AppResources.ExposureNotificationHandler2ErrorMessage,
                        AppResources.ButtonOk);
                    loggerService.Error($"Cannot connect to the server.");
                    break;

                case HttpStatusCode.BadRequest:
                    await UserDialogs.Instance.AlertAsync(
                        null,
                        AppResources.ExposureNotificationHandler3ErrorMessage,
                        AppResources.ButtonOk);
                    loggerService.Error($"There is a problem with the record data.");
                    break;

                default:
                    loggerService.Error($"Unexpected status");
                    break;
            }
        }

        public void OnClickRadioButtonIsTrueCommand(string text)
        {
            loggerService.StartMethod();

            if (AppResources.NotifyOtherPageRadioButtonYes.Equals(text))
            {
                IsVisibleWithSymptomsLayout = true;
                IsVisibleNoSymptomsLayout = false;
            }
            else if (AppResources.NotifyOtherPageRadioButtonNo.Equals(text))
            {
                IsVisibleWithSymptomsLayout = false;
                IsVisibleNoSymptomsLayout = true;
            }
            else
            {
                IsVisibleWithSymptomsLayout = false;
                IsVisibleNoSymptomsLayout = false;
            }

            loggerService.Info($"Is visible with symptoms layout: {IsVisibleWithSymptomsLayout}, Is visible no symptoms layout: {IsVisibleNoSymptomsLayout}");
            loggerService.EndMethod();
        }

        public bool CheckRegisterButtonEnable()
        {
            return ProcessingNumber.Length == AppConstants.MaxProcessingNumberLength && (IsVisibleWithSymptomsLayout || IsVisibleNoSymptomsLayout);
        }

        public async void OnGetTekHistoryAllowed()
        {
            loggerService.StartMethod();

            try
            {
                using (UserDialogs.Instance.Loading(AppResources.LoadingTextRegistering))
                {
                    HttpStatusCode httpResult = await SubmitDiagnosisKeys();

                    ShowResult(httpResult);

                    if (httpResult != HttpStatusCode.OK)
                    {
                        errorCount++;
                    }
                }
            }
            catch (ENException exception)
            {
                loggerService.Exception("GetTemporaryExposureKeyHistoryAsync", exception);
            }
            catch (Exception ex)
            {
                errorCount++;

                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDialogExceptionText,
                    AppResources.NotifyOtherPageDialogExceptionTitle,
                    AppResources.ButtonOk
                );
                loggerService.Exception("Failed to submit DiagnosisKeys.", ex);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public async void OnGetTekHistoryDecline()
        {
            loggerService.StartMethod();

            loggerService.Info("GetTekHistory request is declined by user.");
            await UserDialogs.Instance.AlertAsync(
                null,
                AppResources.NotifyOtherPageDiag2Title,
                AppResources.ButtonOk
                );

            loggerService.EndMethod();
        }

    }
}
