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
        Task Migrate(TermsType termsType, bool isAgree);
        Task<TermsUpdateInfoModel> GetTermsUpdateInfo();
        bool IsReAgree(TermsType termsType, TermsUpdateInfoModel privacyUpdateInfo);
        void SaveLastUpdateDate(TermsType termsType, DateTime updateDate);
        bool IsAllAgreed();
        void RemoveAllUpdateDate();
    }

    public class TermsUpdateService : ITermsUpdateService
    {
        private readonly ILoggerService loggerService;
        private readonly IApplicationPropertyService applicationPropertyService;
        private readonly IPreferencesService preferencesService;

        private static readonly string TermsOfServiceLastUpdateDateKey = "TermsOfServiceLastUpdateDateTime";
        private static readonly string PrivacyPolicyLastUpdateDateKey = "PrivacyPolicyLastUpdateDateTime";

        public TermsUpdateService(ILoggerService loggerService, IApplicationPropertyService applicationPropertyService, IPreferencesService preferencesService)
        {
            this.loggerService = loggerService;
            this.applicationPropertyService = applicationPropertyService;
            this.preferencesService = preferencesService;
        }

        public async Task Migrate(TermsType termsType, bool isAgree)
        {
            loggerService.StartMethod();

            var applicationPropertyKey = termsType == TermsType.TermsOfService ? TermsOfServiceLastUpdateDateKey : PrivacyPolicyLastUpdateDateKey;
            var preferenceKey = termsType == TermsType.TermsOfService ? PreferenceKey.TermsOfServiceLastUpdateDateTime : PreferenceKey.PrivacyPolicyLastUpdateDateTime;

            if (preferencesService.ContainsKey(applicationPropertyKey))
            {
                return;
            }

            if (isAgree)
            {
                if (applicationPropertyService.ContainsKey(applicationPropertyKey))
                {
                    var lastUpdateDate = (DateTime) applicationPropertyService.GetProperties(applicationPropertyKey);
                    preferencesService.SetValue(preferenceKey, lastUpdateDate);
                }
                else
                {
                    preferencesService.SetValue(preferenceKey, new DateTime());
                }
            }

            await applicationPropertyService.RemoveAsync(applicationPropertyKey);

            loggerService.EndMethod();
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

        public bool IsReAgree(TermsType termsType, TermsUpdateInfoModel termsUpdateInfo)
        {
            loggerService.StartMethod();

            TermsUpdateInfoModel.Detail info = null;
            string key = null;

            switch (termsType)
            {
                case TermsType.TermsOfService:
                    info = termsUpdateInfo.TermsOfService;
                    key = PreferenceKey.TermsOfServiceLastUpdateDateTime;
                    break;
                case TermsType.PrivacyPolicy:
                    info = termsUpdateInfo.PrivacyPolicy;
                    key = PreferenceKey.PrivacyPolicyLastUpdateDateTime;
                    break;
            }

            if (info == null)
            {
                loggerService.EndMethod();
                return false;
            }

            var lastUpdateDate = new DateTime();
            if (preferencesService.ContainsKey(key))
            {
                lastUpdateDate = preferencesService.GetValue(key, lastUpdateDate);
            }

            loggerService.Info($"termsType: {termsType}, lastUpdateDate: {lastUpdateDate}, info.UpdateDateTime: {info.UpdateDateTime}");
            loggerService.EndMethod();

            return lastUpdateDate < info.UpdateDateTime;
        }

        public void SaveLastUpdateDate(TermsType termsType, DateTime updateDate)
        {
            loggerService.StartMethod();

            var key = termsType == TermsType.TermsOfService ? PreferenceKey.TermsOfServiceLastUpdateDateTime : PreferenceKey.PrivacyPolicyLastUpdateDateTime;
            preferencesService.SetValue(key, updateDate);

            loggerService.EndMethod();
        }

        public bool IsAllAgreed()
        {
            return preferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime) && preferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime);
        }

        public void RemoveAllUpdateDate()
        {
            loggerService.StartMethod();
            preferencesService.RemoveValue(PreferenceKey.TermsOfServiceLastUpdateDateTime);
            preferencesService.RemoveValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime);
            loggerService.EndMethod();
        }
    }
}
