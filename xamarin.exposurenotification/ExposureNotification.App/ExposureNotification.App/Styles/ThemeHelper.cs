
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExposureNotification.App.Styles
{
    public static class ThemeHelper
    {
        static AppTheme CurrentTheme { get; set; } = AppTheme.Unspecified;

        public static void ChangeTheme(bool forceTheme = false)
        {
            var  theme = AppInfo.RequestedTheme;
            // don't change to the same theme
            if (theme == CurrentTheme && !forceTheme)
                return;

            //// clear all the resources
            var applicationResourceDictionary = Application.Current.Resources;

#pragma warning disable IDE0007 // Use implicit type
            ResourceDictionary newTheme = theme switch
#pragma warning restore IDE0007 // Use implicit type
            {
                AppTheme.Dark => new DarkTheme(),
                AppTheme.Unspecified => new LightTheme(),
                AppTheme.Light => new LightTheme(),
                _ => new LightTheme()
            };

            ManuallyCopyThemes(newTheme, applicationResourceDictionary);

            CurrentTheme = theme;
        }

        static void ManuallyCopyThemes(ResourceDictionary fromResource, ResourceDictionary toResource)
        {
            foreach (var item in fromResource.Keys)
                toResource[item] = fromResource[item];
        }
    }
}
