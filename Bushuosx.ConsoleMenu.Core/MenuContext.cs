using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 菜单上下文。父菜单active时生成，用于绘制参考
    /// </summary>
    public class MenuContext : IConsoleContextItem
    {
        /// <summary>
        /// 菜单绘制样式
        /// </summary>
        public MenuStyle Style { get; private set; }

        internal CursorStatus CursorStatus { get; private set; }
        internal SafeConsoleColor ConsoleColor { get; private set; }

        /// <summary>
        /// 菜单上下文。父菜单active时生成，用于绘制参考
        /// </summary>
        /// <param name="menuStyle"></param>
        public MenuContext(MenuStyle menuStyle)
        {
            Style = menuStyle ?? throw new ArgumentNullException(nameof(menuStyle));
            CursorStatus = new CursorStatus();
            ConsoleColor = new SafeConsoleColor();
        }

        /// <summary>
        /// 恢复至console
        /// </summary>
        public void SetToConsole()
        {
            ConsoleColor.SetToConsole();
            CursorStatus.SetToConsole();
        }
    }
}
