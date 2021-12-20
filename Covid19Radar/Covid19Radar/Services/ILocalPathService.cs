/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public interface ILocalPathService
    {
        private const string EXPOSURE_CONFIGURATION_DIR = "exposure_configuration";
        private const string CURRENT_EXPOSURE_CONFIGURATION_FILENAME = "current.json";

        private const string DIAGNOSIS_KEYS_DIR = "diagnosis_keys";

        string ExposureConfigurationDirPath =>
            Path.Combine(FileSystem.AppDataDirectory, EXPOSURE_CONFIGURATION_DIR);

        string CurrentExposureConfigurationPath =>
            Path.Combine(ExposureConfigurationDirPath, CURRENT_EXPOSURE_CONFIGURATION_FILENAME);

        string CacheDirectory => FileSystem.CacheDirectory;

        string LogsDirPath { get; }
        string LogUploadingTmpPath { get; }
        string LogUploadingPublicPath { get; }

        public static string GetDiagnosisKeysDir(string baseDir, string region, string subRegion)
        {
                var dir = Path.Combine(baseDir, DIAGNOSIS_KEYS_DIR);
                dir = Path.Combine(dir, region);

                if (string.IsNullOrEmpty(subRegion))
                {
                    return dir;
                }

                dir = Path.Combine(dir, subRegion);

                return dir;
        }
    }
}
