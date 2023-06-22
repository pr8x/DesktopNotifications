using System.Collections.Generic;

namespace DesktopNotifications
{
    public static class Extensions
    {
        public static bool TryGetKey<K, V>(this IDictionary<K, V> dict, V value, out K key)
        {
            foreach (var entry in dict)
            {
                if (entry.Value?.Equals(value) == true)
                {
                    key = entry.Key;
                    return true;
                }
            }

            key = default!;

            return false;
        }
    }
}