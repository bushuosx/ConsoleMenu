using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// MenuItem事件参数
    /// </summary>
    public class MenuItemEventArgs : EventArgs
    {
        /// <summary>
        /// 设为true，将阻止事件继续传递
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
