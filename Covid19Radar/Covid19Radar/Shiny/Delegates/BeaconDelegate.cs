using Covid19Radar.Shiny;
using Covid19Radar.Shiny.Models;
using Shiny.Beacons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Shiny.Delegates
{
    public class BeaconDelegate : IBeaconDelegate
    {

        readonly CoreDelegateServices services;
        public BeaconDelegate(CoreDelegateServices services) => this.services = services;

        async Task IBeaconDelegate.OnStatusChanged(BeaconRegionState newStatus, BeaconRegion region)
        {
            await this.services.Connection.InsertAsync(new BeaconEvent
            {
                Identifier = region.Identifier,
                Uuid = region.Uuid.ToString(),
                Major = region.Major,
                Minor = region.Minor,
                Entered = newStatus == BeaconRegionState.Entered,
                Date = DateTime.Now
            });
            var notify = newStatus == BeaconRegionState.Entered
                ? this.services.AppSettings.UseNotificationsBeaconRegionEntry
                : this.services.AppSettings.UseNotificationsBeaconRegionExit;

            if (notify)
            {
                await this.services.SendNotification(
                    $"Beacon Region {newStatus}",
                    $"{region.Identifier} - {region.Uuid}/{region.Major}/{region.Minor}"
                );
            }
        }
    }
}
