using System.Threading.Tasks;
using ExposureNotification.App.Styles;
using ExposureNotification.App.Views;
using Plugin.LocalNotification;
using Xamarin.Forms;

namespace ExposureNotification.App
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

#if DEBUG
			// For debug mode, set the mock api provider to interact
			// with some fake data
			Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(
				new Services.TestNativeImplementation());
#endif
			// Local Notification tap event listener
			NotificationCenter.Current.NotificationTapped += OnNotificationTapped;

			// Initialize the library
			Xamarin.ExposureNotifications.ExposureNotification.Init();

			MainPage = new AppShell();

			_ = InitializeBackgroundTasks();
		}

		async Task InitializeBackgroundTasks()
		{
			if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
				await Xamarin.ExposureNotifications.ExposureNotification.ScheduleFetchAsync();
		}

		void OnNotificationTapped(NotificationTappedEventArgs e)
			=> Shell.Current.GoToAsync($"//{nameof(ExposuresPage)}", false);

		protected override void OnStart()
		{
			OnResume();
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
			ThemeHelper.ChangeTheme(true);
		}
	}
}
