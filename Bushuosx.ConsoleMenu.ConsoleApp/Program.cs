using System;
using Bushuosx.ConsoleMenu;

namespace Bushuosx.ConsoleMenu.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MenuItem mainItem = new MenuItem("主菜单") { Color = ConsoleColor.Cyan };

            var openMenu = new MenuItem("打开") { Key = 'o', Color = ConsoleColor.Green };
            var invisibleMenu = new MenuItem("默认不可见") { Visible = false };
            var disableMenu = new MenuItem("disable项", () => Console.WriteLine("disable on click")) { Disabled = true, Key = 'd' };
            var closeMenu = new MenuItem("关闭", (s, e) => mainItem.Close()) { Key = 'c', Color = ConsoleColor.Red };

            MenuItem fileMenu = new MenuItem("文件file.txt") { Color = ConsoleColor.Yellow };
            fileMenu.AddSubMenu(new MenuItem("编辑", () => Console.WriteLine("开始编辑……")) { Key = 'e', Color = ConsoleColor.Green });
            fileMenu.AddSubMenu(new MenuItem("浏览"));
            fileMenu.OnBeforePaint += SubMenu_OnBeforePaint;
            openMenu.AddSubMenu(fileMenu);


            mainItem.AddSubMenu(openMenu);
            mainItem.AddSubMenu(invisibleMenu);
            mainItem.AddSubMenu(disableMenu);
            mainItem.AddSubMenu(closeMenu);


            mainItem.Active();
        }

        static void SubMenu_OnBeforePaint(object s, MenuItemEventArgs args)
        {
            Console.WriteLine("Painting");
            Console.WriteLine("这是文件……");
            Console.WriteLine("");
        }
    }
}
