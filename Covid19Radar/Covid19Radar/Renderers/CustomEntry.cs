using System;
using Xamarin.Forms;

namespace Covid19Radar.Renderers
{
    public class CustomEntry : Entry
    {
        public event EventHandler DeleteClicked;

        public static readonly BindableProperty BorderColorProperty =
           BindableProperty.Create(nameof(BorderColor),
               typeof(Color),
               typeof(CustomEntry),
               Color.Default);

        /// <summary>
        /// Border Color
        /// </summary>
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public void TriggerDeleteClicked()
        {
            DeleteClicked?.Invoke(this, null);
        }
    }
}
