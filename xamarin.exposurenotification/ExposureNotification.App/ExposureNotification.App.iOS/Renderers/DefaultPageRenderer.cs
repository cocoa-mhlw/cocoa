using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExposureNotification.App.iOS;
using ExposureNotification.App.Styles;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(DefaultPageRenderer))]
namespace ExposureNotification.App.iOS
{

    public class DefaultPageRenderer : Xamarin.Forms.Platform.iOS.PageRenderer
    {
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            Console.WriteLine($"TraitCollectionDidChange: {TraitCollection.UserInterfaceStyle} != {previousTraitCollection.UserInterfaceStyle}");

            if (TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
                ThemeHelper.ChangeTheme();
        }
    }
}