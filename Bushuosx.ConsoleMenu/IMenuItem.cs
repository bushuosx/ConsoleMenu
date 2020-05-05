using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    public interface IMenuItem
    {
        event EventHandler<MenuItemEventArgs> OnClick;

        string Title { get; }
        char Key { get; }
        string Description { get; set; }
        ConsoleColor ForegroundColor { get; }

        string Header { get; }
        string Footer { get; }

        bool Disable { get; }
        bool Visible { get; }

        IMenuItem Parent { get; }
        IReadOnlyList<IMenuItem> Children { get; }
        int SelectedSubmenuIndex { get; }
        void SelectSubmenuItem(int childMenuItemIndex);

        void AddSubMenu(params IMenuItem[] subMenuItems);
        void AddSubMenu(int index, params IMenuItem[] subMenuItems);
        void AttachParentMenu(IMenuItem parentMenuItem);

        void Active(MenuContext menuContext);

        string GetShortTitle(int max);

        void GetMenuPath(ref List<IMenuItem> menuPath);
    }
}
