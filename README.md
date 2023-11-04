# DesktopNotifications

A cross-platform C# library for native desktop "toast" notifications.

[![Build](https://github.com/pr8x/DesktopNotifications/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/pr8x/DesktopNotifications/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/DesktopNotifications)](https://www.nuget.org/packages/DesktopNotifications/)
[![Nuget](https://img.shields.io/nuget/dt/DesktopNotifications)](https://www.nuget.org/packages/DesktopNotifications/)

[Screenshot (Linux)](.github/images/linux_demo_image_06_02_21.png?raw=true)

[Screenshot (Windows)](.github/images/win_demo_image_06_02_21.png?raw=true)

# Features

|                          | Windows | Linux (FreeDesktop.DBus) | OSX |
|--------------------------|---------|---------------------|-----|
| Show notifications  | ✓       | ✓                   | ✕   |
| Hide notifications  | ✓       | ✓                   | ✕   |
| Schedule notifications | ✓       | ✓*                   | ✕   |
| Launch actions         | ✓**       | ✕                   |  ✕   |
| Replacing notifications                | ✕       | ✕                   |  ✕   |
| Buttons                  | ✓       | ✓                   |  ✕   |
| Audio                  | ✕       | ✕                   |  ✕   |
| Images                  | ✓       | ✓***                   |  ✕   |

<sub> * Scheduled notifications will only be delivered while the application is running. </sub>  
<sub> ** This is currently not supported when targeting .netstandard </sub>  
<sub> *** If supported by the notification server </sub>

# Application Context

Most operating systems require you to register the application in some form before you can actually send notifications. This registration process is handled by the `ApplicationContext`. On Windows (see `WindowsApplicationContext`) it will create and assign a [Application User Model Id](https://docs.microsoft.com/en-us/windows/win32/shell/appids) to the current process and associate a shell link in the start menu with it. This will cause your application to appear in the Windows Start Menu.  On Linux/FreeDesktop.DBUS, the application context is just holding the name and optional icon of your application.

# Avalonia

The `DesktopNotifications.Avalonia` package offers support for the [Avalonia](https://github.com/AvaloniaUI/Avalonia) project. It doesn't do much on its own, it just provides helpers to register
the `INotificationManager` with the application builder. You can see an example of it in the Example.Avalonia project.


# Help wanted

My MacBook Pro is 11 years old now and it does not run OSX Mojave or higher. Unfortunately, Apple deprecated `NSUserNotificationCenter` with OSX 10.14 and I suppose they want us to use the newer `UNUserNotificationCenter` API instead. I would kindly appreciate any contributions from folks that own a more modern Mac and are interested in implementing the OSX backend using the new API.

# License

See [License](LICENSE.md)
