using System;
using Covid19Radar.Model;
using Covid19Radar.iOS.Model;
using Xamarin.Forms;
using CoreBluetooth;
using CoreLocation;
using Foundation;
using System.ComponentModel;

[assembly: Dependency(typeof(Covid19Radar.iOS.Model.iBeaconService))]
namespace Covid19Radar.iOS.Model
{
    public class iBeaconService : IBeaconService
    {
        #region Variables

        /// <summary>
        /// BLEの発信を制御するクラスのインスタンス
        /// </summary>
        private CBPeripheralManager _peripheralManager = new CBPeripheralManager();

        public event EventHandler<ListChangedEventArgs> ListChanged;
        public event EventHandler DataClearing;

        #endregion

        #region Public methods

        /// <summary>
        /// 端末がBLEの発信に対応しているかどうかをチェックする処理
        /// </summary>
        /// <returns><c>true</c>, 発信可能, <c>false</c> 発信不可</returns>
        public bool TransmissionSupported()
        {
            return true;
        }

        /// <summary>
        /// iBeacon発信開始処理（引数で渡されたiBeaconの定義に従い発信）
        /// </summary>
        /// <param name="ibeacon">iBeaconの定義</param>
        public void StartTransmission(iBeacon ibeacon)
        {
            this.StartTransmission(ibeacon.Uuid, ibeacon.Major, ibeacon.Minor, ibeacon.TxPower);
        }

        /// <summary>
        /// iBeacon発信開始処理（UUID、Major、Minorを直指定して発信）
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        public void StartTransmission(Guid uuid, ushort major, ushort minor, sbyte txPower)
        {
            // BLE発信制御クラスに渡すためのiBeacon定義を作成する。
            NSUuid nsUuid = new NSUuid(uuid.ToString());
            CLBeaconRegion region = new CLBeaconRegion(nsUuid, major, minor, uuid.ToString());
            NSNumber nsnumTxPower = new NSNumber(txPower);
            NSDictionary peripheralData = region.GetPeripheralData(nsnumTxPower);

            // iBeaconの発信を開始する。
            _peripheralManager.StartAdvertising(peripheralData);
        }

        /// <summary>
        /// iBeaconの発信を停止する処理
        /// </summary>
        public void StopTransmission()
        {
            _peripheralManager.StopAdvertising();
        }

        public void InitializeService()
        {
            throw new NotImplementedException();
        }

        public void StartRanging()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}