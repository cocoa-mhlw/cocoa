using Acr.UserDialogs.Forms;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Shiny;
using Shiny.Beacons;
using Shiny.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IBeaconManager
    {

        // ナビゲーション
        protected INavigationService NavigationService { get; private set; }
        // Shiny接続
        protected IConnectivity ConnectivityService { get; private set; }

        protected IUserDialogs UserDialogs { get; private set; }

        protected IBeaconManager BeaconManager { get; private set; }

        // ページタイトル
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        public ViewModelBase(INavigationService navigationService, IConnectivity connectivityService, IUserDialogs dialogs, IBeaconManager beaconManager)
        {
            NavigationService = navigationService;
            ConnectivityService = connectivityService;
            UserDialogs = dialogs;
            BeaconManager = beaconManager;

        }
        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }


        public AccessState GetCurrentStatus(bool forMonitoring)
        {
            return BeaconManager.GetCurrentStatus(forMonitoring);
        }

        public IObservable<AccessState> WhenAccessStatusChanged(bool monitoring)
        {
            return BeaconManager.WhenAccessStatusChanged(monitoring);
        }

        public Task<AccessState> RequestAccess(bool monitoring)
        {
            return BeaconManager.RequestAccess(monitoring);
        }

        public Task<IEnumerable<BeaconRegion>> GetMonitoredRegions()
        {
            return BeaconManager.GetMonitoredRegions();
        }

        public IObservable<Beacon> WhenBeaconRanged(BeaconRegion region)
        {
            return BeaconManager.WhenBeaconRanged(region);
        }

        public Task StartMonitoring(BeaconRegion region)
        {
            return BeaconManager.StartMonitoring(region);
        }

        public Task StopMonitoring(string identifier)
        {
            return BeaconManager.StopMonitoring(identifier);
        }

        public Task StopAllMonitoring()
        {
            return BeaconManager.StopAllMonitoring();
        }
    }
}
