using Android.App;
using Android.Content.PM;
using Android.OS;
using Acr.UserDialogs;
using Plugin.LocalNotification;
using Android.Content;
using ExposureNotification.App.Styles;
using Android.Content.Res;

namespace ExposureNotification.App.Droid
{
	[Activity(
		Label = "Exposure Notifications",
		Icon = "@mipmap/icon",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			Xamarin.Forms.Forms.Init(this, savedInstanceState);
			Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

			UserDialogs.Init(this);

			NotificationCenter.CreateNotificationChannel();

			LoadApplication(new App());

			NotificationCenter.NotifyNotificationTapped(base.Intent);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnNewIntent(Intent intent)
		{
			NotificationCenter.NotifyNotificationTapped(intent);

			base.OnNewIntent(intent);
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			ThemeHelper.ChangeTheme();
			base.OnConfigurationChanged(newConfig);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			Xamarin.ExposureNotifications.ExposureNotification.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
