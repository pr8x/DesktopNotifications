using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;

namespace Example
{
    internal class Program
    {
        [DllImport("shell32.dll", SetLastError = true)]
        internal static extern void SetCurrentProcessExplicitAppUserModelID(
            [MarshalAs(UnmanagedType.LPWStr)] string appId);

        private static INotificationManager CreateManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new FreeDesktopNotificationManager();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsNotificationManager();
            }

            throw new PlatformNotSupportedException();
        }

        private static async Task Main(string[] args)
        {
            using var manager = CreateManager();
            await manager.Initialize();

            manager.NotificationActivated += ManagerOnNotificationActivated;
            manager.NotificationDismissed += ManagerOnNotificationDismissed;

            var notification = new Notification
            {
                Title = "Hello World!",
                Body = "Isn't this awesome?",
                Buttons =
                {
                    ("Yes", "answer_yes"),
                    ("No", "answer_no"),
                    ("Maybe", "answer_maybe")
                }
            };

            await manager.ShowNotification(notification);

            await Task.Delay(10_000);
        }

        private static void ManagerOnNotificationDismissed(object? sender, NotificationDismissedEventArgs e)
        {
            Console.WriteLine($"Notification dismissed: {e.Reason}");
        }

        private static void ManagerOnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            Console.WriteLine($"Notification activated: {e.ActionId}");
        }
    }
}