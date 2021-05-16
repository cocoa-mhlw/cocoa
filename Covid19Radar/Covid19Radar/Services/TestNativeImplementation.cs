/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
	public class TestNativeImplementation : INativeImplementation
	{
		static readonly Random random = new Random();

		Task WaitRandom()
			=> Task.Delay(random.Next(100, 2500));

		public async Task StartAsync()
		{
			await WaitRandom();
			Preferences.Set("fake_enabled", true);
		}

		public async Task StopAsync()
		{
			await WaitRandom();
			Preferences.Set("fake_enabled", false);
		}

		public async Task<bool> IsEnabledAsync()
		{
			await WaitRandom();
			return Preferences.Get("fake_enabled", true);
		}

		public async Task<IEnumerable<TemporaryExposureKey>> GetSelfTemporaryExposureKeysAsync()
		{
			var keys = new List<TemporaryExposureKey>();

			for (var i = 1; i < 14; i++)
				keys.Add(GenerateRandomKey(i));

			await WaitRandom();

			return keys;
		}

		public Task<Status> GetStatusAsync()
			=> Task.FromResult(Preferences.Get("fake_enabled", true) ? Status.Active : Status.Disabled);

		private ushort[] DataPreset(int dataVer)
		{
			/* DataPreset for ExposureDetection
			   0(default): two low-risk matches (default for v1.2.3)
			   1: one high-risk match and 2 low-risk matches 
			   2: no match
			   3+: please add
			 */
			switch (dataVer)
			{
				case 1:
					return (
						new ushort[] {10, 3, 27, 0, 0, // ExposureDetectionSummary
									   13, 15, 65, 27, (ushort)RiskLevel.High, // ExposureInfo 1st
									   10, 15, 65, 5, (ushort)RiskLevel.Medium, // ExposureInfo 2st
									   11,  5, 40, 3, (ushort)RiskLevel.Low, // ExposureInfo  3nd
						});
				case 2:
					return (
						new ushort[] {0, 0, 0, 0, 0, // ExposureDetectionSummary
						});
				case 0:
				default:
					return (
						new ushort[] {10, 2, 5, 0, 0, // ExposureDetectionSummary
									   10, 15, 65, 5, (ushort)RiskLevel.Medium, // ExposureInfo 1st (RiskLevel.Medium=4)
									   11,  5, 40, 3, (ushort)RiskLevel.Low, // ExposureInfo  2nd(RiskLevel.Low=2)
						});
			}
		}

		public string[] UrlApi()
		{
			// "UrlApi" -> UrlApi=
			// ".../api1/register1/" -> UrlApi=
			string url = AppSettings.Instance.ApiUrlBase;
			Regex r = new Regex("/r(egister)?[0-9]+");
			Regex d = new Regex("/d(iagnosis)?[0-9]+");
			string urlRegister = r.Match(url).Value;
			url = r.Replace(url, "");
			string urlDiagnosis = d.Match(url).Value;
			url = d.Replace(url, "");
			string urlApi = url;
			return (new string[] { urlApi, urlRegister, urlDiagnosis });
		}

		public ushort NumberEndofSentence(string url)
		{
			Match match = Regex.Match(url, @"(?<d>\d+)$");
			ushort dataVer = 0;
			if (match.Success)
			{
				dataVer = Convert.ToUInt16(match.Groups["d"].Value);
			}
			return (dataVer);
		}

		private ushort[] Data()
		{
			string url = UrlApi()[0];
			if (Regex.IsMatch(url, @"^(\d+,)+\d+,?$"))
			{
				return (url.Split(",").ToList().Select(x => Convert.ToUInt16(x)).ToArray());
			}
			return (DataPreset(NumberEndofSentence(url)));

		}

		public Task<(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getInfo)> DetectExposuresAsync(IEnumerable<string> files)
		{
			/* ApiUrlBase trick for Debug_Mock
			"10,2,5,0,0,10,15,65,5,4,11,5,40,3,2" -> direct input (the same with default)
			"https://API_URL_BASE/api2" -> dataVer = 2
			"https://API_URL_BASE/api" -> dataVer = 0 (default)
			others -> dataVer is the number at the end of the sentence
			*/
			var d = Data();
			int i = 0;
			var summary = new ExposureDetectionSummary(d[i++], d[i++], d[i++], new TimeSpan[d[i++]], d[i++]);

			Task<IEnumerable<ExposureInfo>> GetInfo()
			{
				var info = new List<ExposureInfo>();
				while (i < d.Length)
				{
					info.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-d[i++]), TimeSpan.FromMinutes(d[i++]), d[i++], d[i++], (Xamarin.ExposureNotifications.RiskLevel)d[i++]));
				};
				return Task.FromResult<IEnumerable<ExposureInfo>>(info);
			}

			return Task.FromResult<(ExposureDetectionSummary, Func<Task<IEnumerable<ExposureInfo>>>)>((summary, GetInfo));
		}

		static TemporaryExposureKey GenerateRandomKey(int daysAgo)
		{
			var buffer = new byte[16];
			random.NextBytes(buffer);

			return new TemporaryExposureKey(
				buffer,
				DateTimeOffset.UtcNow.AddDays(-1 * daysAgo),
				TimeSpan.FromMinutes(random.Next(5, 120)),
				(RiskLevel)random.Next(1, 8));
		}
	}
}
