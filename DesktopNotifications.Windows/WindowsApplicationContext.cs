using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DesktopNotifications.Windows
{
    public class WindowsApplicationContext : ApplicationContext
    {
        private WindowsApplicationContext(string name, string appUserModelId) : base(name)
        {
            AppUserModelId = appUserModelId;
        }

        public string AppUserModelId { get; }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SetCurrentProcessExplicitAppUserModelID(
            [MarshalAs(UnmanagedType.LPWStr)] string appId);

        public static WindowsApplicationContext FromCurrentProcess(string? customName = null,
            string? appUserModelId = null)
        {
            var aumid = appUserModelId ?? Guid.NewGuid().ToString();

            SetCurrentProcessExplicitAppUserModelID(aumid);

            var mainModule = Process.GetCurrentProcess().MainModule;

            using var shortcut = new ShellLink
            {
                TargetPath = mainModule.FileName,
                Arguments = string.Empty,
                AppUserModelID = aumid
            };

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var startMenuPath = Path.Combine(appData, @"Microsoft\Windows\Start Menu\Programs");
            var appName = customName ?? Path.GetFileNameWithoutExtension(mainModule.FileName);
            var shortcutFile = Path.Combine(startMenuPath, $"{appName}.lnk");

            shortcut.Save(shortcutFile);

            return new WindowsApplicationContext(appName, aumid);
        }
    }
}