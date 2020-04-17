using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Covid19Radar.iOS.Renderers;
using Covid19Radar.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace Covid19Radar.iOS.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        private CustomEntry _element;
        private CALayer _borderLine;
        private double _height;
        private double _width;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control is null || e.NewElement is null) return;

            _element = (CustomEntry)e.NewElement;

            if (_element.BorderColor == Color.Default) return;

            var uiColor = _element.BorderColor.ToUIColor();

            Control.BorderStyle = UITextBorderStyle.None;
            _height = Frame.Height / 2;

            _borderLine = new CALayer
            {
                BorderColor = uiColor.CGColor,
                BackgroundColor = uiColor.CGColor,
                Frame = new CGRect(0, _height, Frame.Width / 2, 1f)
            };

            Control.Layer.AddSublayer(_borderLine);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Width")
            {
                _width = _element.Width;
                _borderLine.Frame = new CGRect(0, _height, _width, 1f);
            }
        }
    }
}