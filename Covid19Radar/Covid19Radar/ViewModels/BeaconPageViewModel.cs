using Covid19Radar.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UniversalBeacon.Library.Core.Entities;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;
using Prism.Ioc;
using Prism.DryIoc;

namespace Covid19Radar.ViewModels
{
    public class BeaconPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private INavigationService _navigationService;

        //public event PropertyChangedEventHandler PropertyChanged;

        private BeaconService _service;
        public ObservableCollection<Beacon> Beacons => _service?.Beacons;
        private Beacon _selectedBeacon;

        public async Task RequestPermissions()
        {
            await RequestLocationPermission();
        }

        private async Task RequestLocationPermission()
        {
            // Actually coarse location would be enough, the plug-in only provides a way to request fine location
            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var requestedPermissionStatus = requestedPermissions[Permission.Location];
            Debug.WriteLine("Location permission status: " + requestedPermissionStatus);
            if (requestedPermissionStatus == PermissionStatus.Granted)
            {
                Debug.WriteLine("Starting beacon service...");
                StartBeaconService();
            }
        }


        private void StartBeaconService()
        {
            IContainerProvider container = App.Current.Container;
            _service = container.Resolve<BeaconService>();
            /*
            var provider = container.Resolve<BeaconService>();
            if (_service == null)
            {
                _service = container.Get.Services.AddNew<BeaconService>();
                if (_service.Beacons != null) _service.Beacons.CollectionChanged += Beacons_CollectionChanged;
            }
            */
        }

        private void Beacons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine($"Beacons_CollectionChanged {sender} e {e}");
        }


        public Beacon SelectedBeacon
        {
            get => _selectedBeacon;
            set
            {
                _selectedBeacon = value;
                //PropertyChanged.Fire(this, "SelectedBeacon");
            }
        }

        public BeaconPageViewModel(INavigationService navigationService)
            : base(navigationService)

        {
            _navigationService = navigationService;
            Title = "ビーコンテスト";
        }
    }
}
