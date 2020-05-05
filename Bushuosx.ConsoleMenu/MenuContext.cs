﻿using System;
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
        public MenuDrawingStyle DrawingStyle { get; }
        internal CursorStatus CursorStatus { get; }
        internal SafeConsoleColor ConsoleColor { get; }
        public MenuItemKeyStyle KeyStyle { get; }
        public ConsoleColor DisabledItemForegroundColor { get; }

        public int MaxLengthOfShortTitle { get; set; } = 6;

        /// <summary>
        /// 菜单上下文。父菜单active时生成，用于绘制参考
        /// </summary>
        /// <param name="menuStyle"></param>
        public MenuContext(MenuDrawingStyle drawingStyle, MenuItemKeyStyle keyStyle, ConsoleColor disabledItemForegroundColor = System.ConsoleColor.DarkGray)
        {
            DrawingStyle = drawingStyle;
            KeyStyle = keyStyle;
            DisabledItemForegroundColor = disabledItemForegroundColor;

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
