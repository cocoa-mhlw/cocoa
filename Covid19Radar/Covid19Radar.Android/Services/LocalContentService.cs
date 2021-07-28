/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Services;

namespace Covid19Radar.Droid.Services
{
    public class LocalContentService : ILocalContentService
    {
        private const string ASSET_PATH = "file:///android_asset";

        public string LicenseUrl
        {
            get => Path.Combine(ASSET_PATH, ILocalContentService.LICENSE_FILENAME);
        }
    }
}
