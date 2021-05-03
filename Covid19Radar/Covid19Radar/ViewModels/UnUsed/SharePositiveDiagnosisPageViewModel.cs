﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SharePositiveDiagnosisPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; } = true;
        public string DiagnosisUid { get; set; }
        public DateTime? DiagnosisTimestamp { get; set; } = DateTime.Now;

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public SharePositiveDiagnosisPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileSharePositiveDiagnosis;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        public Command CancelCommand => (new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
        }));


        public Command SubmitAndVerifyCommand => (new Command(async () =>
        {

            // Verify  Check the parameters

            if (string.IsNullOrEmpty(DiagnosisUid))
            {
                // Check gov's positive api check here!!

                await UserDialogs.Instance.AlertAsync("Please provide a valid Diagnosis ID", "Invalid Diagnosis ID", "OK");
                return;
            }
            if (!DiagnosisTimestamp.HasValue || DiagnosisTimestamp.Value > DateTime.Now)
            {
                await UserDialogs.Instance.AlertAsync("Please provide a valid Test Date", "Invalid Test Date", "OK");
                return;
            }

            // Submit the UID
            using var dialog = UserDialogs.Instance.Loading("Submitting Diagnosis...");
            IsEnabled = false;
            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    dialog.Hide();

                    await UserDialogs.Instance.AlertAsync("Please enable Exposure Notifications before submitting a diagnosis.", "Exposure Notifications Disabled", "OK");
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DiagnosisTimestamp.Value));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();

                dialog.Hide();

                await UserDialogs.Instance.AlertAsync("Diagnosis Submitted", "Complete", "OK");
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                dialog.Hide();
                UserDialogs.Instance.Alert("Please try again later.", "Failed", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }));
    }
}
