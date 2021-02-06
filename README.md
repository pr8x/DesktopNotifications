# DesktopNotifications

A cross-platform C# library for desktop "toast" notifications.

![Build](https://github.com/pr8x/DesktopNotifications/workflows/Build/badge.svg)

[Screnshot (Linux)](.github/images/linux_demo_image_06_02_21.png?raw=true)

[Screenshot (Windows)](.github/images/win_demo_image_06_02_21.png?raw=true)

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

# Help wanted

My MacBook Pro is 11 years old now and it does not run OSX Mojave or higher. Unfortunately, Apple deprecated `NSUserNotificationCenter` with OSX 10.14 and I suppose they want us to use the newer `UNUserNotificationCenter` API instead. I would kindly appreciate any contributions from folks that own a more modern Mac and are interested in implementing the OSX backend using the new API.

# License

See [License](License.md)
