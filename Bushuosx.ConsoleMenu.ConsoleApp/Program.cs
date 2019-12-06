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
            subMenu.AddSubMenu(new MenuItem("编辑", () => Console.WriteLine("开始编辑……")) { Key = 'e', Color = ConsoleColor.Green });
            subMenu.AddSubMenu(new MenuItem("浏览"));
            subMenu.OnBeforePaint += SubMenu_OnBeforePaint;

            MenuItem menuItem = new MenuItem("主菜单") { Color = ConsoleColor.Cyan };
            var openMenu = new MenuItem("打开") { Key = 'o', Color = ConsoleColor.Green };
            openMenu.AddSubMenu(subMenu);

            menuItem.AddSubMenu(openMenu);
            var m = new MenuItem("关闭", (s, e) => menuItem.Close()) { Key = 'c', Color = ConsoleColor.Red };
            menuItem.AddSubMenu(m);


            menuItem.Active();
        }

        static void SubMenu_OnBeforePaint(object s, MenuItemEventArgs args)
        {
            Console.WriteLine("Painting");
            Console.WriteLine("这是文件……");
            Console.WriteLine("");
        }
    }
}
