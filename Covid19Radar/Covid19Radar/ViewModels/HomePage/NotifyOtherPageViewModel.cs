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

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public string HowToReceiveProcessingNumberReadText => $"{AppResources.NotifyOtherPageLabel} {AppResources.Button}";

        private readonly ILoggerService loggerService;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly ICloseApplicationService closeApplicationService;
        private readonly IEssentialsService _essentialsService;

        private string _diagnosisUid;
        public string DiagnosisUid
        {
            get => _diagnosisUid;
            set
            {
                SetProperty(ref _diagnosisUid, value);
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

        public NotifyOtherPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IExposureNotificationService exposureNotificationService,
            ICloseApplicationService closeApplicationService,
            IEssentialsService essentialsService
            ) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
            this.exposureNotificationService = exposureNotificationService;
            this.closeApplicationService = closeApplicationService;
            _essentialsService = essentialsService;
            errorCount = 0;
            DiagnosisUid = "";
            DiagnosisDate = DateTime.Today;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters != null && parameters.ContainsKey(NotifyOtherPage.ProcessingNumberKey))
            {
                DiagnosisUid = parameters.GetValue<string>(NotifyOtherPage.ProcessingNumberKey);
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

            var param = new NavigationParameters();
            param = SubmitConsentPage.BuildNavigationParams(true, _diagnosisUid, param);
            var result = await NavigationService.NavigateAsync("SubmitConsentPage", param);

            loggerService.EndMethod();
        });

        public Command OnClickRegister => (new Command(async () =>
        {
            loggerService.StartMethod();

            var result = await UserDialogs.Instance.ConfirmAsync(AppResources.NotifyOtherPageDiag1Message, AppResources.NotifyOtherPageDiag1Title, AppResources.ButtonRegister, AppResources.ButtonCancel);
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

            UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);

            // Check helthcare authority positive api check here!!
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


            // Init Dialog
            if (string.IsNullOrEmpty(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag4Message,
                    AppResources.ProcessingNumberErrorDiagTitle,
                    AppResources.ButtonOk
                );
                errorCount++;
                UserDialogs.Instance.HideLoading();

                loggerService.Error($"No diagnostic number entered.");
                loggerService.EndMethod();
                return;
            }

            Regex regex = new Regex(AppConstants.positiveRegex);
            if (!regex.IsMatch(_diagnosisUid))
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.NotifyOtherPageDiag5Message,
                    AppResources.ProcessingNumberErrorDiagTitle,
                    AppResources.ButtonOk
                );
                errorCount++;
                UserDialogs.Instance.HideLoading();

                loggerService.Error($"Incorrect diagnostic number format.");
                loggerService.EndMethod();
                return;
            }

            // Submit the UID
            try
            {
                // EN Enabled Check
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

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
                    loggerService.EndMethod();
                    return;
                }

                loggerService.Info($"Submit the processing number.");

                // Submit our diagnosis
                exposureNotificationService.PositiveDiagnosis = _diagnosisUid;
                exposureNotificationService.DiagnosisDate = DiagnosisDate;
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.NotifyOtherPageDialogSubmittedTitle,
                    AppResources.ButtonOk
                );
                await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

                loggerService.Info($"Successfully submit the diagnostic number.");
                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                errorCount++;
                UserDialogs.Instance.Alert(
                    AppResources.NotifyOtherPageDialogExceptionText,
                    AppResources.NotifyOtherPageDialogExceptionTitle,
                    AppResources.ButtonOk
                );
                loggerService.Exception("Failed to submit UID.", ex);
                loggerService.EndMethod();
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }));

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
            return DiagnosisUid.Length == AppConstants.MaxDiagnosisUidCount && (IsVisibleWithSymptomsLayout || IsVisibleNoSymptomsLayout);
        }
    }
}
