using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly ListBox _eventsListBox;
        private readonly TextBox _titleTextBox;
        private readonly INotificationManager _notificationManager;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _titleTextBox = this.FindControl<TextBox>("TitleTextBox");
            _bodyTextBox = this.FindControl<TextBox>("BodyTextBox");
            _eventsListBox = this.FindControl<ListBox>("EventsListBox");
            _eventsListBox.Items = new ObservableCollection<string>();

            _notificationManager = AvaloniaLocator.Current.GetService<INotificationManager>() ??
                                   throw new InvalidOperationException("Missing notification manager");
            _notificationManager.NotificationActivated += OnNotificationActivated;
            _notificationManager.NotificationDismissed += OnNotificationDismissed;

            if (_notificationManager.LaunchActionId != null)
            {
                ((IList<string>) _eventsListBox.Items).Add($"Launch action: {_notificationManager.LaunchActionId}");
            }
        }

        private void OnNotificationDismissed(object? sender, NotificationDismissedEventArgs e)
        {
            ((IList<string>) _eventsListBox.Items).Add($"Notification dismissed: {e.Reason}");
        }

        private void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            ((IList<string>) _eventsListBox.Items).Add($"Notification activated: {e.ActionId}");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Show_OnClick(object? sender, RoutedEventArgs e)
        {
            Debug.Assert(_notificationManager != null);

            _notificationManager.ShowNotification(new Notification
            {
                Title = _titleTextBox.Text ?? _titleTextBox.Watermark,
                Body = _bodyTextBox.Text ?? _bodyTextBox.Watermark,
                Buttons =
                {
                    ("This is awesome!", null)
                }
            });
        }

        private void Schedule_OnClick(object? sender, RoutedEventArgs e)
        {
            _notificationManager.ScheduleNotification(new Notification
            {
                Title = _titleTextBox.Text ?? _titleTextBox.Watermark,
                Body = _bodyTextBox.Text ?? _bodyTextBox.Watermark
            }, DateTimeOffset.Now + TimeSpan.FromSeconds(5));
        }
    }
}