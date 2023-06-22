namespace DesktopNotifications
{
    /// <summary>
    /// </summary>
    public class NotificationActivatedEventArgs : NotificationEventArgs
    {
        public NotificationActivatedEventArgs(Notification notification, string actionId)
            : base(notification)
        {
            ActionId = actionId;
        }

        /// <summary>
        /// The id associated with the activation action. "default" denotes the platform-specific default action.
        /// On Windows this means the user clicked on the notification.
        /// </summary>
        public string ActionId { get; }
    }
}