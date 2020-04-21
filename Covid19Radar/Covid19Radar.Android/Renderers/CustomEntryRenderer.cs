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
        private CustomEntry _element;

        public CustomEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.KeyPress -= Control_KeyPress;
            }
            if (Control is null || e.NewElement is null) return;

            _element = (CustomEntry)e.NewElement;

            if (_element.BorderColor == Color.Default) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Control.BackgroundTintList = ColorStateList.ValueOf(_element.BorderColor.ToAndroid());
            else
                Control.Background.SetColorFilter(_element.BorderColor.ToAndroid(), PorterDuff.Mode.SrcAtop);

            Control.KeyPress += Control_KeyPress;
        }

        private void Control_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Android.Views.Keycode.Del)
            {
                _element.TriggerDeleteClicked();
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
