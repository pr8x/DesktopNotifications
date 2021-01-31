using System.Collections.Generic;

namespace DesktopNotifications
{
    /// <summary>
    /// </summary>
    public class Notification
    {
        public Notification()
        {
            Buttons = new List<(string Title, string ActionId)>();
        }

        public string Title { get; set; }

        public string Body { get; set; }

        public List<(string Title, string ActionId)> Buttons { get; }
    }
}