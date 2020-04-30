using Covid19Radar.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    /// <summary>
    /// Infection Model
    /// </summary>
    public class InfectionModel : IUser
    {
        /// <summary>
        /// for Cosmos DB
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// PartitionKey
        /// </summary>
        public string PartitionKey { get; set; }
     
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }
   
        /// <summary>
        /// User Major 0 to 65536
        /// </summary>
        /// <value>User Major</value>
        public string Major { get; set; }
     
        /// <summary>
        /// User Minor 0 to 65536
        /// </summary>
        /// <value>User Minor</value>
        public string Minor { get; set; }

        /// <summary>
        ///  Status Contacted,OnSet,Suspected,Inspection,Infection
        /// </summary>
        /// <value></value>
        public UserStatus Status { get; set; }
      
        /// <summary>
        /// Confirmation date for status change.
        /// </summary>
        public DateTime ConfirmationDate { get; set; }
       
        /// <summary>
        /// Impact start date
        /// </summary>
        public DateTime ImpactStart { get; set; }
       
        /// <summary>
        /// Impact end date
        /// </summary>
        public DateTime ImpactEnd { get; set; }
        
        public DateTime Created { get; set; }
        
        public string _etag { get; set; }
    }
}
