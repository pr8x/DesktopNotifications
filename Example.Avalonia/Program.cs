using Avalonia;
using DesktopNotifications.Avalonia;

namespace Example.Avalonia
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .SetupDesktopNotifications()
                .LogToTrace();
        }
    }
}