using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Shiny.Settings
{
    public interface IAppSettings
    {
        bool IsChecked { get; set; }
        string YourText { get; set; }
        DateTime? LastUpdated { get; set; }

        bool UseNotificationsBle { get; set; }
        bool UseNotificationsGeofenceEntry { get; set; }
        bool UseNotificationsGeofenceExit { get; set; }
        bool UseNotificationsJobStart { get; set; }
        bool UseNotificationsJobFinish { get; set; }
        bool UseNotificationsHttpTransfers { get; set; }
        bool UseNotificationsBeaconRegionEntry { get; set; }
        bool UseNotificationsBeaconRegionExit { get; set; }
    }
}
