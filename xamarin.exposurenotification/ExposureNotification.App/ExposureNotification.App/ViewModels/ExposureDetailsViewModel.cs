using System;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace ExposureNotification.App.ViewModels
{
	[QueryProperty(nameof(ExposureInfoParameter), "info")]
	public class ExposureDetailsViewModel : ViewModelBase
	{
		public ExposureDetailsViewModel()
		{
		}

		public string ExposureInfoParameter
		{
			set
			{
				var json = Uri.UnescapeDataString(value ?? string.Empty);
				if (!string.IsNullOrWhiteSpace(json))
				{
					ExposureInfo = JsonConvert.DeserializeObject<ExposureInfo>(json);
					OnPropertyChanged(nameof(ExposureInfo));
				}
			}
		}

		public AsyncCommand CancelCommand
			=> new AsyncCommand(() => GoToAsync(".."));

		public ExposureInfo ExposureInfo { get; set; } = new ExposureInfo();

		public DateTime When
			=> ExposureInfo.Timestamp;

		public TimeSpan Duration
			=> ExposureInfo.Duration;
	}
}
