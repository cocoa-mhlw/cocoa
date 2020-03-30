using System;
using SQLite;


namespace Covid19Radar.Shiny.Models
{
    public class BeaconEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Uuid { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public bool Entered { get; set; }
        public DateTime Date { get; set; }
    }
}
