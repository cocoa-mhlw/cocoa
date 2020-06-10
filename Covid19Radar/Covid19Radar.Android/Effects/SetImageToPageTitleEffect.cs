using Android.Support.V7.Widget;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Covid19Radar")]
[assembly: ExportEffect(typeof(Covid19Radar.Droid.Effects.SetImageToPageTitleEffect), nameof(Covid19Radar.Droid.Effects.SetImageToPageTitleEffect))]
namespace Covid19Radar.Droid.Effects
{
    public class SetImageToPageTitleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            SetLogo();
            if (Container is PageRenderer r)
            {
                r.Element.Title = ""; // don't need title text
                r.Element.Disappearing += Element_Disappearing;
                r.Element.Appearing += Element_Appearing;
            }
        }

        private static void SetLogo()
        {
            var toolbar = MainActivity.Current.FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetLogo(Resource.Drawable.ic_hamburger);
        }

        private void Element_Appearing(object sender, EventArgs e) => SetLogo();

        private void Element_Disappearing(object sender, System.EventArgs e)
        {
            var toolbar = MainActivity.Current.FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetLogo(Android.Resource.Color.Transparent);
        }

        protected override void OnDetached()
        {
            if (Container is PageRenderer r)
            {
                r.Element.Disappearing -= Element_Disappearing;
                r.Element.Appearing -= Element_Appearing;
            }
        }
    }
}