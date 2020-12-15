using Covid19Radar.Controls;
using Covid19Radar.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRendererIos))]
namespace Covid19Radar.iOS.Renderers
{
    public class CustomDatePickerRendererIos : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && Control != null)
            {
                var datePicker = Control.InputView as UIDatePicker;
                datePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
            }
        }
    }
}
