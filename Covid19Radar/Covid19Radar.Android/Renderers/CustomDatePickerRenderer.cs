using Android.Content;
using Covid19Radar.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DatePicker), typeof(CustomDatePickerRendererAndroid))]
namespace Covid19Radar.Droid.Renderers
{
    public class CustomDatePickerRendererAndroid : DatePickerRenderer
    {
        public CustomDatePickerRendererAndroid(Context context) : base(context)
        {
        }
    }
}
