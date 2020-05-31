using System;
using System.Globalization;
using Xamarin.Forms;

namespace ExposureNotification.App.Converters
{
	public class InvertedBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value is bool v ? !v : value;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			value is bool v ? !v : value;
	}
}
