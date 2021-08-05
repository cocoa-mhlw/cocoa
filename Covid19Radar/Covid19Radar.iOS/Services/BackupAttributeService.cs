/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Foundation;

namespace Covid19Radar.iOS.Services.Logs
{
    public class BackupAttributeService : IBackupAttributeService
    {
        private readonly ILogPathService logPathService;

        public BackupAttributeService(
            ILogPathService logPathService
            )
        {
            this.logPathService = logPathService;
        }

        public void SetSkipBackupAttributeToLogDir()
        {
            var logsDirPath = logPathService.LogsDirPath;
            var url = NSUrl.FromFilename(logsDirPath);
            _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
        }
    }
}
