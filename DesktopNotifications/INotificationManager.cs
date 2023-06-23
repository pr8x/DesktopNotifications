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
        /// The action identifier the process was launched with.
        /// <remarks>
        /// "default" denotes the platform-specific default action.
        /// On Windows this means the user simply clicked the notification body.
        /// </remarks>
        /// </summary>
        string? LaunchActionId { get; }

        /// <summary>
        /// Retrieve the capabilities of the notification manager (and its respective platform backend)
        /// </summary>
        NotificationManagerCapabilities Capabilities { get; }

        /// <summary>
        /// Raised when a notification was activated. The notion of "activation" varies from platform to platform.
        /// </summary>
        event EventHandler<NotificationActivatedEventArgs> NotificationActivated;

        /// <summary>
        /// Raised when a notification was dismissed. The exact reason can be found in
        /// <see cref="NotificationDismissedEventArgs" />.
        /// </summary>
        event EventHandler<NotificationDismissedEventArgs> NotificationDismissed;

        /// <summary>
        /// Initialized the notification manager.
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        /// Schedules a notification for delivery.
        /// </summary>
        /// <param name="notification">The notification to present.</param>
        /// <param name="expirationTime">The expiration time marking the point when the notification gets removed.</param>
        Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null);

        /// <summary>
        /// Hides an already delivered notification (if possible).
        /// If the notification is scheduled for delivery the schedule will be cancelled.
        /// </summary>
        /// <param name="notification">The notification to hide</param>
        Task HideNotification(Notification notification);

        /// <summary>
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="expirationTime"></param>
        /// <returns></returns>
        Task ScheduleNotification(
            Notification notification,
            DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null);
    }
}