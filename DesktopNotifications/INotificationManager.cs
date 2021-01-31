using System;
using System.Threading.Tasks;

namespace DesktopNotifications
{
    /// <summary>
    /// Interface for notification managers that handle the presentation and lifetime of notifications.
    /// </summary>
    public interface INotificationManager : IDisposable
    {
        /// <summary>
        /// Raised when a notification was activated. The notion of "activation" varies from platform to platform.
        /// </summary>
        event EventHandler<NotificationActivatedEventArgs> NotificationActivated;

        /// <summary>
        /// Raised when a notification was dismissed. The exact reason can be found in <see cref="NotificationDismissedEventArgs"/>.
        /// </summary>
        event EventHandler<NotificationDismissedEventArgs> NotificationDismissed;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ValueTask Initialize();

        /// <summary>
        /// Schedules a notification for presentation.
        /// </summary>
        /// <param name="notification">The notification to present.</param>
        /// <param name="expirationTime">The expiration time marking the point when the notification gets removed.</param>
        ValueTask ShowNotification(Notification notification, DateTimeOffset? expirationTime = null);
    }
}