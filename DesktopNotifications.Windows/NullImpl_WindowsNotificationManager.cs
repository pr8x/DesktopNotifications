using System;
using System.Threading.Tasks;

#pragma warning disable CS0067

namespace DesktopNotifications.Windows
{
    internal class WindowsApplicationContext : ApplicationContext
    {
        public static WindowsApplicationContext FromCurrentProcess(
            string? customName = null,
            string? appUserModelId = null)
        {
        }
    }

    internal class WindowsNotificationManager : INotificationManager
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string? LaunchActionId { get; }

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null)
        {
            throw new NotImplementedException();
        }

        public Task ScheduleNotification(Notification notification, DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null)
        {
            throw new NotImplementedException();
        }
    }
}