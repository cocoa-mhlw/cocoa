using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    class HttpDataServiceMock : IHttpDataService
    {
        Task<Stream> IHttpDataService.GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<Stream>(() => {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKey called");
                return new MemoryStream();
            });
        }

        Task<List<TemporaryExposureKeyExportFileModel>> IHttpDataService.GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<List<TemporaryExposureKeyExportFileModel>>(() => {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList called");
                return new List<TemporaryExposureKeyExportFileModel>();
            });
        }

        async Task<UserDataModel> IHttpDataService.PostRegisterUserAsync()
        {
            Debug.WriteLine("HttpDataServiceMock::PostRegisterUserAsync called");

            UserDataModel userData = new UserDataModel();
            userData.Secret = "dummy secret";
            userData.UserUuid = "dummy uuid";
            userData.JumpConsistentSeed = 999;
            userData.IsOptined = true;
            Application.Current.Properties["Secret"] = userData.Secret;
            await Application.Current.SavePropertiesAsync();
            return userData;
        }

        Task IHttpDataService.PutSelfExposureKeysAsync(SelfDiagnosisSubmission request)
        {
            return Task.Factory.StartNew(() => {
                Debug.WriteLine("HttpDataServiceMock::PutSelfExposureKeysAsync called");
            });
        }
    }
}
