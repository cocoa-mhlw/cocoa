﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using UIKit;
using Xamarin.Forms;

namespace Covid19Radar.iOS.Extensions
{
    internal static class AlignmentExtensions
	{
		internal static UITextAlignment ToNativeTextAlignment(this TextAlignment alignment, EffectiveFlowDirection flowDirection)
		{
			var isLtr = flowDirection.IsLeftToRight();
			switch (alignment)
			{
				case TextAlignment.Center:
					return UITextAlignment.Center;
				case TextAlignment.End:
					if (isLtr)
						return UITextAlignment.Right;
					else
						return UITextAlignment.Left;
				default:
					if (isLtr)
						return UITextAlignment.Left;
					else
						return UITextAlignment.Right;
			}
		}

		internal static UIControlContentVerticalAlignment ToNativeTextAlignment(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return UIControlContentVerticalAlignment.Center;
				case TextAlignment.End:
					return UIControlContentVerticalAlignment.Bottom;
				case TextAlignment.Start:
					return UIControlContentVerticalAlignment.Top;
				default:
					return UIControlContentVerticalAlignment.Top;
			}
		}
	}
}
