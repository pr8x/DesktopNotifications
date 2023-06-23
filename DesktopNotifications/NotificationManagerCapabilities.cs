using System;

namespace DesktopNotifications
{
    [Flags]
    public enum NotificationManagerCapabilities
    {
        None = 0,
        BodyText = 1 << 0,
        BodyImages = 1 << 1,
        BodyMarkup = 1 << 2,
        Audio = 1 << 3,
        Icon = 1 << 4
    }
}