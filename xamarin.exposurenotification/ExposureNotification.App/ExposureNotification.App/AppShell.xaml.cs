using ExposureNotification.App.Services;
using ExposureNotification.App.Views;
using Xamarin.Forms;

namespace ExposureNotification.App
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

#if DEBUG
			tabDeveloper.IsEnabled = true;
#endif

			Routing.RegisterRoute(nameof(ExposureDetailsPage), typeof(ExposureDetailsPage));
			Routing.RegisterRoute(nameof(SharePositiveDiagnosisPage), typeof(SharePositiveDiagnosisPage));

			if (LocalStateManager.Instance.LastIsEnabled)
				GoToAsync($"//{nameof(InfoPage)}", false);

		}
	}
}
