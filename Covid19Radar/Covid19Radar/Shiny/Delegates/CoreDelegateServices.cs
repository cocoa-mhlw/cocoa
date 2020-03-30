using Covid19Radar.Shiny;
using Covid19Radar.Shiny.Models;
using Covid19Radar.Shiny.Settings;
using Shiny;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Covid19Radar.Shiny.ShinyAppStartup;

namespace Covid19Radar.Shiny.Delegates
{
    public class CoreDelegateServices
    {
        public CoreDelegateServices(ShinySqliteStateConnection conn,
                                    INotificationManager notifications,
                                    IAppSettings appSettings)
        {
            this.Connection = conn;
            this.Notifications = notifications;
            this.AppSettings = appSettings;
        }


        public ShinySqliteStateConnection Connection { get; }
        public INotificationManager Notifications { get; }
        public IAppSettings AppSettings { get; }


        public async Task SendNotification(string title, string message, Expression<Func<IAppSettings, bool>>? expression = null)
        {
            var notify = expression == null
                ? true
                : this.AppSettings.ReflectGet(expression);

            if (notify)
                await this.Notifications.Send(title, message);
        }
    }
}
