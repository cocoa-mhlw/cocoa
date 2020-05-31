using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using ExposureNotifications;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace Xamarin.ExposureNotifications
{
	public static partial class ExposureNotification
	{
		static ENManager manager;

		static async Task<ENManager> GetManagerAsync()
		{
			if (manager == null)
			{
				manager = new ENManager();
				await manager.ActivateAsync();
			}

			return manager;
		}

		static async Task<ENExposureConfiguration> GetConfigurationAsync()
		{
			var c = await Handler.GetConfigurationAsync();

			var nc = new ENExposureConfiguration
			{
				AttenuationLevelValues = c.AttenuationScores,
				DurationLevelValues = c.DurationScores,
				DaysSinceLastExposureLevelValues = c.DaysSinceLastExposureScores,
				TransmissionRiskLevelValues = c.TransmissionRiskScores,
				AttenuationWeight = c.AttenuationWeight,
				DaysSinceLastExposureWeight = c.DaysSinceLastExposureWeight,
				DurationWeight = c.DurationWeight,
				TransmissionRiskWeight = c.TransmissionWeight,
				MinimumRiskScore = (byte)c.MinimumRiskScore,
			};

			if (c.DurationAtAttenuationThresholds != null)
			{
				if (c.DurationAtAttenuationThresholds.Length < 2)
					throw new ArgumentOutOfRangeException(nameof(c.DurationAtAttenuationThresholds), "Must be an array of length 2");

				var nsArr = NSArray.FromObjects(2, c.DurationAtAttenuationThresholds[0], c.DurationAtAttenuationThresholds[1]);
				nc.Metadata ??= new NSMutableDictionary();
				nc.Metadata.SetValueForKey(nsArr, new NSString("attenuationDurationThresholds"));
			}

			return nc;
		}

		static void PlatformInit()
		{
			_ = ScheduleFetchAsync();
		}

		static async Task PlatformStart()
		{
			var m = await GetManagerAsync();
			await m.SetExposureNotificationEnabledAsync(true);
		}

		static async Task PlatformStop()
		{
			var m = await GetManagerAsync();
			await m.SetExposureNotificationEnabledAsync(false);
		}

		static async Task<bool> PlatformIsEnabled()
		{
			var m = await GetManagerAsync();
			return m.ExposureNotificationEnabled;
		}

		static Task PlatformScheduleFetch()
		{
			// This is a special ID suffix which iOS treats a certain way
			// we can basically request infinite background tasks
			// and iOS will throttle it sensibly for us.
			var id = AppInfo.PackageName + ".exposure-notification";

			var isUpdating = false;
			BGTaskScheduler.Shared.Register(id, null, task =>
			{
				// Disallow concurrent exposure detection, because if allowed we might try to detect the same diagnosis keys more than once
				if (isUpdating)
				{
					task.SetTaskCompleted(false);
					return;
				}
				isUpdating = true;

				var cancelSrc = new CancellationTokenSource();
				task.ExpirationHandler = cancelSrc.Cancel;

				// Run the actual task on a background thread
				Task.Run(async () =>
				{
					try
					{
						await UpdateKeysFromServer(cancelSrc.Token);
						task.SetTaskCompleted(true);
					}
					catch (OperationCanceledException)
					{
						Console.WriteLine($"[Xamarin.ExposureNotifications] Background task took too long to complete.");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[Xamarin.ExposureNotifications] There was an error running the background task: {ex}");
						task.SetTaskCompleted(false);
					}

					isUpdating = false;
				});

				scheduleBgTask();
			});

			scheduleBgTask();

			return Task.CompletedTask;

			void scheduleBgTask()
			{
				if (ENManager.AuthorizationStatus != ENAuthorizationStatus.Authorized)
					return;

				var newBgTask = new BGProcessingTaskRequest(id);
				newBgTask.RequiresNetworkConnectivity = true;
				try
				{
					BGTaskScheduler.Shared.Submit(newBgTask, out var error);

					if (error != null)
						throw new NSErrorException(error);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[Xamarin.ExposureNotifications] There was an error submitting the background task: {ex}");
				}
			}
		}

		// Tells the local API when new diagnosis keys have been obtained from the server
		static async Task<(ExposureDetectionSummary, IEnumerable<ExposureInfo>)> PlatformDetectExposuresAsync(IEnumerable<string> keyFiles, CancellationToken cancellationToken)
		{
			// Submit to the API
			var c = await GetConfigurationAsync();
			var m = await GetManagerAsync();

			var detectionSummary = await m.DetectExposuresAsync(
				c,
				keyFiles.Select(k => new NSUrl(k, false)).ToArray(),
				out var detectProgress);
			cancellationToken.Register(detectProgress.Cancel);

			var summary = new ExposureDetectionSummary(
				(int)detectionSummary.DaysSinceLastExposure,
				detectionSummary.MatchedKeyCount,
				detectionSummary.MaximumRiskScore);

			// Get the info
			IEnumerable<ExposureInfo> info = Array.Empty<ExposureInfo>();
			if (summary?.MatchedKeyCount > 0)
			{
				var exposures = await m.GetExposureInfoAsync(detectionSummary, Handler.UserExplanation, out var exposuresProgress);
				cancellationToken.Register(exposuresProgress.Cancel);
				info = exposures.Select(i => new ExposureInfo(
					((DateTime)i.Date).ToLocalTime(),
					TimeSpan.FromMinutes(i.Duration),
					i.AttenuationValue,
					i.TotalRiskScore,
					i.TransmissionRiskLevel.FromNative()));
			}

			// Return everything
			return (summary, info);
		}

		static async Task<IEnumerable<TemporaryExposureKey>> PlatformGetTemporaryExposureKeys()
		{
			var m = await GetManagerAsync();
			var selfKeys = await m.GetDiagnosisKeysAsync();

			return selfKeys.Select(k => new TemporaryExposureKey(
				k.KeyData.ToArray(),
				k.RollingStartNumber,
				TimeSpan.FromMinutes(k.RollingPeriod * 10),
				k.TransmissionRiskLevel.FromNative()));
		}

		static async Task<Status> PlatformGetStatusAsync()
		{
			var m = await GetManagerAsync();

			switch (m.ExposureNotificationStatus)
			{
				case ENStatus.Active:
					return Status.Active;
				case ENStatus.BluetoothOff:
					return Status.BluetoothOff;
				case ENStatus.Disabled:
					return Status.Disabled;
				case ENStatus.Restricted:
					return Status.Restricted;
				case ENStatus.Unknown:
				default:
					return Status.Unknown;
			}
		}
	}

	static partial class Utils
	{
		public static RiskLevel FromNative(this byte riskLevel) =>
			riskLevel switch
			{
				1 => RiskLevel.Lowest,
				2 => RiskLevel.Low,
				3 => RiskLevel.MediumLow,
				4 => RiskLevel.Medium,
				5 => RiskLevel.MediumHigh,
				6 => RiskLevel.High,
				7 => RiskLevel.VeryHigh,
				8 => RiskLevel.Highest,
				_ => RiskLevel.Invalid,
			};

		public static byte ToNative(this RiskLevel riskLevel) =>
			riskLevel switch
			{
				RiskLevel.Lowest => 1,
				RiskLevel.Low => 2,
				RiskLevel.MediumLow => 3,
				RiskLevel.Medium => 4,
				RiskLevel.MediumHigh => 5,
				RiskLevel.High => 6,
				RiskLevel.VeryHigh => 7,
				RiskLevel.Highest => 8,
				_ => 0,
			};
	}
}
