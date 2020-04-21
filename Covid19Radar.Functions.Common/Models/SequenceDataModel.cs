using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class SequenceDataModel
    {

        public static Guid _id = new Guid("191A64D7-1226-497C-9C6B-8FE941086559");
        /// <summary>
        /// Id
        /// </summary>
        public string id { get => _id.ToString(); }

        /// <summary>
        /// User Major 0 to 65536
        /// </summary>
        /// <value>User Major</value>
        public int Major { get; set; }

        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
        public int Minor { get; set; }

        /// <summary>
        /// Etag only use cosmos DB
        /// </summary>
        public string _etag { get; set; }

        public bool IsMajorMax()
        {
            return Major >= 65536;
        }

        public bool IsMinorMax()
        {
            return Minor >= 65536;
        }

        public void Increment()
        {
            if (IsMinorMax())
            {
                Minor = 0;
                Major++;
            }
            else
            {
                Minor++;
            }
        }
    }
}
