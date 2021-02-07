using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;

namespace DesktopNotifications.Avalonia
{
    /// <summary>
    /// Extensions for <see cref="AppBuilderBase{TAppBuilder}"/>
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Setups the <see cref="INotificationManager"/> for the current platform and
        /// binds it to the service locator (<see cref="AvaloniaLocator"/>).
        /// </summary>
        /// <typeparam name="TAppBuilder"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static TAppBuilder SetupDesktopNotifications<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            return builder.AfterSetup(async b =>
            {
                var runtimeInfo = b.RuntimePlatform.GetRuntimeInfo();
                INotificationManager manager;

                switch (runtimeInfo.OperatingSystem)
                {
                    case OperatingSystemType.WinNT:
                    {
                        var context = WindowsApplicationContext.FromCurrentProcess(b.Instance.Name);
                        manager = new WindowsNotificationManager(context);
                        break;
                    }

                    case OperatingSystemType.Linux:
                    {
                        var context = FreeDesktopApplicationContext.FromCurrentProcess();
                        manager = new FreeDesktopNotificationManager(context);
                        break;
                    }

                    //TODO: OSX once implemented/stable
                    default: return;
                }

                await manager.Initialize();

                AvaloniaLocator.CurrentMutable.Bind<INotificationManager>().ToConstant(manager);
            });
        }
    }
}