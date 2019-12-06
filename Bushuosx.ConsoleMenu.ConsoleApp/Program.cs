using System;
using Bushuosx.ConsoleMenu;

namespace Bushuosx.ConsoleMenu.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MenuItem subMenu = new MenuItem("文件file.txt") { Color = ConsoleColor.Yellow };
            subMenu.AddSubMenu(new MenuItem("编辑", (s, e) => global::System.Console.WriteLine("开始编辑……")) { Key = 'e', Color = ConsoleColor.Green });
            subMenu.AddSubMenu(new MenuItem("浏览"));

            MenuItem menuItem = new MenuItem("主菜单") { Color = ConsoleColor.Cyan };
            var openMenu = new MenuItem("打开") { Key = 'o', Color = ConsoleColor.Green };
            openMenu.AddSubMenu(subMenu);

            menuItem.AddSubMenu(openMenu);
            var m = new MenuItem("关闭", (s, e) => menuItem.Close()) { Key = 'c', Color = ConsoleColor.Red };
            menuItem.AddSubMenu(m);


            menuItem.Active();


            //Console.Write("请选择：");
            //while (true)
            //{
            //    var k = Console.ReadKey(false);
            //    if (k.Key == ConsoleKey.O)
            //    {
            //        openMenu.Click();
            //        break;
            //    }
            //}
        }
    }
}
