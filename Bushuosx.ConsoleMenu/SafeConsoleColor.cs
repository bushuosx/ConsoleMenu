using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    internal class SafeConsoleColor : IConsoleContextItem
    {
        public ConsoleColor BackgroundColor { get; private set; }
        public ConsoleColor ForegroundColor { get; private set; }

        public void SetToConsole()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
        }

        /// <summary>
        /// 无参构造时，直接catch from console
        /// </summary>
        internal SafeConsoleColor()
        {
            BackgroundColor = Console.BackgroundColor;
            ForegroundColor = Console.ForegroundColor;
        }

        /// <summary>
        /// 建造一对可视化比较强的颜色组
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="intensity">颜色反差化</param>
        /// <returns></returns>
        public SafeConsoleColor(ConsoleColor foregroundColor, ConsoleColor backgroundColor, bool intensity = false)
        {
            //var rst = new SafeConsoleColor { BackgroundColor = bColor, ForegroundColor = fColor };

            if (foregroundColor == backgroundColor)
            {
                if (backgroundColor == ConsoleColor.White)
                {
                    BackgroundColor = backgroundColor;
                    ForegroundColor = ConsoleColor.Black;
                }
                else if (backgroundColor == ConsoleColor.Black)
                {
                    BackgroundColor = backgroundColor;
                    ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    BackgroundColor = ConsoleColor.Black;
                    ForegroundColor = foregroundColor;
                }
            }
            else if (intensity)
            {
                BackgroundColor = foregroundColor;
                ForegroundColor = backgroundColor;
            }
            else
            {
                BackgroundColor = backgroundColor;
                ForegroundColor = foregroundColor;
            }
        }
    }
}
