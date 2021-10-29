// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.RegularExpressions;

namespace Covid19Radar.Common
{
    public class Validator
    {
        /// <summary>
        /// Value pattern of processing-number.
        /// </summary>
        private const string ProcessingNumberRegex = @"\A[0-9]{8}\z";

        public static bool IsValidProcessNumber(string processNumber)
        {
            return Regex.IsMatch(processNumber, ProcessingNumberRegex);
        }
    }
}
