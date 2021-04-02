/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
			Preferences.Set("fake_enabled", true);
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

		public Task<(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getInfo)> DetectExposuresAsync(IEnumerable<string> files)
		{
			var summary = new ExposureDetectionSummary(10, 2, 5);

			Task<IEnumerable<ExposureInfo>> GetInfo()
			{
				var info = new List<ExposureInfo>
				{
					new ExposureInfo (DateTime.UtcNow.AddDays(-10), TimeSpan.FromMinutes(15), 65, 5, RiskLevel.Medium),
					new ExposureInfo (DateTime.UtcNow.AddDays(-11), TimeSpan.FromMinutes(5), 40, 3, RiskLevel.Low),
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
