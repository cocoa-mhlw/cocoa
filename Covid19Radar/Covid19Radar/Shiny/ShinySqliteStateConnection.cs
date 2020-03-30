using Covid19Radar.Shiny.Models;
using Shiny.IO;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Covid19Radar.Shiny
{
    public class ShinySqliteStateConnection : SQLiteAsyncConnection
    {
        public ShinySqliteStateConnection(IFileSystem fileSystem) : base(Path.Combine(fileSystem.AppData.FullName, Constants.ShinyStateDBName))
        {
            var conn = this.GetConnection();

            conn.CreateTable<AppStateEvent>();
            conn.CreateTable<BeaconEvent>();
            conn.CreateTable<GeofenceEvent>();
            conn.CreateTable<JobLog>();
            conn.CreateTable<BleEvent>();
            conn.CreateTable<GpsEvent>();
            conn.CreateTable<HttpEvent>();
            conn.CreateTable<NotificationEvent>();
            conn.CreateTable<PushEvent>();
        }

        public AsyncTableQuery<AppStateEvent> AppStateEvents => this.Table<AppStateEvent>();
        public AsyncTableQuery<BeaconEvent> BeaconEvents => this.Table<BeaconEvent>();
        public AsyncTableQuery<BleEvent> BleEvents => this.Table<BleEvent>();
        public AsyncTableQuery<GeofenceEvent> GeofenceEvents => this.Table<GeofenceEvent>();
        public AsyncTableQuery<JobLog> JobLogs => this.Table<JobLog>();
        public AsyncTableQuery<GpsEvent> GpsEvents => this.Table<GpsEvent>();
        public AsyncTableQuery<HttpEvent> HttpEvents => this.Table<HttpEvent>();
        public AsyncTableQuery<NotificationEvent> NotificationEvents => this.Table<NotificationEvent>();
        public AsyncTableQuery<PushEvent> PushEvents => this.Table<PushEvent>();
    }

}
