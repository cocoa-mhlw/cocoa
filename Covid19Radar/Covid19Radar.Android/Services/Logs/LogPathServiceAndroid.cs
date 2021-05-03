/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Android.Content;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Droid.Services.Logs
{
    public class LogPathServiceAndroid : ILogPathDependencyService
    {
        #region Instance Properties

        public string LogsDirPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), logsDirName);
        public string LogUploadingTmpPath => context.CacheDir.Path;
        public string LogUploadingPublicPath => context.GetExternalFilesDir(string.Empty).Path;

        #endregion

        #region Static Fields

        private static readonly string logsDirName = "logs";

        #endregion

        #region Instance Fields

        private readonly Context context = Android.App.Application.Context;

        #endregion

        #region Constructors

        public LogPathServiceAndroid()
        {
        }

        #endregion
    }
}
