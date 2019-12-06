using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    public class MenuItemEventArgs : EventArgs
    {
        public bool IsCanceled { get; set; }
    }
}
