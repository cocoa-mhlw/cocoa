/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using System;
using System.Net;
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
        private readonly IHttpDataService httpDataService;
        private readonly IUserDataRepository userDataRepository;

        public UserDataService(
            IHttpDataService httpDataService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            )
        {
            this.httpDataService = httpDataService;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
        }

        public async Task<HttpStatusCode> RegisterUserAsync()
        {
            loggerService.StartMethod();
            try
            {
                var resultStatusCode = await httpDataService.PostRegisterUserAsync();

                if (resultStatusCode == HttpStatusCode.OK)
                {
                    loggerService.Info("Success register");
                    userDataRepository.SetStartDate(DateTime.UtcNow);
                }
                else
                {
                    loggerService.Info("Failed register");
                }

                loggerService.EndMethod();
                return resultStatusCode;
            }
            catch(Exception ex)
            {
                loggerService.Exception("Failed to register user.", ex);
                loggerService.EndMethod();
                throw ex;
            }

        }
    }
}
