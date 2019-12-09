using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    public class MenuStyle
    {
        public MenuStyle(MenuDrawingStyle drawingStyle, MenuItemTitleStyle titleStyle)
        {
            DrawingStyle = drawingStyle;
            TitleStyle = titleStyle;
        }
        public MenuDrawingStyle DrawingStyle { get; set; }
        public MenuItemTitleStyle TitleStyle { get; set; }
    }
}
