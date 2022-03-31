/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IUserDataService
    {
        Task<HttpStatusCode> RegisterUserAsync();
    }

    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService : IUserDataService
    {
        private readonly ILoggerService loggerService;
        private readonly IHttpClientService httpClientService;
        private readonly IUserDataRepository userDataRepository;
        private readonly IServerConfigurationRepository serverConfigurationRepository;

        public UserDataService(
            IHttpClientService httpClientService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository
            )
        {
            this.httpClientService = httpClientService;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
            this.serverConfigurationRepository = serverConfigurationRepository;
        }

        public async Task<HttpStatusCode> RegisterUserAsync()
        {
            loggerService.StartMethod();
            try
            {
                await serverConfigurationRepository.LoadAsync();

                string url = serverConfigurationRepository.UserRegisterApiEndpoint;
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                HttpResponseMessage result = await httpClientService.HttpClient.PostAsync(url, content);
                HttpStatusCode resultStatusCode = result.StatusCode;

                if (resultStatusCode == HttpStatusCode.OK)
                {
                    loggerService.Info("Success register");
                    userDataRepository.SetStartDate(DateTime.UtcNow);
                }
                else
                {
                    loggerService.Info("Failed register");
                }

                return resultStatusCode;
            }
            catch(Exception ex)
            {
                loggerService.Exception("Failed to register user.", ex);
                throw;
            }
            finally
            {
                loggerService.EndMethod();
            }

        }
    }

    public class UserDataServiceMock : IUserDataService
    {
        public Task<HttpStatusCode> RegisterUserAsync()
            => Task.FromResult(HttpStatusCode.OK);
    }
}
