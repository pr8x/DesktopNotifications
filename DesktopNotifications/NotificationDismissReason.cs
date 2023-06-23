namespace DesktopNotifications
{
    /// <summary>
    /// Reasons why a notification was dismissed.
    /// </summary>
    public enum NotificationDismissReason
    {
        /// <summary>
        /// The user closed the notification.
        /// </summary>
        User,

        /// <summary>
        /// The notification expired.
        /// </summary>
        Expired,

        /// <summary>
        /// The notification was explicitly removed by application code.
        /// </summary>
        Application,

        /// <summary>
        /// 
        /// </summary>
        Unknown
    }
}