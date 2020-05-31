using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using ExposureNotification.App.Services;
using ExposureNotification.App.Views;
using MvvmHelpers.Commands;

using Command = MvvmHelpers.Commands.Command;

namespace ExposureNotification.App.ViewModels
{
	public class WelcomeViewModel : ViewModelBase
	{
		public WelcomeViewModel()
		{
			_ = Initialize();
		}

		async Task Initialize()
		{
			IsEnabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
			if (IsEnabled)
				await GoToAsync($"//{nameof(InfoPage)}");
		}

		public new bool IsEnabled
		{
			get => LocalStateManager.Instance.LastIsEnabled;
			set
			{
				LocalStateManager.Instance.LastIsEnabled = value;
				LocalStateManager.Save();
				OnPropertyChanged();
			}
		}

		public bool IsWelcomed
		{
			get => LocalStateManager.Instance.IsWelcomed;
			set
			{
				LocalStateManager.Instance.IsWelcomed = value;
				LocalStateManager.Save();
				OnPropertyChanged();
			}
		}

		public AsyncCommand EnableCommand
			=> new AsyncCommand(async () =>
			{
				using var _ = UserDialogs.Instance.Loading();

				try
				{
					var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
					if (!enabled)
						await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();

					await GoToAsync($"//{nameof(InfoPage)}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Unable to start notifications: {ex}");
				}
			});

		public ICommand GetStartedCommand
			=> new Command(() => IsWelcomed = true);

		public ICommand NotNowCommand
			=> new Command(() => IsWelcomed = false);
	}
}
