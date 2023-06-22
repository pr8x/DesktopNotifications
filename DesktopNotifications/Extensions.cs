using System.Collections.Generic;

namespace DesktopNotifications
{
    public static class Extensions
    {
        public static bool TryGetKey<K, V>(this IDictionary<K, V> instance, V value, out K key)
        {
            foreach (var entry in instance)
            {
                if (!entry.Value.Equals(value))
                {
                    continue;
                }
                key = entry.Key;
                return true;
            }
            key = default(K);
            return false;
        }
    }
}
