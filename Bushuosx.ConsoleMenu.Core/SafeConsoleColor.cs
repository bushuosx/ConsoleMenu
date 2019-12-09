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
        public SafeConsoleColor()
        {
            BackgroundColor = Console.BackgroundColor;
            ForegroundColor = Console.ForegroundColor;
        }

        /// <summary>
        /// 建造一对可视化比较强的颜色组
        /// </summary>
        /// <param name="bColor"></param>
        /// <param name="fColor"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        public SafeConsoleColor(ConsoleColor bColor, ConsoleColor fColor, bool intensity = false)
        {
            //var rst = new SafeConsoleColor { BackgroundColor = bColor, ForegroundColor = fColor };

            if (fColor == bColor)
            {
                if (bColor == ConsoleColor.White)
                {
                    BackgroundColor = bColor;
                    ForegroundColor = ConsoleColor.Black;
                }
                else if (bColor == ConsoleColor.Black)
                {
                    BackgroundColor = bColor;
                    ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    BackgroundColor = ConsoleColor.Black;
                    ForegroundColor = fColor;
                }
            }
            else if (intensity)
            {
                BackgroundColor = fColor;
                ForegroundColor = bColor;
            }
            else
            {
                BackgroundColor = bColor;
                ForegroundColor = fColor;
            }
        }
    }
}
