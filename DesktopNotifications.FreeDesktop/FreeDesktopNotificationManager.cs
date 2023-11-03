using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus;

namespace DesktopNotifications.FreeDesktop
{
    public class FreeDesktopNotificationManager : INotificationManager
    {
        private const string NotificationsService = "org.freedesktop.Notifications";

        private static readonly ObjectPath NotificationsPath = new ObjectPath("/org/freedesktop/Notifications");

        private static Dictionary<string, NotificationManagerCapabilities> CapabilitiesMapping =
            new Dictionary<string, NotificationManagerCapabilities>
            {
                { "body", NotificationManagerCapabilities.BodyText },
                { "body-images", NotificationManagerCapabilities.BodyImages },
                { "body-markup", NotificationManagerCapabilities.BodyMarkup },
                { "sound", NotificationManagerCapabilities.Audio },
                { "icon", NotificationManagerCapabilities.Icon }
            };

        private readonly Dictionary<uint, Notification> _activeNotifications;
        private readonly FreeDesktopApplicationContext _appContext;
        private Connection? _connection;
        private IDisposable? _notificationActionSubscription;
        private IDisposable? _notificationCloseSubscription;

        private IFreeDesktopNotificationsProxy? _proxy;

        /// <summary>
        /// </summary>
        /// <param name="appContext"></param>
        public FreeDesktopNotificationManager(FreeDesktopApplicationContext? appContext = null)
        {
            _appContext = appContext ?? FreeDesktopApplicationContext.FromCurrentProcess();
            _activeNotifications = new Dictionary<uint, Notification>();
        }

        public void Dispose()
        {
            _notificationActionSubscription?.Dispose();
            _notificationCloseSubscription?.Dispose();
        }

        public NotificationManagerCapabilities Capabilities { get; private set; } =
            NotificationManagerCapabilities.None;

        public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;

        public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

        public string? LaunchActionId { get; }

        public async Task Initialize()
        {
            _connection = Connection.Session;

            await _connection.ConnectAsync();

            _proxy = _connection.CreateProxy<IFreeDesktopNotificationsProxy>(
                NotificationsService,
                NotificationsPath
            );

            _notificationActionSubscription = await _proxy.WatchActionInvokedAsync(
                OnNotificationActionInvoked,
                OnNotificationActionInvokedError
            );

            _notificationCloseSubscription = await _proxy.WatchNotificationClosedAsync(
                OnNotificationClosed,
                OnNotificationClosedError
            );

            foreach (var cap in await _proxy.GetCapabilitiesAsync())
            {
                if (CapabilitiesMapping.TryGetValue(cap, out var capE))
                {
                    Capabilities |= capE;
                }
            }
        }

        public async Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null)
        {
            CheckConnection();

            if (expirationTime < DateTimeOffset.Now)
            {
                throw new ArgumentException(nameof(expirationTime));
            }

            var duration = expirationTime - DateTimeOffset.Now;
            var actions = GenerateActions(notification);

            var id = await _proxy!.NotifyAsync(
                _appContext.Name,
                0,
                _appContext.AppIcon ?? string.Empty,
                notification.Title ?? throw new ArgumentException(),
                GenerateNotificationBody(notification),
                actions.ToArray(),
                new Dictionary<string, object> { { "urgency", 1 } },
                (int?) duration?.TotalMilliseconds ?? 0
            ).ConfigureAwait(false);

            _activeNotifications[id] = notification;
        }

        public async Task HideNotification(Notification notification)
        {
            CheckConnection();

            if (_activeNotifications.TryGetKey(notification, out var id))
            {
                await _proxy!.CloseNotificationAsync(id);
            }
        }

        public async Task ScheduleNotification(
            Notification notification,
            DateTimeOffset deliveryTime,
            DateTimeOffset? expirationTime = null)
        {
            CheckConnection();

            if (deliveryTime < DateTimeOffset.Now || deliveryTime > expirationTime)
            {
                throw new ArgumentException(nameof(deliveryTime));
            }

            //Note: We could consider spawning some daemon that sends the notification at the specified time.
            //For now we only allow to schedule notifications while the application is running.
            await Task.Delay(deliveryTime - DateTimeOffset.Now);

            await ShowNotification(notification, expirationTime);
        }

        private string GenerateNotificationBody(Notification notification)
        {
            if (notification.Body == null)
            {
                throw new ArgumentException();
            }

            var sb = new StringBuilder();

            sb.Append(notification.Body);

            if (Capabilities.HasFlag(NotificationManagerCapabilities.BodyImages) &&
                notification.BodyImagePath is { } img)
            {
                sb.Append($@"\n<img src=""{img}"" alt=""{notification.BodyImageAltText}""/>");
            }

            return sb.ToString();
        }

        private void CheckConnection()
        {
            if (_connection == null || _proxy == null)
            {
                throw new InvalidOperationException("Not connected. Call Initialize() first.");
            }
        }

        private static IEnumerable<string> GenerateActions(Notification notification)
        {
            foreach (var (title, actionId) in notification.Buttons)
            {
                yield return actionId;
                yield return title;
            }
        }

        private static void OnNotificationClosedError(Exception obj)
        {
            throw obj;
        }

        private static NotificationDismissReason GetReason(uint reason)
        {
            return reason switch
            {
                1 => NotificationDismissReason.Expired,
                2 => NotificationDismissReason.User,
                3 => NotificationDismissReason.Application,
                _ => NotificationDismissReason.Unknown
            };
        }

        private void OnNotificationClosed((uint id, uint reason) @event)
        {
            if (!_activeNotifications.TryGetValue(@event.id, out var notification)) return;

            _activeNotifications.Remove(@event.id);

            //TODO: Not sure why but it calls this event twice sometimes
            //In this case the notification has already been removed from the dict.
            if (notification == null)
            {
                return;
            }

            var dismissReason = GetReason(@event.reason);

            NotificationDismissed?.Invoke(this,
                new NotificationDismissedEventArgs(notification, dismissReason));
        }

        private static void OnNotificationActionInvokedError(Exception obj)
        {
            throw obj;
        }

        private void OnNotificationActionInvoked((uint id, string actionKey) @event)
        {
            if (!_activeNotifications.TryGetValue(@event.id, out var notification)) return;

            NotificationActivated?.Invoke(this,
                new NotificationActivatedEventArgs(notification, @event.actionKey));
        }
    }
}