using Xamarin.Forms;

namespace Covid19Radar.Renderers
{
    public class CustomEntry : Entry
    {
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
    }
}
