using Xamarin.Forms;

namespace Covid19Radar.Controls
{
    public partial class OtpEntry : ContentView
    {
        public OtpEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Text),
                typeof(string),
                typeof(OtpEntry),
                null,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: TextChanged);

        private static void TextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((OtpEntry)bindable).Entry.Text = (string)newValue;
        }

        /// <summary>
        /// Text for the OTP.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
