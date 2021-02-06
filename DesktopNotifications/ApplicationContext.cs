using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopNotifications
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationContext
    {
        public ApplicationContext(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

    }
}
