using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Foundation;
using SQLite;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Covid19Radar.iOS.Services.SQLiteConnectionProvider))]
namespace Covid19Radar.iOS.Services
{
    public class SQLiteConnectionProvider : ISQLiteConnectionProvider
    {
        private SQLiteConnection Connection { get; set; }
        public SQLiteConnection GetConnection()
        {
            if (this.Connection != null) { return this.Connection; }

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = Path.Combine(path, "..", "Library", AppConstants.SqliteFilename);
            return this.Connection = new SQLiteConnection(path);
        }
    }
}