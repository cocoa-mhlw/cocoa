using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Covid19Radar.Api.Services
{
    public interface ITemporaryExposureKeyValidationService
    {
        public bool Validate(bool hasSymptom, V3DiagnosisSubmissionParameter.Key key);
    }

    public class TemporaryExposureKeyValidationService : ITemporaryExposureKeyValidationService
    {
        private readonly int _infectiousFilterDaysSinceOnsetOfSymptomsFrom;
        private readonly int _infectiousFilterDaysSinceOnsetOfSymptomsTo;

        private readonly int _infectiousFilterDaysSinceTestFrom;
        private readonly int _infectiousFilterDaysSinceTestTo;

        private readonly ILogger<TemporaryExposureKeyValidationService> _logger;

        public TemporaryExposureKeyValidationService(
            IConfiguration configuration,
            ILogger<TemporaryExposureKeyValidationService> logger
            )
        {
            _infectiousFilterDaysSinceOnsetOfSymptomsFrom = configuration.InfectiousFilterDaysSinceOnsetOfSymptomsFrom();
            _infectiousFilterDaysSinceOnsetOfSymptomsTo = configuration.InfectiousFilterDaysSinceOnsetOfSymptomsTo();

            _infectiousFilterDaysSinceTestFrom = configuration.InfectiousFilterDaysSinceTestFrom();
            _infectiousFilterDaysSinceTestTo = configuration.InfectiousFilterDaysSinceTestTo();

            _logger = logger;
        }

        public bool Validate(bool hasSymptom, V3DiagnosisSubmissionParameter.Key key)
        {
            if (string.IsNullOrWhiteSpace(key.KeyData))
            {
                _logger.LogWarning("key.KeyData is null or whiteSpace.");
                return false;
            }

            // The devices generate the 16-byte Temporary Exposure Key.
            // https://blog.google/documents/69/Exposure_Notification_-_Cryptography_Specification_v1.2.1.pdf/
            try
            {
                byte[] keyDataBytes = Convert.FromBase64String(key.KeyData);
                if (keyDataBytes.Length != 16)
                {
                    _logger.LogWarning("key.KeyData length must be 16 bytes.");
                    return false;
                }
            }
            catch (FormatException)
            {
                _logger.LogError($"Convert.FromBase64String FormatException occurred. {key.KeyData}");
                return false;
            }
            catch (Exception)
            {
                _logger.LogError($"Convert.FromBase64String Exception occurred. {key.KeyData}");
                return false;
            }

            if (key.RollingPeriod > Constants.ActiveRollingPeriod)
            {
                _logger.LogWarning($"key.RollingPeriod must be less or equal 144 but {key.RollingPeriod}");
                return false;
            }

            // https://developer.apple.com/documentation/exposurenotification/enexposureinfo/3583716-transmissionrisklevel
            if (key.TransmissionRisk < 0 || key.TransmissionRisk > 7)
            {
                _logger.LogWarning($"key.RollingPeriod must be int 0 to 7 but {key.TransmissionRisk}");
                return false;
            }

            // https://developer.apple.com/documentation/exposurenotification/endiagnosisreporttype
            if (key.ReportType < 0 || key.ReportType > 5)
            {
                _logger.LogWarning($"key.ReportType must be int 0 to 5 but {key.ReportType}");
                return false;
            }

            var dateTime = DateTime.UtcNow.Date;
            var todayRollingStartNumber = dateTime.ToRollingStartNumber();

            var oldestRollingStartNumber = dateTime.AddDays(Constants.OutOfDateDays).ToRollingStartNumber();

            if (key.RollingStartNumber < oldestRollingStartNumber)
            {
                _logger.LogWarning("key.RollingStartNumber must be a date newer than 14 days ago");
                return false;
            }
            if (key.RollingStartNumber > todayRollingStartNumber)
            {
                _logger.LogWarning("key.RollingStartNumber must be a date older than today");
                return false;
            }

            if (key.DaysSinceOnsetOfSymptoms < Constants.MIN_DAYS_SINCE_ONSET_OF_SYMPTOMS
                || key.DaysSinceOnsetOfSymptoms > Constants.MAX_DAYS_SINCE_ONSET_OF_SYMPTOMS
                )
            {
                _logger.LogWarning("key.DaysSinceOnsetOfSymptoms must be in -14 to 14 but {key.DaysSinceOnsetOfSymptoms}");
                return false;
            }

            if (hasSymptom)
            {
                _logger.LogDebug("hasSymptom");

                if (_infectiousFilterDaysSinceOnsetOfSymptomsFrom > key.DaysSinceOnsetOfSymptoms
                    || _infectiousFilterDaysSinceOnsetOfSymptomsTo < key.DaysSinceOnsetOfSymptoms
                    )
                {
                    _logger.LogInformation($"key.DaysSinceOnsetOfSymptoms must be in {_infectiousFilterDaysSinceOnsetOfSymptomsFrom} to {_infectiousFilterDaysSinceOnsetOfSymptomsTo} but {key.DaysSinceOnsetOfSymptoms}");
                    return false;
                }
            }
            else
            {
                _logger.LogDebug("diagnosis");

                if (_infectiousFilterDaysSinceTestFrom > key.DaysSinceOnsetOfSymptoms
                    || _infectiousFilterDaysSinceTestTo < key.DaysSinceOnsetOfSymptoms
                    )
                {
                    _logger.LogInformation($"key.DaysSinceOnsetOfSymptoms must be in {_infectiousFilterDaysSinceTestFrom} to {_infectiousFilterDaysSinceTestTo} but {key.DaysSinceOnsetOfSymptoms}");
                    return false;
                }
            }

            return true;
        }
    }
}
