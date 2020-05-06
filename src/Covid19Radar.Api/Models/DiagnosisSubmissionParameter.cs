using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
	public class DiagnosisSubmissionParameter : IUser
	{
		[JsonProperty("submissionNumber")]
		public string SubmissionNumber { get; set; }
		[JsonProperty("userUuid")]
		public string UserUuid { get; set; }
        /// <summary>
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        [JsonProperty("userMajor")]
        public string UserMajor { get; set; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        [JsonProperty("userMinor")]
        public string UserMinor { get; set; }

        [JsonProperty("keys")]
		public TemporaryExposureKey[] Keys { get; set; }
        string IUser.Major => UserMajor;
        string IUser.Minor => UserMinor;

    }
}
