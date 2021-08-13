using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IDiagnosisKeyRepository
    {
        public Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync(ServerConfiguration serverConfiguration);

        public Task<string> DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string outputDir);
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
        private const string CATALOG_FILE_NAME = "list.json";
        private const long BUFFER_LENGTH = 4 * 1024 * 1024;

        private readonly HttpClient _client;
        private readonly ILoggerService _loggerService;

        public DiagnosisKeyRepository(
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _client = httpClientService.Create();
            _loggerService = loggerService;
        }

        public async Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync(ServerConfiguration serverConfiguration)
        {
            Uri uri = new Uri($"{serverConfiguration.ApiEndpoint}/{serverConfiguration.Region}/{CATALOG_FILE_NAME}");
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                _loggerService.Debug(content);
                return JsonConvert.DeserializeObject<List<DiagnosisKeyEntry>>(content);
            }
            else
            {
                _loggerService.Debug($"GetDiagnosisKeysListAsync {response.StatusCode}");
            }

            return new List<DiagnosisKeyEntry>();
        }

        public async Task<string> DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string outputDir)
        {
            Uri uri = new Uri(diagnosisKeyEntry.Url);
            HttpResponseMessage response = await _client.GetAsync(uri);
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
                return null;
            }
        }

    }

}
