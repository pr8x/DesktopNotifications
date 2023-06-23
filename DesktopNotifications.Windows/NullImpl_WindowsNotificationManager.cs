using System;
using System.Threading.Tasks;

#pragma warning disable CS0067

namespace DesktopNotifications.Windows
{
    public class WindowsApplicationContext : ApplicationContext
    {
        public static WindowsApplicationContext FromCurrentProcess(
            string? customName = null,
            string? appUserModelId = null)
        {
            throw new PlatformNotSupportedException();
        }

        public WindowsApplicationContext(string name) : base(name)
        {
            throw new PlatformNotSupportedException();
        }
    }

    public class WindowsNotificationManager : INotificationManager
    {
        public WindowsNotificationManager(WindowsApplicationContext? context = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void Dispose()
        {
            throw new PlatformNotSupportedException();
        }

        public string? LaunchActionId { get; }

        public NotificationManagerCapabilities Capabilities => NotificationManagerCapabilities.None;

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public Task Initialize()
        {
            throw new PlatformNotSupportedException();
        }

        public Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null)
        {
            throw new PlatformNotSupportedException();
        }

        public Task HideNotification(Notification notification)
        {
            throw new PlatformNotSupportedException();
        }

        public Task ScheduleNotification(Notification notification, DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null)
        {
            throw new PlatformNotSupportedException();
        }
    }
}