using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 菜单绘制样式
    /// </summary>
    public class MenuStyle
    {
        /// <summary>
        /// 菜单绘制样式
        /// </summary>
        /// <param name="drawingStyle">绘制方式</param>
        /// <param name="titleStyle">标题风格</param>
        public MenuStyle(MenuDrawingStyle drawingStyle, MenuItemTitleStyle titleStyle)
        {
            DrawingStyle = drawingStyle;
            TitleStyle = titleStyle;
        }
        /// <summary>
        /// 绘制方式
        /// </summary>
        public MenuDrawingStyle DrawingStyle { get; set; }
        /// <summary>
        /// 标题风格
        /// </summary>
        public MenuItemTitleStyle TitleStyle { get; set; }
    }
}
