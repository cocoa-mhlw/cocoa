using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Covid19Radar.Model
{
	public interface IIBeaconService
	{
		// Receiver
		void InitializeService();
//		void StartMonitoring();
		void StartRanging();

		event EventHandler<ListChangedEventArgs> ListChanged;
		event EventHandler DataClearing;


		// Transmitter
		bool TransmissionSupported();
		void StartTransmission(iBeacon ibeacon);
		void StartTransmission(Guid uuid, ushort major, ushort minor, sbyte txPower);
		void StopTransmission();
	}
}
