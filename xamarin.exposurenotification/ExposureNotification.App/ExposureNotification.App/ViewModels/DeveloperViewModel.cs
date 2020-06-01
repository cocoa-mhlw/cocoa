using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using ExposureNotification.App.Services;
using MvvmHelpers.Commands;
using Xamarin.Forms;

namespace ExposureNotification.App.ViewModels
{
	public class DeveloperViewModel : ViewModelBase
	{
		public DeveloperViewModel()
		{
		}

		public string NativeImplementationName
			=> Xamarin.ExposureNotifications.ExposureNotification.OverridesNativeImplementation
				? "TEST" : "LIVE";

		public string CurrentBatchFileIndex
			=> string.Join(", ", LocalStateManager.Instance.ServerBatchNumbers.Select(p => $"{p.Key}={p.Value}"));

		public AsyncCommand ResetSelfDiagnosis
			=> new AsyncCommand(() =>
			{
				LocalStateManager.Instance.ClearDiagnosis();
				LocalStateManager.Save();
				return UserDialogs.Instance.AlertAsync("Self Diagnosis Cleared!");
			});


		public AsyncCommand ResetExposures
			=> new AsyncCommand(async () =>
			{
				await Device.InvokeOnMainThreadAsync(() => LocalStateManager.Instance.ExposureInformation.Clear());

				LocalStateManager.Instance.ExposureSummary = null;
				LocalStateManager.Save();
				await UserDialogs.Instance.AlertAsync("Exposures Cleared!");
			});

		public AsyncCommand AddExposures
			=> new AsyncCommand(() =>
			{
				return Device.InvokeOnMainThreadAsync(() =>
				{
					LocalStateManager.Instance.ExposureInformation.Add(
						new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-7), TimeSpan.FromMinutes(30), 70, 6, Xamarin.ExposureNotifications.RiskLevel.High));
					LocalStateManager.Instance.ExposureInformation.Add(
						new Xamarin.ExposureNotifications.ExposureInfo(DateTime.Now.AddDays(-3), TimeSpan.FromMinutes(10), 40, 3, Xamarin.ExposureNotifications.RiskLevel.Low));

					LocalStateManager.Save();
				});
			});

		public AsyncCommand ResetWelcome
			=> new AsyncCommand(() =>
			{
				LocalStateManager.Instance.IsWelcomed = false;
				LocalStateManager.Save();
				return UserDialogs.Instance.AlertAsync("Welcome state reset!");
			});

		public AsyncCommand ResetEnabled
			=> new AsyncCommand(async () =>
			{
				using (UserDialogs.Instance.Loading(string.Empty))
				{
					if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
						await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();

					LocalStateManager.Instance.LastIsEnabled = false;
					LocalStateManager.Save();
				}
				await UserDialogs.Instance.AlertAsync("Last known enabled state reset!");
			});

		public AsyncCommand ResetBatchFileIndex
			=> new AsyncCommand(() =>
			{
				LocalStateManager.Instance.ServerBatchNumbers = AppSettings.Instance.GetDefaultDefaultBatch();
				LocalStateManager.Save();
				OnPropertyChanged(nameof(CurrentBatchFileIndex));
				return UserDialogs.Instance.AlertAsync("Reset Batch file index!");
			});

		public AsyncCommand ManualTriggerKeyFetch
			=> new AsyncCommand(async () =>
			{
				using (UserDialogs.Instance.Loading("Fetching..."))
				{
					await Xamarin.ExposureNotifications.ExposureNotification.UpdateKeysFromServer();

					OnPropertyChanged(nameof(CurrentBatchFileIndex));
				}
			});
	}
}
