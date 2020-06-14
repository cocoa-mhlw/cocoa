using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    public interface ISQLiteConnectionProvider
    {
        SQLiteConnection GetConnection();
    }
}
