/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class StorageServiceMock : IStorageService
    {
        public async Task<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath)
        {
            await Task.Delay(500);
            return true;
        }
    }
}
