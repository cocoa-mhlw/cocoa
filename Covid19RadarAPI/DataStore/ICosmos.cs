using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataStore
{
    public interface ICosmos
    {
        Task CreateDatabaseAsync(string databaseId);
    }
}
