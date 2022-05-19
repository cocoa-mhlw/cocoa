// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;

namespace Covid19Radar.Services
{
    public class DialogService : IDialogService
    {
        private readonly IEssentialsService _essentialsService;

        public DialogService(IEssentialsService essentialsService)
        {
            _essentialsService = essentialsService;
        }

        public async Task<bool> ShowExposureNotificationOffWarningAsync() =>
            await ConfirmAsync(
                AppResources.ExposureNotificationOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel);

        public async Task<bool> ShowBluetoothOffWarningAsync()
        {
            if (_essentialsService.IsIos)
            {
                await AlertAsync(
                    AppResources.BluetoothOffWarningDialogMessage,
                    AppResources.CheckSettingsDialogTitle,
                    AppResources.ButtonClose);
                return false;
            }
            return await ConfirmAsync(
                AppResources.BluetoothOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel);
        }

        public async Task<bool> ShowLocationOffWarningAsync()
        {
            if (_essentialsService.IsIos)
            {
                throw new PlatformNotSupportedException();
            }
            return await ConfirmAsync(
                AppResources.LocationOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel);
        }

        public async Task ShowTemporarilyUnavailableWarningAsync() =>
            await AlertAsync(
                AppResources.TemporarilyUnavailableWarningMessage,
                AppResources.TemporarilyUnavailableWarningTitle,
                AppResources.ButtonOk);

        public async Task ShowHomePageUnknownErrorWaringAsync()
        {
            await AlertAsync(
                AppResources.HomePageDialogExceptionDescription,
                AppResources.HomePageDialogExceptionTitle,
                AppResources.ButtonOk);
        }

        public async Task<bool> ShowLocalNotificationOffWarningAsync()
        {
            return await ConfirmAsync(
                AppResources.LocalNotificationOffWarningDialogMessage,
                AppResources.LocalNotificationOffWarningDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel);
        }


        public async Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null) =>
            await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);

        public async Task AlertAsync(string message, string title = null, string okText = null) =>
            await UserDialogs.Instance.AlertAsync(message, title, okText);
    }
}
