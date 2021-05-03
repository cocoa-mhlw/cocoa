/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.iOS.Services.Logs
{
    public class LogPathServiceIos : ILogPathDependencyService
    {
        #region Instance Properties

        public string LogsDirPath
        {
            get
            {
                var libraryPath = Path.Combine(documentsPath, "..", "Library");
                var applicationSupportPath = Path.Combine(libraryPath, "Application Support");
                return Path.Combine(applicationSupportPath, logsDirName);
            }
        }
        public string LogUploadingTmpPath => Path.Combine(documentsPath, "..", "tmp");
        public string LogUploadingPublicPath => documentsPath;

        #endregion

        #region Static Fields

        private static readonly string logsDirName = "Logs";

        #endregion

        #region Instance Fields

        private readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        #endregion

        #region Constructors

        public LogPathServiceIos()
        {
        }

        #endregion
    }
}
