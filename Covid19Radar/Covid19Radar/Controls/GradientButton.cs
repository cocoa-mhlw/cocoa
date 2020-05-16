using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.Controls
{
    // set a gradient button for a button
    public class GradientButton : Button
    {
        #region enum
        // specifies the orientation of the gradient color
        public enum GradientOrientationStates
        {
            Vertical,
            Horizontal
        }
        #endregion

        #region bindable properties
        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(
                nameof(StartColor),
                typeof(Color),
                typeof(GradientButton),
                default(Color));

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(
                nameof(EndColor),
                typeof(Color),
                typeof(GradientButton),
                default(Color));

        public static readonly BindableProperty GradientOrientationProperty =
            BindableProperty.Create(
                nameof(GradientOrientation),
                typeof(GradientOrientationStates),
                typeof(GradientButton),
                default(GradientOrientationStates));
        #endregion

        #region properties
        /// <summary>
        /// The start color of the gradient background
        /// </summary>
        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        /// <summary>
        ///  The end color of the gradient background
        /// </summary>
        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        /// <summary>
        ///  The gradient color orientation
        /// </summary>
        public GradientOrientationStates GradientOrientation
        {
            get => (GradientOrientationStates)GetValue(GradientOrientationProperty);
            set => SetValue(GradientOrientationProperty, value);
        }
        #endregion
    }
}
