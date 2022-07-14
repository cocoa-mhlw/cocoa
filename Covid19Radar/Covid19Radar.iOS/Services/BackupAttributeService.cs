/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;

namespace Covid19Radar.iOS.Services.Logs
{
    public class BackupAttributeService : IBackupAttributeService
    {
        private readonly ILogPathService logPathService;
        private readonly ILocalPathService localPathService;

        public BackupAttributeService(
            ILogPathService logPathService,
            ILocalPathService localPathService
            )
        {
            this.logPathService = logPathService;
            this.localPathService = localPathService;
        }

        public void SetSkipBackupAttributeToLogDir()
        {
            var logsDirPath = logPathService.LogsDirPath;
            var url = NSUrl.FromFilename(logsDirPath);
            _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
        }

        public void SetSkipBackupAttributeToEventLogDir()
        {
            var dirPath = localPathService.EventLogDirPath;
            var url = NSUrl.FromFilename(dirPath);
            _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(false));
        }
    }
}
