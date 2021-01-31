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
        private readonly Dictionary<ToastNotification, Notification> _notifications;
        private readonly ToastNotifier _toastNotifier;
        private TaskCompletionSource<string?>? _launchActionPromise;

        /// <summary>
        /// </summary>
        /// <param name="appId"></param>
        public WindowsNotificationManager(string appId)
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier(appId);
            _notifications = new Dictionary<ToastNotification, Notification>();
        }

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;
        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public async ValueTask Initialize()
        {
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                _launchActionPromise = new TaskCompletionSource<string?>();
                ToastNotificationManagerCompat.OnActivated += OnAppActivated;

                var launchAction = await _launchActionPromise.Task;

                Debug.Assert(launchAction != null);

                //TODO: Lookup notification object from history?
                NotificationActivated?.Invoke(this,
                    new NotificationActivatedEventArgs(null, launchAction));
            }
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

        private void OnAppActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            Debug.Assert(_launchActionPromise != null);
            _launchActionPromise.SetResult(e.Argument);
        }

        private static void ToastNotificationOnFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            throw args.ErrorCode;
        }

        private void ToastNotificationOnDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            var notification = _notifications[sender];
            var reason = args.Reason switch
            {
                ToastDismissalReason.UserCanceled => NotificationDismissReason.User,
                ToastDismissalReason.TimedOut => NotificationDismissReason.Expired,
                ToastDismissalReason.ApplicationHidden => NotificationDismissReason.Application,
                _ => throw new ArgumentOutOfRangeException()
            };

            NotificationDismissed?.Invoke(this, new NotificationDismissedEventArgs(notification, reason));

            _notifications.Remove(sender);
        }

        private void ToastNotificationOnActivated(ToastNotification sender, object args)
        {
            var activationArgs = (ToastActivatedEventArgs) args;
            var notification = _notifications[sender];
            var actionId = string.IsNullOrEmpty(activationArgs.Arguments) ? "default" : activationArgs.Arguments;

            NotificationActivated?.Invoke(this, new NotificationActivatedEventArgs(notification, actionId));
        }

        public void Dispose()
        {
        }
    }
}