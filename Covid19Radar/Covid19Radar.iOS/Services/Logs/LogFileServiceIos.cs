/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Covid19Radar.iOS.Services.Logs.LogFileServiceIos))]
namespace Covid19Radar.iOS.Services.Logs
{
    public class LogFileServiceIos : ILogFileDependencyService
    {
        public void AddSkipBackupAttribute()
        {
            var logsDirPath = DependencyService.Resolve<ILogPathService>().LogsDirPath;
            var url = NSUrl.FromFilename(logsDirPath);
            _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
        }
    }
}
