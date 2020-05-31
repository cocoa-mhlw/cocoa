using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ExposureNotification.App.Services;
using ExposureNotification.App.Views;
using MvvmHelpers.Commands;

namespace ExposureNotification.App.ViewModels
{
	public class InfoViewModel : ViewModelBase
	{
		public InfoViewModel()
		{
			_ = Initialize();
		}

		async Task Initialize()
		{
			var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
			if (!enabled)
				await Disabled();
		}

		Task Disabled()
		{
			LocalStateManager.Instance.LastIsEnabled = false;
			LocalStateManager.Instance.IsWelcomed = false;
			LocalStateManager.Save();

			return GoToAsync($"//{nameof(WelcomePage)}");
		}

		public AsyncCommand DisableCommand
			=> new AsyncCommand(async () =>
			{
				try
				{
					using (UserDialogs.Instance.Loading())
					{
						var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

						if (enabled)
							await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error disabling notifications: {ex}");
				}
				finally
				{
					await Disabled();
				}
			});
	}
}
