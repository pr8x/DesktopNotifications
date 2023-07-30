using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DesktopNotifications;

namespace Example.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly INotificationManager _notificationManager;

        private Notification? _lastNotification;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _notificationManager = Program.NotificationManager ??
                                   throw new InvalidOperationException("Missing notification manager");
            _notificationManager.NotificationActivated += OnNotificationActivated;
            _notificationManager.NotificationDismissed += OnNotificationDismissed;

            Log($"Capabilities: {FormatFlagEnum(_notificationManager.Capabilities)}");

            BodyTextBox.IsEnabled =
                _notificationManager.Capabilities.HasFlag(NotificationManagerCapabilities.BodyText);

            ImagePathTextBox.IsEnabled =
                _notificationManager.Capabilities.HasFlag(NotificationManagerCapabilities.BodyImages);

            if (_notificationManager.LaunchActionId != null)
            {
                Log($"Launch action: {_notificationManager.LaunchActionId}");
            }
        }

        private static string FormatFlagEnum<TEnum>(TEnum e) where TEnum : Enum
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

            return string.Join(", ",
                enumValues
                    .Where(x => Convert.ToInt32(x) != 0 && e.HasFlag(x))
                    .Select(x => x.ToString()));
        }

        private void Log(string @event)
        {
            LogListBox.Items.Add(@event);
        }

        private void OnNotificationDismissed(object? sender, NotificationDismissedEventArgs e)
        {
            Log($"Notification dismissed: {e.Reason}");
        }

        private void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            Log($"Notification activated: {e.ActionId}");
        }

        public async void Show_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                Debug.Assert(_notificationManager != null);

                var nf = new Notification
                {
                    Title = TitleTextBox.Text ?? TitleTextBox.Watermark,
                    Body = BodyTextBox.Text ?? BodyTextBox.Watermark,
                    BodyImagePath = ImagePathTextBox.Text,
                    Buttons =
                    {
                        ("This is awesome!", "awesome")
                    }
                };

                await _notificationManager.ShowNotification(nf);

                _lastNotification = nf;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private async void Schedule_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                var nf = new Notification
                {
                    Title = TitleTextBox.Text ?? TitleTextBox.Watermark,
                    Body = BodyTextBox.Text ?? BodyTextBox.Watermark
                };

                await _notificationManager.ScheduleNotification(
                    nf,
                    DateTimeOffset.Now + TimeSpan.FromSeconds(5));

                _lastNotification = nf;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private async void HideLast_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_lastNotification != null)
                {
                    await _notificationManager.HideNotification(_lastNotification);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
    }
}