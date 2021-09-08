using System;
using System.Diagnostics;
using System.IO;

namespace DesktopNotifications.FreeDesktop
{
    /// <summary>
    /// </summary>
    public class FreeDesktopApplicationContext : ApplicationContext
    {
        private FreeDesktopApplicationContext(string name, string? appIcon) : base(name)
        {
            AppIcon = appIcon;
        }

        /// <summary>
        /// </summary>
        public string? AppIcon { get; }

        public static FreeDesktopApplicationContext FromCurrentProcess(string? appIcon = null)
        {
            var mainModule = Process.GetCurrentProcess().MainModule;

            if (mainModule?.FileName == null)
            {
                throw new InvalidOperationException("No valid process module found.");
            }

            return new FreeDesktopApplicationContext(
                Path.GetFileNameWithoutExtension(mainModule.FileName),
                appIcon
            );
        }
    }
}