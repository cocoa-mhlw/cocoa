/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    /// <summary>
    ///  <see cref="Covid19Radar.Model.LatestInformationModel"/>を取得します。
    /// </summary>
    public interface ILatestInformationService
    {
        /// <summary>
        ///  <see cref="Covid19Radar.Model.LatestInformationModel"/>を非同期的にダウンロードします。
        /// </summary>
        /// <returns>ダウンロードした最新情報を含むこの処理の非同期操作です。</returns>
        public ValueTask<LatestInformationModel[]?> DownloadAsync();
    }

    internal sealed class LatestInformationService : ILatestInformationService
    {
        private readonly ILoggerService     _logger;
        private readonly IHttpClientService _http_client_service;
        private readonly string             _url;

        public LatestInformationService(ILoggerService logger, IHttpClientService httpClientService)
        {
            _logger              = logger            ?? throw new ArgumentNullException(nameof(logger));
            _http_client_service = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            _url                 = "https://"; // TODO: アドレスを設定する。
        }

        public async ValueTask<LatestInformationModel[]?> DownloadAsync()
        {
            _logger.StartMethod();
            try {
                using (var client = _http_client_service.Create()) {
                    var response =  await client.GetAsync(_url);
                    if (response.StatusCode == HttpStatusCode.OK) {
                        var stream = await response.Content.ReadAsStreamAsync();
                        await using (stream.ConfigureAwait(false)) {
                            var result = await JsonSerializer.DeserializeAsync<LatestInformationModel[]>(stream);
                            _logger.Info($"Downloaded the latest information of {result.Length}.");
                            return result;
                        }
                    } else {
                        _logger.Warning($"Could not download the latest information. (HTTP ERROR: {response.StatusCode})");
                        return null;
                    }
                }
            } catch (Exception e) {
                _logger.Exception("Failed to download the latest information.", e);
                return null;
            } finally {
                _logger.EndMethod();
            }
        }
    }

#if USE_MOCK
    internal sealed class LatestInformationServiceMock : ILatestInformationService
    {
        public ValueTask<LatestInformationModel[]?> DownloadAsync()
        {
            return new ValueTask<LatestInformationModel[]?>(new[] {
                new LatestInformationModel() {
                    Title    = "[TITLE IN HERE]",
                    Posted   = DateTime.Now,
                    Contents = "[CONTENTS IN HERE]",
                    Tags     = new[] { "Tag 1", "Tag 2", "Tag 3" }
                },
                new LatestInformationModel() {
                    Title    = "[TITLE IN HERE]",
                    Posted   = DateTime.Now,
                    Contents = "[CONTENTS IN HERE]",
                    Tags     = new[] { "Tag 1", "Tag 2", "Tag 3" }
                },
                new LatestInformationModel() {
                    Title    = "[TITLE IN HERE]",
                    Posted   = DateTime.Now,
                    Contents = "[CONTENTS IN HERE]",
                    Tags     = new[] { "Tag 1", "Tag 2", "Tag 3" }
                },
                new LatestInformationModel() {
                    Title    = "[TITLE IN HERE]",
                    Posted   = DateTime.Now,
                    Contents = "[CONTENTS IN HERE]",
                    Tags     = new[] { "Tag 1", "Tag 2", "Tag 3" }
                }
            });
        }
    }
#endif
}
