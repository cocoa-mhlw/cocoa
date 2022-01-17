// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    [JsonObject]
    public class V1ExposureRiskCalculationConfiguration
    {
        /// <summary>
        /// Specifies the format version of configuration file.
        /// If any events will be occurred that require migration process.(e.g. Add/delete member, change structure, or rename key)
        /// This value will be increment.
        /// </summary>
        [JsonProperty("format_version")]
        public int FormatVersion = 1;

        [JsonProperty("DailySummary_DaySummary_ScoreSum")]
        public Threshold DailySummary_DaySummary_ScoreSum { get; set; } = new Threshold();

        [JsonProperty("DailySummary_DaySummary_WeightedDurationAverage")]
        public Threshold DailySummary_WeightedDurationAverage { get; set; } = new Threshold();

        [JsonProperty("ExposureWindow_ScanInstance_SecondsSinceLastScanSum")]
        public Threshold ExposureWindow_ScanInstance_SecondsSinceLastScanSum { get; set; } = new Threshold();

        [JsonProperty("ExposureWindow_ScanInstance_TypicalAttenuationDb_Max")]
        public Threshold ExposureWindow_ScanInstance_TypicalAttenuationDb_Max { get; set; } = new Threshold();

        [JsonProperty("ExposureWindow_ScanInstance_TypicalAttenuationDb_Min")]
        public Threshold ExposureWindow_ScanInstance_TypicalAttenuationDb_Min { get; set; } = new Threshold();

        public override bool Equals(object obj)
        {
            return obj is V1ExposureRiskCalculationConfiguration configuration &&
                   FormatVersion == configuration.FormatVersion &&
                   EqualityComparer<Threshold>.Default.Equals(DailySummary_DaySummary_ScoreSum, configuration.DailySummary_DaySummary_ScoreSum) &&
                   EqualityComparer<Threshold>.Default.Equals(DailySummary_WeightedDurationAverage, configuration.DailySummary_WeightedDurationAverage) &&
                   EqualityComparer<Threshold>.Default.Equals(ExposureWindow_ScanInstance_SecondsSinceLastScanSum, configuration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum) &&
                   EqualityComparer<Threshold>.Default.Equals(ExposureWindow_ScanInstance_TypicalAttenuationDb_Max, configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Max) &&
                   EqualityComparer<Threshold>.Default.Equals(ExposureWindow_ScanInstance_TypicalAttenuationDb_Min, configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Min);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FormatVersion, DailySummary_DaySummary_ScoreSum, DailySummary_WeightedDurationAverage, ExposureWindow_ScanInstance_SecondsSinceLastScanSum, ExposureWindow_ScanInstance_TypicalAttenuationDb_Max, ExposureWindow_ScanInstance_TypicalAttenuationDb_Min);
        }

        public override string ToString()
            => JsonConvert.SerializeObject(this);

        [JsonObject]
        public class Threshold
        {
            public const string OPERATION_NOP = "NOP";

            public const string OPERATION_EQUAL = "=";
            public const string OPERATION_GREATER_EQUAL = ">=";
            public const string OPERATION_GREATER = ">";
            public const string OPERATION_LESS_EQUAL = "<=";
            public const string OPERATION_LESS = "<";

            [JsonProperty("op")]
            public string Op { get; set; } = OPERATION_NOP;

            [JsonProperty("value")]
            public double Value { get; set; } = 0.0;

            public bool Cond(double value)
            {
                return Op switch
                {
                    OPERATION_EQUAL => this.Value == value,
                    OPERATION_GREATER => this.Value < value,
                    OPERATION_GREATER_EQUAL => this.Value <= value,
                    OPERATION_LESS => this.Value > value,
                    OPERATION_LESS_EQUAL => this.Value >= value,
                    _ => true
                };
            }

            public override bool Equals(object obj)
            {
                return obj is Threshold threshold &&
                       Op == threshold.Op &&
                       Value == threshold.Value;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Op, Value);
            }
        }
    }
}
