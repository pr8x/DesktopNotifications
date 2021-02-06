using System;
using System.Threading.Tasks;

namespace DesktopNotifications.Apple
{
    public class AppleNotificationManager : INotificationManager
    {
        public void Dispose()
        {
        }

        public event EventHandler<NotificationActivatedEventArgs> NotificationActivated;
        public event EventHandler<NotificationDismissedEventArgs> NotificationDismissed;

        public ValueTask Initialize()
        {
            return default;
        }

        public ValueTask ShowNotification(Notification notification, DateTimeOffset? expirationTime = null)
        {
            return default;
        }
    }
}
