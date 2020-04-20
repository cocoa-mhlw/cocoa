using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Covid19Radar.iOS.Controls;
using Covid19Radar.iOS.Extensions;
using Covid19Radar.iOS.Renderers;
using Covid19Radar.Renderers;
using Foundation;
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
        private UIBackwardsTextField _textfield;
        private bool _cursorPositionChangePending;
        private bool _selectionLengthChangePending;
        private IDisposable _selectedTextRangeObserver;
        private bool _nativeSelectionIsUpdating;
        private readonly Color _defaultPlaceholderColor = new UIColor(0.7f, 0.7f, 0.7f, 1).ToColor();


        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null && _textfield != null)
            {
                _textfield.EditingChanged -= EditingChanged;
                _textfield.OnDeleteBackward -= OnDeleteBackward;
                _textfield.EditingDidEnd -= EditingDidEnd;
                _textfield.EditingDidBegin -= EditingDidBegin;
                _textfield.ShouldChangeCharacters -= ShouldChangeCharacters;

                _textfield.Dispose();
                _selectedTextRangeObserver?.Dispose();
                _textfield = null;
            }
            if (Control is null || e.NewElement is null) return;

            _element = (CustomEntry)e.NewElement;

            if (_element.BorderColor == Color.Default) return;

            _textfield = new UIBackwardsTextField();
            _textfield.EditingChanged += EditingChanged;
            _textfield.OnDeleteBackward += OnDeleteBackward;
            _textfield.EditingDidEnd += EditingDidEnd;
            _textfield.EditingDidBegin += EditingDidBegin;
            _textfield.ShouldReturn = OnShouldReturn;
            _textfield.ShouldChangeCharacters += ShouldChangeCharacters;

            // When we set the control text, it triggers the UpdateCursorFromControl event, which updates CursorPosition and SelectionLength;
            // These one-time-use variables will let us initialize a CursorPosition and SelectionLength via ctor/xaml/etc.
            _cursorPositionChangePending = Element.IsSet(Entry.CursorPositionProperty);
            _selectionLengthChangePending = Element.IsSet(Entry.SelectionLengthProperty);

            SetNativeControl(_textfield);

            //Reupdate properties as when set native control all set up is gone
            Control.Font = _element.ToUIFont();
            Control.ApplyKeyboard(_element.Keyboard);
            Control.ReloadInputViews();
            Control.TextAlignment = _element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)_element).EffectiveFlowDirection);
            Control.VerticalAlignment = _element.VerticalTextAlignment.ToNativeTextAlignment();
            Control.TextColor = _element.TextColor.ToUIColor();

            SetupBorder();

            var formatted = (FormattedString)Element.Placeholder;
            if (formatted != null)
            {
                var targetColor = Element.PlaceholderColor;
                var color = targetColor.IsDefault ? _defaultPlaceholderColor : targetColor;
                UpdateAttributedPlaceholder(formatted.ToAttributed(Element, color));
                UpdateAttributedPlaceholder(Control.AttributedPlaceholder.AddCharacterSpacing(Element.Placeholder, Element.CharacterSpacing));
            }

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

        private void SetupBorder()
        {
            var uiColor = _element.BorderColor.ToUIColor();

            Control.BorderStyle = UITextBorderStyle.None;

            _height = _element.HeightRequest;
            if (_height <= 0)
            {
                _height = Frame.Height / 2;
            }

            _borderLine = new CALayer
            {
                BorderColor = uiColor.CGColor,
                BackgroundColor = uiColor.CGColor,
                Frame = new CGRect(0, _height, Frame.Width / 2, 1f)
            };

            Control.Layer.AddSublayer(_borderLine);
        }

        private bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            var newLength = textField?.Text?.Length + replacementString.Length - range.Length;
            return newLength <= Element?.MaxLength;
        }

        private void EditingDidBegin(object sender, EventArgs e)
        {
            if (!_cursorPositionChangePending && !_selectionLengthChangePending)
                UpdateCursorFromControl(null);
            else
                UpdateCursorSelection();

            _element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
        }

        private void EditingDidEnd(object sender, EventArgs e)
        {
            // Typing aid changes don't always raise EditingChanged event

            // Normalizing nulls to string.Empty allows us to ensure that a change from null to "" doesn't result in a change event.
            // While technically this is a difference it serves no functional good.
            var controlText = Control.Text ?? string.Empty;
            var entryText = _element.Text ?? string.Empty;
            if (controlText != entryText)
            {
                _element.SetValueFromRenderer(Entry.TextProperty, controlText);
            }

            _element.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
        }

        private void OnDeleteBackward(object sender, EventArgs e)
        {
            _element.TriggerDeleteClicked();
        }

        private void EditingChanged(object sender, EventArgs e)
        {
            var currentControlText = Control.Text;

            if (currentControlText.Length > _element.MaxLength)
                Control.Text = currentControlText.Substring(0, _element.MaxLength);
            _element.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }

        private void UpdateCursorFromControl(NSObservedChange obj)
        {
            if (_nativeSelectionIsUpdating || Control == null || Element == null)
                return;

            var currentSelection = Control.SelectedTextRange;
            if (currentSelection != null)
            {
                if (!_cursorPositionChangePending)
                {
                    int newCursorPosition = (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, currentSelection.Start);
                    if (newCursorPosition != Element.CursorPosition)
                        SetCursorPositionFromRenderer(newCursorPosition);
                }

                if (!_selectionLengthChangePending)
                {
                    int selectionLength = (int)Control.GetOffsetFromPosition(currentSelection.Start, currentSelection.End);

                    if (selectionLength != Element.SelectionLength)
                        SetSelectionLengthFromRenderer(selectionLength);
                }
            }
        }

        private void SetCursorPositionFromRenderer(int start)
        {
            try
            {
                _nativeSelectionIsUpdating = true;
                _element?.SetValueFromRenderer(Entry.CursorPositionProperty, start);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _nativeSelectionIsUpdating = false;
            }
        }

        private void SetSelectionLengthFromRenderer(int selectionLength)
        {
            try
            {
                _nativeSelectionIsUpdating = true;
                _element?.SetValueFromRenderer(Entry.SelectionLengthProperty, selectionLength);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _nativeSelectionIsUpdating = false;
            }
        }

        private void UpdateCursorSelection()
        {
            if (_nativeSelectionIsUpdating || Control == null || Element == null)
                return;

            _cursorPositionChangePending = _selectionLengthChangePending = true;

            // If this is run from the ctor, the control is likely too early in its lifecycle to be first responder yet. 
            // Anything done here will have no effect, so we'll skip this work until later.
            // We'll try again when the control does become first responder later OnEditingBegan
            if (Control.BecomeFirstResponder())
            {
                try
                {
                    int cursorPosition = Element.CursorPosition;

                    UITextPosition start = GetSelectionStart(cursorPosition, out int startOffset);
                    UITextPosition end = GetSelectionEnd(cursorPosition, start, startOffset);

                    Control.SelectedTextRange = Control.GetTextRange(start, end);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    _cursorPositionChangePending = _selectionLengthChangePending = false;
                }
            }
        }

        private UITextPosition GetSelectionEnd(int cursorPosition, UITextPosition start, int startOffset)
        {
            UITextPosition end = start;
            int endOffset = startOffset;
            int selectionLength = Element.SelectionLength;

            if (Element.IsSet(Entry.SelectionLengthProperty))
            {
                end = Control.GetPosition(start, Math.Max(startOffset, Math.Min(Control.Text.Length - cursorPosition, selectionLength))) ?? start;
                endOffset = Math.Max(startOffset, (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, end));
            }

            int newSelectionLength = Math.Max(0, endOffset - startOffset);
            if (newSelectionLength != selectionLength)
                SetSelectionLengthFromRenderer(newSelectionLength);

            return end;
        }

        private UITextPosition GetSelectionStart(int cursorPosition, out int startOffset)
        {
            UITextPosition start = Control.EndOfDocument;
            startOffset = Control.Text.Length;

            if (Element.IsSet(Entry.CursorPositionProperty))
            {
                start = Control.GetPosition(Control.BeginningOfDocument, cursorPosition) ?? Control.EndOfDocument;
                startOffset = Math.Max(0, (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, start));
            }

            if (startOffset != cursorPosition)
                SetCursorPositionFromRenderer(startOffset);

            return start;
        }

        
    }
}