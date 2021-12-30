/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IExposureRiskCalculationService
    {
        RiskLevel CalcRiskLevel(DailySummary dailySummary);
        Task<bool> HasContact();
    }

    public class ExposureRiskCalculationService : IExposureRiskCalculationService
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILoggerService _loggerService;

        public ExposureRiskCalculationService(
            IUserDataRepository userDataRepository,
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
        }

        public async Task<bool> HasContact()
        {
            _loggerService.StartMethod();

            var dailySummaryList = await _userDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
            var userExposureInformationList = _userDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            var count = dailySummaryList.Count() + userExposureInformationList.Count();

            _loggerService.Info($"Exposure count: {count}");
            _loggerService.EndMethod();

            return (count > 0); 
        }

        // TODO:  We should make consideration later.
        public RiskLevel CalcRiskLevel(DailySummary dailySummary)
            => RiskLevel.High;


    }
}
