using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Covid19Radar.Controls;
using Covid19Radar.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(GradientButton), typeof(GradientButtonRenderer))]
namespace Covid19Radar.iOS.Renderers
{
    public class GradientButtonRenderer : ButtonRenderer
    {
        #region overrides
        /// <summary>
        /// Draw the gradient background button
        /// </summary>
        /// <param name="rect"></param>
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (Element != null)
            {
                if (Element is GradientButton)
                {
                    var gradientLayer = new CAGradientLayer();
                    var button = Element as GradientButton;
                    gradientLayer.Frame = rect;
                    gradientLayer.Colors = new CGColor[] {
                        button.StartColor.ToCGColor(),
                        button.EndColor.ToCGColor()
                    };

                    // horizontal gradient
                    if (button.GradientOrientation == GradientButton.GradientOrientationStates.Horizontal)
                    {
                        gradientLayer.StartPoint = new CGPoint(0.0, 0.5);
                        gradientLayer.EndPoint = new CGPoint(1.0, 0.5);
                    }
                    // vertical gradient
                    else if (button.GradientOrientation == GradientButton.GradientOrientationStates.Vertical)
                    {
                        gradientLayer.StartPoint = new CGPoint(0.5, 0.0);
                        gradientLayer.EndPoint = new CGPoint(0.5, 1.0);
                    }


                    gradientLayer.CornerRadius = button.CornerRadius;

                    NativeView.Layer.InsertSublayer(gradientLayer, 0);
                }
            }
        }
        #endregion
    }
}