using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UniversalBeacon.Library.Core.Entities;
using UniversalBeacon.Library.Core.Interfaces;
using Xamarin.Forms;
using DryIoc;
using Prism.Ioc;

namespace Covid19Radar.Models
{
    internal class BeaconService : IDisposable
    {
        private readonly BeaconManager _manager;

        public BeaconService()
        {
            IContainerProvider container = App.Current.Container;
            // get the platform-specific provider
            var provider = container.Resolve<IBluetoothPacketProvider>();
            if (null != provider)
            {
                // create a beacon manager, giving it an invoker to marshal collection changes to the UI thread
                _manager = new BeaconManager(provider, Device.BeginInvokeOnMainThread);
                _manager.Start();
#if DEBUG
                _manager.BeaconAdded += _manager_BeaconAdded;
                provider.AdvertisementPacketReceived += Provider_AdvertisementPacketReceived;
#endif // DEBUG
            }
        }

        public void Dispose()
        {
            _manager?.Stop();
        }

        public ObservableCollection<Beacon> Beacons => _manager?.BluetoothBeacons;

#if DEBUG
        void _manager_BeaconAdded(object sender, Beacon e)
        {
            Debug.WriteLine($"_manager_BeaconAdded {sender} Beacon {e}");
        }

        void Provider_AdvertisementPacketReceived(object sender, UniversalBeacon.Library.Core.Interop.BLEAdvertisementPacketArgs e)
        {
            Debug.WriteLine($"Provider_AdvertisementPacketReceived {sender} Beacon {e}");
        }
#endif // DEBUG
    }
}