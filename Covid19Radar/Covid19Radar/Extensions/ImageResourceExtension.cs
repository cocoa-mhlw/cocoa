using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Extensions
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source is null) return null;
        
            ImageSource imageSource = null;

            if (Source.StartsWith("http"))
            {
                imageSource = ImageSource.FromUri(new Uri(Source));
            }
            else
            {
                // Do your translation lookup here, using whatever method you require
                imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);
            }

            return imageSource;
        }
    }
}