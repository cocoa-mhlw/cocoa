/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System.Globalization;
using Xunit;

namespace Covid19Radar.UnitTests
{
    public interface IUseFixedCulture : IClassFixture<CultureFixer> { }

    public class CultureFixer
    {
        public CultureFixer() : this(CultureInfo.InvariantCulture) { }

        protected CultureFixer(CultureInfo c)
        {
            CultureInfo.DefaultThreadCurrentCulture   = c;
            CultureInfo.DefaultThreadCurrentUICulture = c;
            CultureInfo.CurrentCulture                = c;
            CultureInfo.CurrentUICulture              = c;
        }
    }
}
