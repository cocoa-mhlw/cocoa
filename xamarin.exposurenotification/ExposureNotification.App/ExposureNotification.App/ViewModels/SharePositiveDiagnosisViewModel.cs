using System;
using System.Windows.Input;
using Acr.UserDialogs;
using ExposureNotification.App.Services;
using MvvmHelpers.Commands;
using Xamarin.Forms;

namespace ExposureNotification.App.ViewModels
{
	public class SharePositiveDiagnosisViewModel : ViewModelBase
	{
		public SharePositiveDiagnosisViewModel()
		{
			IsEnabled = true;
		}
		public string DiagnosisUid { get; set; }

		public DateTime? DiagnosisTimestamp { get; set; } = DateTime.Now;

		public AsyncCommand CancelCommand
			=> new AsyncCommand(() => GoToAsync(".."));

		public AsyncCommand SubmitDiagnosisCommand
			=> new AsyncCommand(async () =>
			{
				// Check the parameters
				if (string.IsNullOrEmpty(DiagnosisUid))
				{
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
					LocalStateManager.Instance.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DiagnosisTimestamp.Value));
					LocalStateManager.Save();

					// Submit our diagnosis
					await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();

					dialog.Hide();

					await UserDialogs.Instance.AlertAsync("Diagnosis Submitted", "Complete", "OK");

					await GoToAsync("..");
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
			});
	}
}
