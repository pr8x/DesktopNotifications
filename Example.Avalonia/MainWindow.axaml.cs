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
    public class MainWindow : Window
    {
        private readonly TextBox _bodyTextBox;
        private readonly ListBox _logListBox;
        private readonly TextBox _imagePathTextBox;
        private readonly INotificationManager _notificationManager;
        private readonly TextBox _titleTextBox;

        private Notification? _lastNotification;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _titleTextBox = this.FindControl<TextBox>("TitleTextBox");
            _bodyTextBox = this.FindControl<TextBox>("BodyTextBox");
            _imagePathTextBox = this.FindControl<TextBox>("ImagePathTextBox");
            _logListBox = this.FindControl<ListBox>("LogListBox");
            _logListBox.Items = new ObservableCollection<string>();

            _notificationManager = AvaloniaLocator.Current.GetService<INotificationManager>() ??
                                   throw new InvalidOperationException("Missing notification manager");
            _notificationManager.NotificationActivated += OnNotificationActivated;
            _notificationManager.NotificationDismissed += OnNotificationDismissed;

            Log($"Capabilities: {FormatFlagEnum(_notificationManager.Capabilities)}");

            _bodyTextBox.IsEnabled =
                _notificationManager.Capabilities.HasFlag(NotificationManagerCapabilities.BodyText);

            _imagePathTextBox.IsEnabled =
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
            ((IList<string>)_logListBox.Items).Add(@event);
        }

        private void OnNotificationDismissed(object? sender, NotificationDismissedEventArgs e)
        {
            Log($"Notification dismissed: {e.Reason}");
        }

        private void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            Log($"Notification activated: {e.ActionId}");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void Show_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                Debug.Assert(_notificationManager != null);

                var nf = new Notification
                {
                    Title = _titleTextBox.Text ?? _titleTextBox.Watermark,
                    Body = _bodyTextBox.Text ?? _bodyTextBox.Watermark,
                    BodyImagePath = _imagePathTextBox.Text,
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
                    Title = _titleTextBox.Text ?? _titleTextBox.Watermark,
                    Body = _bodyTextBox.Text ?? _bodyTextBox.Watermark
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