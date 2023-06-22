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
            _eventsListBox = this.FindControl<ListBox>("EventsListBox");
            _eventsListBox.Items = new ObservableCollection<string>();

            _notificationManager = AvaloniaLocator.Current.GetService<INotificationManager>() ??
                                   throw new InvalidOperationException("Missing notification manager");
            _notificationManager.NotificationActivated += OnNotificationActivated;
            _notificationManager.NotificationDismissed += OnNotificationDismissed;

            if (_notificationManager.LaunchActionId != null)
            {
                RegisterEvent($"Launch action: {_notificationManager.LaunchActionId}");
            }
        }

        private void RegisterEvent(string @event)
        {
            ((IList<string>)_eventsListBox.Items).Add(@event);
        }

        private void OnNotificationDismissed(object? sender, NotificationDismissedEventArgs e)
        {
            RegisterEvent($"Notification dismissed: {e.Reason}");
        }

        private void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            RegisterEvent($"Notification activated: {e.ActionId}");
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
                RegisterEvent(ex.Message);
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
                RegisterEvent(ex.Message);
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
                RegisterEvent(ex.Message);
            }
        }
    }
}