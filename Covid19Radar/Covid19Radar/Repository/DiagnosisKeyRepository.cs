/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IDiagnosisKeyRepository
    {
        public Task<(HttpStatusCode, IList<DiagnosisKeyEntry>)> GetDiagnosisKeysListAsync(string url, CancellationToken cancellationToken);

        public Task<string> DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string outputDir, CancellationToken cancellationToken);
    }

    [JsonObject]
    public class DiagnosisKeyEntry
    {
        [JsonProperty("region")]
        public int Region;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("created")]
        public long Created;
    }

    public class DiagnosisKeyRepository : IDiagnosisKeyRepository
    {
        private const long BUFFER_LENGTH = 4 * 1024 * 1024;

        private readonly ILoggerService _loggerService;

        private readonly IHttpClientService _httpClientService;

        public DiagnosisKeyRepository(
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _httpClientService = httpClientService;
            _loggerService = loggerService;
        }

        public async Task<(HttpStatusCode, IList<DiagnosisKeyEntry>)> GetDiagnosisKeysListAsync(string url, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClientService.CdnClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                _loggerService.Debug(content);

                return (response.StatusCode, JsonConvert.DeserializeObject<List<DiagnosisKeyEntry>>(content));
            }
            else
            {
                _loggerService.Debug($"GetDiagnosisKeysListAsync {response.StatusCode}");
            }

            return (response.StatusCode, new List<DiagnosisKeyEntry>());
        }

        public async Task<string> DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string outputDir, CancellationToken cancellationToken)
        {
            Uri uri = new Uri(diagnosisKeyEntry.Url);

            HttpResponseMessage response = await _httpClientService.CdnClient.GetAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                string fileName = uri.Segments[uri.Segments.Length - 1];
                string outputPath = Path.Combine(outputDir, fileName);

                byte[] buffer = new byte[BUFFER_LENGTH];

                using BufferedStream bs = new BufferedStream(await response.Content.ReadAsStreamAsync());
                using FileStream fs = File.OpenWrite(outputPath);

                int len = 0;
                while ((len = await bs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, len);
                }

                return outputPath;
            }
            else
            {
                _loggerService.Debug($"DownloadDiagnosisKeysAsync {response.StatusCode}");
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
        }
    }

}
