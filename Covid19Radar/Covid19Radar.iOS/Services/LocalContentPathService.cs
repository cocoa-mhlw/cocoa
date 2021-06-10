/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Services;
using Foundation;

namespace Covid19Radar.iOS.Services
{
    public class LocalContentPathService : ILocalContentService
    {
        private const string CONTENT_DIR = "Content";

        public string GetLocalContentUrl() => Path.Combine(NSBundle.MainBundle.BundleUrl.AbsoluteString, CONTENT_DIR);
    }
}
