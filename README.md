# Bushuosx.ConsoleMenu

Demo:
![��ʾ](https://github.com/bushuosx/ConsoleMenu/raw/master/example20191210.gif)

nuget:
[https://www.nuget.org/packages/Bushuosx.ConsoleMenu.Core/](https://www.nuget.org/packages/Bushuosx.ConsoleMenu.Core/)

```bash
Install-Package Bushuosx.ConsoleMenu.Core
```

Example Code:
```csharp
    Console.WriteLine("Hello World!");
    Console.WriteLine("���Ǽ���˵�֮ǰ��ʾ��");
    Console.WriteLine("===");

    MenuItem mainItem = new MenuItem("���˵�") { Color = ConsoleColor.Cyan };

    var openMenu = new MenuItem("�ļ�����") { Key = 'o', Color = ConsoleColor.Green };
    var invisibleMenu = new MenuItem("Ĭ�ϲ��ɼ�") { Visible = false };
    var reserveInvisibleMenu = new MenuItem("��ת�ɼ�", () => invisibleMenu.Visible = !invisibleMenu.Visible);
    //reserveInvisibleMenu.AtachToMenu(mainItem);
    var disableMenu = new MenuItem("disable��", () => Console.WriteLine("disable on click")) { Disabled = true, Key = 'd' };
    var closeMenu = new MenuItem("�ر�", (s, e) => mainItem.Close()) { Key = 'c', Color = ConsoleColor.Red };

    MenuItem fileMenu = new MenuItem("�ļ�file.txt") { Color = ConsoleColor.Yellow };
    fileMenu.AtachToMenu(openMenu);
    fileMenu.AddSubMenu(new MenuItem("�༭", () => Console.WriteLine("��ʼ�༭����")) { Key = 'e', Color = ConsoleColor.Green });
    fileMenu.AddSubMenu(new MenuItem("���"));
    fileMenu.OnBeforePaint += SubMenu_OnBeforePaint;

    openMenu.AddSubMenu(new MenuItem("�ļ�file.xls") { Color = ConsoleColor.Magenta });

    MenuItem closeFileMenu = new MenuItem("�ر��ļ�", () => fileMenu.Close());

    mainItem.AddSubMenu(openMenu);
    mainItem.AddSubMenu(invisibleMenu);
    closeFileMenu.AtachToMenu(fileMenu);
    mainItem.AddSubMenu(disableMenu);
    mainItem.AddSubMenu(closeMenu);

    mainItem.AddSubMenu(1, reserveInvisibleMenu);


    mainItem.Active(new MenuStyle(MenuDrawingStyle.InheritStartPoint, MenuItemTitleStyle.KeyFront));
```