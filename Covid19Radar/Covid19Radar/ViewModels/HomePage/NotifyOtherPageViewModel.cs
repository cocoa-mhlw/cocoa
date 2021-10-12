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
using System.IO;
using System.Collections.Generic;
using Chino;
using System.Net;
using System.Linq;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase, IExposureNotificationEventCallback
    {
        private readonly ILoggerService loggerService;

        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly IDiagnosisKeyRegisterServer diagnosisKeyRegisterServer;
        private readonly ICloseApplicationService closeApplicationService;

        private string _processNumber;
        public string ProcessNumber
        {
            get { return _processNumber; }
            set
            {
                SetProperty(ref _processNumber, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private bool _isConsentLinkVisible;
        public bool IsConsentLinkVisible
        {
            get { return _isConsentLinkVisible; }
            set { SetProperty(ref _isConsentLinkVisible, value); }
        }

        private bool _isProcessNumberEnabled;
        public bool IsProcessNumberEnabled
        {
            get { return _isProcessNumberEnabled; }
            set { SetProperty(ref _isProcessNumberEnabled, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }
        private bool _isVisibleWithSymptomsLayout;
        public bool IsVisibleWithSymptomsLayout
        {
            get { return _isVisibleWithSymptomsLayout; }
            set
            {
                SetProperty(ref _isVisibleWithSymptomsLayout, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private bool _isVisibleNoSymptomsLayout;
        public bool IsVisibleNoSymptomsLayout
        {
            get { return _isVisibleNoSymptomsLayout; }
            set
            {
                SetProperty(ref _isVisibleNoSymptomsLayout, value);
                IsEnabled = CheckRegisterButtonEnable();
            }
        }
        private DateTime _diagnosisDate;
        public DateTime DiagnosisDate
        {
            get { return _diagnosisDate; }
            set { SetProperty(ref _diagnosisDate, value); }
        }
        private int errorCount { get; set; }

        public NotifyOtherPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IDiagnosisKeyRegisterServer diagnosisKeyRegisterServer,
            ICloseApplicationService closeApplicationService
            ) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;

            this.loggerService = loggerService;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.diagnosisKeyRegisterServer = diagnosisKeyRegisterServer;
            this.closeApplicationService = closeApplicationService;
            errorCount = 0;
            ProcessNumber = "";
            DiagnosisDate = DateTime.Today;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters != null && parameters.ContainsKey(NotifyOtherPage.ProcessNumberKey))
            {
                ProcessNumber = parameters.GetValue<string>(NotifyOtherPage.ProcessNumberKey);
                IsProcessNumberEnabled = false;
                IsConsentLinkVisible = true;
            }
        }

        public Command OnShowConsentPageClicked => new Command(async () =>
        {
            loggerService.StartMethod();

            var param = new NavigationParameters();
            param = SubmitConsentPage.CreateNavigationParams(isFromAppLinks: true, param);
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
                    "",
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
                        AppResources.NotifyOtherPageDiagAppClose,
                        AppResources.NotifyOtherPageDiagAppCloseTitle,
                        AppResources.ButtonOk
                    );
                    UserDialogs.Instance.HideLoading();
                    closeApplicationService.CloseApplication();

                    loggerService.Error($"Exceeded the number of trials.");
                    loggerService.EndMethod();
                    return;
                }

                loggerService.Info($"Number of attempts to submit diagnostic number. ({errorCount + 1} of {AppConstants.MaxErrorCount})");

                if (errorCount > 0)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.NotifyOtherPageDiag3Message,
                        AppResources.NotifyOtherPageDiag3Title,
                        AppResources.ButtonOk
                        );
                    await Task.Delay(errorCount * 5000);
                }

                loggerService.Info($"Number of attempts to submit diagnostic number. ({errorCount + 1} of {AppConstants.MaxErrorCount})");

                if (errorCount > 0)


                    // Init Dialog
                    if (string.IsNullOrEmpty(_processNumber))
                    {
                        await UserDialogs.Instance.AlertAsync(
                            AppResources.NotifyOtherPageDiag4Message,
                            AppResources.ProcessingNumberErrorDiagTitle,
                            AppResources.ButtonOk
                        );
                        errorCount++;
                        loggerService.Error($"No diagnostic number entered.");
                        return;
                    }

                Regex regex = new Regex(AppConstants.processNumberRegex);
                if (!regex.IsMatch(_processNumber))
                {
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.NotifyOtherPageDiag5Message,
                        AppResources.ProcessingNumberErrorDiagTitle,
                        AppResources.ButtonOk
                    );
                    errorCount++;
                    loggerService.Error($"Incorrect process number format.");
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
                    UserDialogs.Instance.HideLoading();
                    await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

                    loggerService.Warning($"Exposure notification is disable.");
                    return;
                }

                UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);

                await SubmitDiagnosisKeys();

                UserDialogs.Instance.HideLoading();
            }
            catch (InvalidDataException ex)
            {
                UserDialogs.Instance.HideLoading();

                errorCount++;
                UserDialogs.Instance.Alert(
                    AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFound,
                    AppResources.NotifyOtherPageDialogExceptionTargetDiagKeyNotFoundTitle,
                    AppResources.ButtonOk
                );
                loggerService.Exception("Failed to submit DiagnosisKeys invalid data.", ex);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                errorCount++;
                UserDialogs.Instance.Alert(
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
        }));

        private async Task SubmitDiagnosisKeys()
        {
            loggerService.Info($"Submit DiagnosisKeys.");

            try
            {
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

                // TODO: Save and use revoke operation.
                string idempotencyKey = Guid.NewGuid().ToString();

                IList<HttpStatusCode> httpStatusCodes = await diagnosisKeyRegisterServer.SubmitDiagnosisKeysAsync(
                    _diagnosisDate,
                    filteredTemporaryExposureKeyList,
                    _processNumber,
                    idempotencyKey
                    );

                foreach (var statusCode in httpStatusCodes)
                {
                    loggerService.Info($"HTTP status is {httpStatusCodes}({(int)statusCode}).");

                    // Mainly, we expect that SubmitDiagnosisKeysAsync returns one result.
                    // Multiple-results is for debug use only.
                    ShowResult(statusCode);
                }
            }
            catch (ENException exception)
            {
                loggerService.Exception("GetTemporaryExposureKeyHistoryAsync", exception);
            }
            catch (Exception exception)
            {
                loggerService.Exception("SubmitDiagnosisKeys", exception);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void ShowResult(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                    // Success
                    loggerService.Info($"Successfully submit DiagnosisKeys.");

                    await UserDialogs.Instance.AlertAsync(
                        "",
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
                        "",
                        AppResources.ExposureNotificationHandler2ErrorMessage,
                        AppResources.ButtonOk);
                    loggerService.Error($"Cannot connect to the server.");
                    break;

                case HttpStatusCode.BadRequest:
                    await UserDialogs.Instance.AlertAsync(
                        "",
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
            return ProcessNumber.Length == AppConstants.MaxProcessNumberLength && (IsVisibleWithSymptomsLayout || IsVisibleNoSymptomsLayout);
        }

        public async void OnGetTekHistoryAllowed()
        {
            loggerService.StartMethod();

            await SubmitDiagnosisKeys();

            loggerService.EndMethod();
        }
    }
}
