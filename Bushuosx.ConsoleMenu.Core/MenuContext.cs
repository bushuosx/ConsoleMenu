using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    public class MenuContext : IConsoleContextItem
    {
        public MenuStyle Style { get; private set; }

        internal CursorStatus CursorStatus { get; private set; }
        internal SafeConsoleColor ConsoleColor { get; private set; }

        public MenuContext(MenuStyle menuStyle)
        {
            Style = menuStyle ?? throw new ArgumentNullException(nameof(menuStyle));
            CursorStatus = new CursorStatus();
            ConsoleColor = new SafeConsoleColor();
        }

        public void SetToConsole()
        {
            ConsoleColor.SetToConsole();
            CursorStatus.SetToConsole();
        }
    }
}
