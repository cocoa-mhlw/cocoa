using Covid19Radar.Shiny;
using Covid19Radar.Shiny.Models;
using Shiny;
using Shiny.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Shiny.Delegates
{
    public class NotificationDelegate : INotificationDelegate
    {
        readonly ShinySqliteStateConnection conn;
        readonly IMessageBus messageBus;
        readonly INotificationManager notifications;


        public NotificationDelegate(ShinySqliteStateConnection conn, IMessageBus messageBus, INotificationManager notifications)
        {
            this.conn = conn;
            this.messageBus = messageBus;
            this.notifications = notifications;
        }


        public Task OnEntry(NotificationResponse response) => this.Store(new NotificationEvent
        {
            NotificationId = response.Notification.Id,
            NotificationTitle = response.Notification.Title ?? response.Notification.Message,
            Action = response.ActionIdentifier,
            ReplyText = response.Text,
            IsEntry = true,
            Timestamp = DateTime.Now
        });


        public Task OnReceived(Notification notification) => this.Store(new NotificationEvent
        {
            NotificationId = notification.Id,
            NotificationTitle = notification.Title ?? notification.Message,
            IsEntry = false,
            Timestamp = DateTime.Now
        });


        async Task Store(NotificationEvent @event)
        {
            await this.conn.InsertAsync(@event);
            this.messageBus.Publish(@event);
        }
    }
}
