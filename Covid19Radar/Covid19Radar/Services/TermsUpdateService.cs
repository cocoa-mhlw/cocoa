/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface ITermsUpdateService
    {
        Task<TermsUpdateInfoModel> GetTermsUpdateInfo();
        bool IsUpdated(TermsType termsType, TermsUpdateInfoModel termsUpdateInfo);
    }

    public class TermsUpdateService : ITermsUpdateService
    {
        private const double TimeoutSeconds = 5.0;

        private readonly ILoggerService loggerService;
        private readonly IUserDataRepository userDataRepository;

        public TermsUpdateService(
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            )
        {
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
        }

        public async Task<TermsUpdateInfoModel> GetTermsUpdateInfo()
        {
            loggerService.StartMethod();

            var uri = AppResources.UrlTermsUpdate;
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

                    var json = await client.GetStringAsync(uri);
                    loggerService.Info($"uri: {uri}");
                    loggerService.Info($"TermsUpdateInfo: {json}");

                    var deserializedJson = JsonConvert.DeserializeObject<TermsUpdateInfoModel>(json);

                    loggerService.EndMethod();

                    return deserializedJson;
                }
                catch (Exception ex)
                {
                    loggerService.Exception("Failed to get terms update info.", ex);
                    loggerService.EndMethod();

                    return new TermsUpdateInfoModel();
                }
            }
        }

        public bool IsUpdated(TermsType termsType, TermsUpdateInfoModel termsUpdateInfo)
        {
            loggerService.StartMethod();

            TermsUpdateInfoModel.Detail info = termsType switch
            {
                TermsType.TermsOfService => termsUpdateInfo.TermsOfService,
                TermsType.PrivacyPolicy => termsUpdateInfo.PrivacyPolicy,
                _ => throw new NotSupportedException()
            };

            if (info == null)
            {
                loggerService.EndMethod();
                return false;
            }

            var updateDatetime = info.UpdateDateTimeUtc;

            DateTime lastUpdateDate = userDataRepository.GetLastUpdateDate(termsType);
            loggerService.Info($"termsType: {termsType}, lastUpdateDate: {lastUpdateDate}, updateDatetimeUtc: {updateDatetime}");
            loggerService.EndMethod();

            return lastUpdateDate < updateDatetime;
        }
    }
}
