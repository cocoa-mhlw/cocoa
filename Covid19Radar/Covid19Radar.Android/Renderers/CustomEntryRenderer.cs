using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Covid19Radar.Droid.Renderers;
using Covid19Radar.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace Covid19Radar.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control is null || e.NewElement is null) return;

            var element = (CustomEntry)e.NewElement;

            if (element.BorderColor == Color.Default) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Control.BackgroundTintList = ColorStateList.ValueOf(element.BorderColor.ToAndroid());
            else
                Control.Background.SetColorFilter(element.BorderColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
        }
    }
}