using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DesktopNotifications.Windows
{
    public class WindowsNotificationManager : INotificationManager
    {
        private const int LaunchNotificationWaitMs = 5_000;
        private readonly WindowsApplicationContext _applicationContext;
        private readonly TaskCompletionSource<string>? _launchActionPromise;
        private readonly Dictionary<ToastNotification, Notification> _notifications;
        private readonly ToastNotifierCompat _toastNotifier;

        /// <summary>
        /// </summary>
        /// <param name="applicationContext"></param>
        public WindowsNotificationManager(WindowsApplicationContext? applicationContext = null)
        {
            _applicationContext = applicationContext ?? WindowsApplicationContext.FromCurrentProcess();
            _launchActionPromise = new TaskCompletionSource<string>();

            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                ToastNotificationManagerCompat.OnActivated += OnAppActivated;

                if (_launchActionPromise.Task.Wait(LaunchNotificationWaitMs))
                {
                    LaunchActionId = _launchActionPromise.Task.Result;
                }
            }

            _toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
            _notifications = new Dictionary<ToastNotification, Notification>();
        }

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public string? LaunchActionId { get; }

        public ValueTask Initialize()
        {
            return default;
        }

        public ValueTask ShowNotification(Notification notification, DateTimeOffset? expirationTime)
        {
            if (expirationTime < DateTimeOffset.Now)
            {
                throw new ArgumentException(nameof(expirationTime));
            }

            var xmlContent = GenerateXml(notification);
            var toastNotification = new ToastNotification(xmlContent)
            {
                ExpirationTime = expirationTime
            };

            toastNotification.Activated += ToastNotificationOnActivated;
            toastNotification.Dismissed += ToastNotificationOnDismissed;
            toastNotification.Failed += ToastNotificationOnFailed;

            _toastNotifier.Show(toastNotification);
            _notifications[toastNotification] = notification;

            return default;
        }

        public ValueTask ScheduleNotification(
            Notification notification,
            DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null)
        {
            if (deliveryTime < DateTimeOffset.Now || deliveryTime > expirationTime)
            {
                throw new ArgumentException(nameof(deliveryTime));
            }

            var xmlContent = GenerateXml(notification);
            var toastNotification = new ScheduledToastNotification(xmlContent, deliveryTime)
            {
                ExpirationTime = expirationTime
            };

            _toastNotifier.AddToSchedule(toastNotification);

            return default;
        }

        public void Dispose()
        {
        }

        private static XmlDocument GenerateXml(Notification notification)
        {
            var builder = new ToastContentBuilder();

            builder.AddText(notification.Title);
            builder.AddText(notification.Body);

            foreach (var (title, actionId) in notification.Buttons)
            {
                builder.AddButton(title, ToastActivationType.Foreground, actionId);
            }

            return builder.GetXml();
        }

        private void OnAppActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            Debug.Assert(_launchActionPromise != null);

            var actionId = GetActionId(e.Argument);
            _launchActionPromise.SetResult(actionId);
        }

        private static void ToastNotificationOnFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            throw args.ErrorCode;
        }

        private void ToastNotificationOnDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            if (!_notifications.Remove(sender, out var notification))
            {
                return;
            }

            var reason = args.Reason switch
            {
                ToastDismissalReason.UserCanceled => NotificationDismissReason.User,
                ToastDismissalReason.TimedOut => NotificationDismissReason.Expired,
                ToastDismissalReason.ApplicationHidden => NotificationDismissReason.Application,
                _ => throw new ArgumentOutOfRangeException()
            };

            NotificationDismissed?.Invoke(this, new NotificationDismissedEventArgs(notification, reason));
        }

        private static string GetActionId(string argument)
        {
            return string.IsNullOrEmpty(argument) ? "default" : argument;
        }

        private void ToastNotificationOnActivated(ToastNotification sender, object args)
        {
            var activationArgs = (ToastActivatedEventArgs) args;
            var notification = _notifications[sender];
            var actionId = GetActionId(activationArgs.Arguments);

            NotificationActivated?.Invoke(this, new NotificationActivatedEventArgs(notification, actionId));
        }
    }
}