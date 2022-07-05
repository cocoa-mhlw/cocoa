// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
    public class TermsUpdateServiceMock : ITermsUpdateService
    {
        public Task<TermsUpdateInfoModel> GetTermsUpdateInfo()
        {
            return Task.FromResult(new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail
                {
                    Text = "Mock: Terms of service",
                    UpdateDateTimeJst = DateTime.MinValue
                },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail
                {
                    Text = "Mock: Privacy policy",
                    UpdateDateTimeJst = DateTime.MinValue
                }
            });
        }

        private bool _termsOfServiceUpdated = false;
        private bool _privacyPolicyUpdated = false;

        public bool IsUpdated(TermsType termsType, TermsUpdateInfoModel termsUpdateInfo)
        {
            bool result = false;
            switch (termsType)
            {
                case TermsType.TermsOfService:
                    if (_termsOfServiceUpdated)
                    {
                        _termsOfServiceUpdated = false;
                        result = true;
                    }
                    break;
                case TermsType.PrivacyPolicy:
                    if (_privacyPolicyUpdated)
                    {
                        _privacyPolicyUpdated = false;
                        result = true;
                    }
                    break;
            }
            return result;
        }
    }
}

