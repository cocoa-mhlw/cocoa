using System;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public enum TermsType
    {
        TermsOfService,
        PrivacyPolicy
    }

    public interface ITermsUpdateService
    {
        Task<TermsUpdateInfoModel> GetTermsUpdateInfo();
        bool IsReAgree(TermsType privacyType, TermsUpdateInfoModel privacyUpdateInfo);
        Task SaveLastUpdateDateAsync(TermsType privacyType, DateTime updateDate);
    }

    public class TermsUpdateService : ITermsUpdateService
    {
        private readonly ILoggerService loggerService;
        private readonly IApplicationPropertyService applicationPropertyService;

        private static readonly string TermsOfServiceLastUpdateDateKey = "TermsOfServiceLastUpdateDateTime";
        private static readonly string PrivacyPolicyLastUpdateDateKey = "PrivacyPolicyLastUpdateDateTime";

        public TermsUpdateService(ILoggerService loggerService, IApplicationPropertyService applicationPropertyService)
        {
            this.loggerService = loggerService;
            this.applicationPropertyService = applicationPropertyService;
        }

        public async Task<TermsUpdateInfoModel> GetTermsUpdateInfo()
        {
            loggerService.StartMethod();

            var uri = AppResources.UrlTermsUpdate;
            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(uri);
                    loggerService.Info($"uri: {uri}");
                    loggerService.Info($"TermsUpdateInfo: {json}");

                    var deserializedJson = Utils.DeserializeFromJson<TermsUpdateInfoModel>(json);

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

        public bool IsReAgree(TermsType privacyType, TermsUpdateInfoModel termsUpdateInfo)
        {
            loggerService.StartMethod();

            TermsUpdateInfoModel.Detail info = null;
            string key = null;

            switch (privacyType)
            {
                case TermsType.TermsOfService:
                    info = termsUpdateInfo.TermsOfService;
                    key = TermsOfServiceLastUpdateDateKey;
                    break;
                case TermsType.PrivacyPolicy:
                    info = termsUpdateInfo.PrivacyPolicy;
                    key = PrivacyPolicyLastUpdateDateKey;
                    break;
            }

            if (info == null)
            {
                loggerService.EndMethod();
                return false;
            }

            var lastUpdateDate = new DateTime();
            if (applicationPropertyService.ContainsKey(key))
            {
                lastUpdateDate = (DateTime)applicationPropertyService.GetProperties(key);
            }

            loggerService.Info($"privacyType: {privacyType}, lastUpdateDate: {lastUpdateDate}, info.UpdateDateTime: {info.UpdateDateTime}");
            loggerService.EndMethod();

            return lastUpdateDate < info.UpdateDateTime;
        }

        public async Task SaveLastUpdateDateAsync(TermsType termsType, DateTime updateDate)
        {
            loggerService.StartMethod();

            var key = termsType == TermsType.TermsOfService ? TermsOfServiceLastUpdateDateKey : PrivacyPolicyLastUpdateDateKey;
            await applicationPropertyService.SavePropertiesAsync(key, updateDate);

            loggerService.EndMethod();
        }
    }
}
