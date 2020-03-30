using SQLite;
using System;


namespace Covid19Radar.Shiny.Models
{
    public class PushEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
