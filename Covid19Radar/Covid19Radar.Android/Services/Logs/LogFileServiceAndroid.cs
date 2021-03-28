/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Xamarin.Forms;

[assembly: Dependency(typeof(Covid19Radar.Droid.Services.Logs.LogFileServiceAndroid))]
namespace Covid19Radar.Droid.Services.Logs
{
    public class LogFileServiceAndroid : ILogFileDependencyService
    {
        public void AddSkipBackupAttribute()
        {
            // Skip backup in `AndroidManifest.xml`
        }
    }
}
