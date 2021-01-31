namespace DesktopNotifications
{
    /// <summary>
    /// </summary>
    public class NotificationDismissedEventArgs
    {
        public NotificationDismissedEventArgs(Notification notification, NotificationDismissReason reason)
        {
            Notification = notification;
            Reason = reason;
        }

        public Notification Notification { get; }
        public NotificationDismissReason Reason { get; }
    }
}