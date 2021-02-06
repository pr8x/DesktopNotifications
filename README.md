# DesktopNotifications

A cross-platform C# library for desktop "toast" notifications.


# Features

|                          | Windows | Linux (FreeDesktop) | OSX |
|--------------------------|---------|---------------------|-----|
| Show notifications  | ✓       | ✓                   | ✕   |
| Schedule notifications | ✕       | ✕                   | ✕   |
| Launch actions¹           | ✓       | ✕                   |  ✕   |
| Replacing notifications                | ✕       | ✕                   |  ✕   |
| Buttons                  | ✓       | ✓                   |  ✕   |
| Advanced content (Audio, Images, etc)                  | ✕       | ✕                   |  ✕   |

<sub> ¹ Some platforms support launching your application when the user clicked a notification. The associated action identifier is passed as a command-line argument. </sub>

# Avalonia

The `DesktopNotifications.Avalonia` package offers support for the [Avalonia](https://github.com/AvaloniaUI/Avalonia) project. It doesn't do much on its own, it just provides helpers to register
the `INotificationManager` with the application builder.


# License

See [License](License.md)
