using System;
using ExposureNotification.App.Resources;
using ExposureNotification.App.Services;
using ExposureNotification.App.Views;
using MvvmHelpers.Commands;
using Xamarin.Essentials;

namespace ExposureNotification.App.ViewModels
{
	public class NotifyOthersViewModel : ViewModelBase
	{
		public NotifyOthersViewModel()
		{
			IsEnabled = true;
		}

		public bool DiagnosisPending
			=> (LocalStateManager.Instance.LatestDiagnosis?.DiagnosisDate ?? DateTimeOffset.MinValue)
				>= DateTimeOffset.UtcNow.AddDays(-14);

		public DateTimeOffset DiagnosisShareTimestamp
			=> LocalStateManager.Instance.LatestDiagnosis?.DiagnosisDate ?? DateTimeOffset.MinValue;

		public AsyncCommand SharePositiveDiagnosisCommand
			=> new AsyncCommand(() => GoToAsync($"{nameof(SharePositiveDiagnosisPage)}"));

		public AsyncCommand LearnMoreCommand
			=> new AsyncCommand(() => Browser.OpenAsync(Strings.NotifyOthers_LearnMore_Url));
	}
}
