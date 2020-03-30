using System;
using SQLite;


namespace Covid19Radar.Shiny.Models
{
    public class BleEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
