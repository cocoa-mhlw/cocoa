using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Core;
using Xamarin.Forms.Internals;

namespace Covid19Radar.iOS.Extensions
{
	public static class FontExtensions
	{
		static readonly string _defaultFontName = UIFont.SystemFontOfSize(12).Name;

		public static UIFont ToUIFont(this Font self) => ToNativeFont(self);

		internal static UIFont ToUIFont(this IFontElement element) => ToNativeFont(element);

		static UIFont _ToNativeFont(string family, float size, FontAttributes attributes)
		{
			var bold = (attributes & FontAttributes.Bold) != 0;
			var italic = (attributes & FontAttributes.Italic) != 0;

			if (family != null && family != _defaultFontName)
			{
				try
				{
					UIFont result = null;
					if (UIFont.FamilyNames.Contains(family))
					{
						var descriptor = new UIFontDescriptor().CreateWithFamily(family);

						if (bold || italic)
						{
							var traits = (UIFontDescriptorSymbolicTraits)0;
							if (bold)
								traits = traits | UIFontDescriptorSymbolicTraits.Bold;
							if (italic)
								traits = traits | UIFontDescriptorSymbolicTraits.Italic;

							descriptor = descriptor.CreateWithTraits(traits);
							result = UIFont.FromDescriptor(descriptor, size);
							if (result != null)
								return result;
						}
					}

					var cleansedFont = CleanseFontName(family);
					result = UIFont.FromName(cleansedFont, size);
					if (family.StartsWith(".SFUI", System.StringComparison.InvariantCultureIgnoreCase))
					{
						var fontWeight = family.Split('-').LastOrDefault();

						if (!string.IsNullOrWhiteSpace(fontWeight) && System.Enum.TryParse<UIFontWeight>(fontWeight, true, out var uIFontWeight))
						{
							result = UIFont.SystemFontOfSize(size, uIFontWeight);
							return result;
						}

						result = UIFont.SystemFontOfSize(size, UIFontWeight.Regular);
						return result;
					}
					if (result == null)
						result = UIFont.FromName(family, size);
					if (result != null)
						return result;
				}
				catch
				{
					Debug.WriteLine("Could not load font named: {0}", family);
				}
			}

			if (bold && italic)
			{
				var defaultFont = UIFont.SystemFontOfSize(size);

				var descriptor = defaultFont.FontDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold | UIFontDescriptorSymbolicTraits.Italic);
				return UIFont.FromDescriptor(descriptor, 0);
			}

			if (italic)
				return UIFont.ItalicSystemFontOfSize(size);

			if (bold)
				return UIFont.BoldSystemFontOfSize(size);

			return UIFont.SystemFontOfSize(size);
		}

		static string CleanseFontName(string fontName)
		{

			//First check Alias
			var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
			if (hasFontAlias)
				return fontPostScriptName;

			var fontFile = FontFile.FromString(fontName);

			if (!string.IsNullOrWhiteSpace(fontFile.Extension))
			{
				var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont)
					return filePath ?? fontFile.PostScriptName;
			}
			else
			{
				foreach (var ext in FontFile.Extensions)
				{

					var formated = fontFile.FileNameWithExtension(ext);
					var (hasFont, filePath) = FontRegistrar.HasFont(formated);
					if (hasFont)
						return filePath;
				}
			}
			return fontFile.PostScriptName;
		}

		static readonly Dictionary<ToNativeFontFontKey, UIFont> ToUiFont = new Dictionary<ToNativeFontFontKey, UIFont>();

		internal static bool IsDefault(this Span self)
		{
			return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) &&
					self.FontAttributes == FontAttributes.None;
		}

		static UIFont ToNativeFont(this IFontElement element)
		{
			var fontFamily = element.FontFamily;
			var fontSize = (float)element.FontSize;
			var fontAttributes = element.FontAttributes;
			return ToNativeFont(fontFamily, fontSize, fontAttributes, _ToNativeFont);
		}

		static UIFont ToNativeFont(this Font self)
		{
			var size = (float)self.FontSize;
			if (self.UseNamedSize)
			{
				switch (self.NamedSize)
				{
					case NamedSize.Micro:
						size = 12;
						break;
					case NamedSize.Small:
						size = 14;
						break;
					case NamedSize.Medium:
						size = 17; // as defined by iOS documentation
						break;
					case NamedSize.Large:
						size = 22;
						break;
					default:
						size = 17;
						break;
				}
			}

			var fontAttributes = self.FontAttributes;

			return ToNativeFont(self.FontFamily, size, fontAttributes, _ToNativeFont);
		}

		static UIFont ToNativeFont(string family, float size, FontAttributes attributes, Func<string, float, FontAttributes, UIFont> factory)
		{
			var key = new ToNativeFontFontKey(family, size, attributes);

			lock (ToUiFont)
			{
				UIFont value;
				if (ToUiFont.TryGetValue(key, out value))
					return value;
			}

			var generatedValue = factory(family, size, attributes);

			lock (ToUiFont)
			{
				UIFont value;
				if (!ToUiFont.TryGetValue(key, out value))
					ToUiFont.Add(key, value = generatedValue);
				return value;
			}
		}

		struct ToNativeFontFontKey
		{
			internal ToNativeFontFontKey(string family, float size, FontAttributes attributes)
			{
				_family = family;
				_size = size;
				_attributes = attributes;
			}
#pragma warning disable 0414 // these are not called explicitly, but they are used to establish uniqueness. allow it!
			string _family;
			float _size;
			FontAttributes _attributes;
#pragma warning restore 0414
		}
	
	}
}