using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Covid19Radar.Common;
using Covid19Radar.Model;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(Covid19Radar.Droid.Services.SQLiteConnectionProvider))]
namespace Covid19Radar.Droid.Services
{
    public class SQLiteConnectionProvider : ISQLiteConnectionProvider
    {
        private SQLiteConnection Connection { get; set; }

        public SQLiteConnection GetConnection()
        {
            if (this.Connection != null) { return this.Connection; }

            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = Path.Combine(path, AppConstants.SqliteFilename);
            return this.Connection = new SQLiteConnection(path);
        }
    }
}