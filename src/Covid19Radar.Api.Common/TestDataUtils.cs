// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.IO;
using System.Reflection;

namespace Covid19Radar.UnitTests
{
    public static class TestDataUtils
    {
        private const string TEST_DATA_DIR = "Files";

        // https://stackoverflow.com/a/23517039
        public static string GetLocalFilePath(string path)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath, TEST_DATA_DIR, path);
        }
    }
}
