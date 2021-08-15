using Chino;

namespace Covid19Radar.Services
{
    public interface IExposureRiskCalculationService
    {
        RiskLevel CalcRiskLevel(DailySummary dailySummary);
    }

    public class ExposureRiskCalculationService : IExposureRiskCalculationService
    {
        // TODO:  We should make consideration later.
        public RiskLevel CalcRiskLevel(DailySummary dailySummary)
            => RiskLevel.High;
    }
}
