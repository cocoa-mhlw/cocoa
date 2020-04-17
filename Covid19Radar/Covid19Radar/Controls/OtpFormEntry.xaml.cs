using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Covid19Radar.Controls
{
    /// <summary>
    /// OTP Form
    /// </summary>
    public partial class OtpFormEntry : ContentView
    {
        private readonly Entry[] _entries;

        public OtpFormEntry()
        {
            InitializeComponent();
            _entries = new[] {OtpEntry1,
            OtpEntry2,
            OtpEntry3,
            OtpEntry4,
            OtpEntry5,
            OtpEntry6 };
        }

        /// <summary>
        /// Subscribe Events. Call this in <see cref="Xamarin.Forms.Page.OnAppearing"/> in the page
        /// </summary>
        public void SubscribeEvents()
        {
            OtpEntry1.TextChanged += TextChanged;
            OtpEntry2.TextChanged += TextChanged;
            OtpEntry3.TextChanged += TextChanged;
            OtpEntry4.TextChanged += TextChanged;
            OtpEntry5.TextChanged += TextChanged;
            OtpEntry6.TextChanged += TextChanged;

            var entry = new Entry();
            OtpEntry1.Focused += TextFocused;
            OtpEntry2.Focused += TextFocused;
            OtpEntry3.Focused += TextFocused;
            OtpEntry4.Focused += TextFocused;
            OtpEntry5.Focused += TextFocused;
            OtpEntry6.Focused += TextFocused;
        }

        private void TextFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry && e.IsFocused && !string.IsNullOrEmpty(entry.Text))
            {
                entry.Text = "";
                SetOtpText();
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
            {
                var entryFocusedIndex = Array.FindIndex(_entries, (x) => x.AutomationId.Equals(entry.AutomationId, StringComparison.Ordinal));
                var nextFocusedEntryIndex = entryFocusedIndex + 1;
                if (nextFocusedEntryIndex == 6)
                {
                    OtpEntry6.Unfocus();
                }
                else
                {
                    var nextFocusedEntry = _entries[nextFocusedEntryIndex];
                    nextFocusedEntry.Focus();
                }
                SetOtpText();
            }
        }

        private void SetOtpText()
        {
            OtpText = "";
            foreach (var entry in _entries)
            {
                OtpText += entry.Text;
            }
        }

        /// <summary>
        /// Unsubscribe Events. Call this in <see cref="Xamarin.Forms.Page.OnDisappearing"/> in the page
        /// </summary>
        public void UnsubscribeEvents()
        {
            OtpEntry1.TextChanged -= TextChanged;
            OtpEntry2.TextChanged -= TextChanged;
            OtpEntry3.TextChanged -= TextChanged;
            OtpEntry4.TextChanged -= TextChanged;
            OtpEntry5.TextChanged -= TextChanged;
            OtpEntry6.TextChanged -= TextChanged;

            OtpEntry1.Focused -= TextFocused;
            OtpEntry2.Focused -= TextFocused;
            OtpEntry3.Focused -= TextFocused;
            OtpEntry4.Focused -= TextFocused;
            OtpEntry5.Focused -= TextFocused;
            OtpEntry6.Focused -= TextFocused;
        }

        public static readonly BindableProperty OtpTextProperty =
            BindableProperty.Create(nameof(OtpText),
                typeof(string),
                typeof(OtpFormEntry),
                null,
                defaultBindingMode: BindingMode.OneWayToSource);

        /// <summary>
        /// OTP Text
        /// </summary>
        public string OtpText
        {
            get => (string)GetValue(OtpTextProperty);
            set => SetValue(OtpTextProperty, value);
        }
    }
}
