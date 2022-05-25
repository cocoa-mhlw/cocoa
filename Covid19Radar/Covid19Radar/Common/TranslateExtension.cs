using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Covid19Radar.Services;

namespace Covid19Radar.Common
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private static CultureInfo _ci = null;
        private const string ResourceId = "AppName.Resources.AppResources";

        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS ||
                Device.RuntimePlatform == Device.Android)
            {
                if (_ci == null)
                {
                    _ci = DependencyService.Get<ILocalizeService>().GetCurrentCultureInfo();
                }
            }
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(ResourceId,
                                         typeof(TranslateExtension).GetTypeInfo().Assembly);

            var translation = resmgr.GetString(Text, _ci);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, _ci.Name),
                    "Text");
#else
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}