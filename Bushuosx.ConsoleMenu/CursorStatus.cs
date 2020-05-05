using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    internal class CursorStatus : IConsoleContextItem
    {
        /// <summary>
        /// 无参构造时，直接catch from console
        /// </summary>
        internal CursorStatus()
        {
            Left = Console.CursorLeft;
            Top = Console.CursorTop;
            Size = Console.CursorSize;
            Visible = Console.CursorVisible;
        }
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Size { get; private set; }
        public bool Visible { get; private set; }

        public void SetToConsole()
        {
            Console.CursorLeft = Left;
            Console.CursorTop = Top;
            Console.CursorSize = Size;
            Console.CursorVisible = Visible;
        }
    }
}
