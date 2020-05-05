using System;
using System.Collections.Generic;
using System.Linq;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 适用于ConsoleApplication的菜单类
    /// </summary>
    public class MenuItem : IMenuItem
    {
        /// <summary>
        /// 适用于ConsoleApplication的菜单类
        /// </summary>
        /// <param name="title">菜单标题</param>
        public MenuItem(string title)
        {
            Title = title;
        }
        /// <summary>
        /// 适用于ConsoleApplication的菜单类
        /// </summary>
        /// <param name="title">菜单标题</param>
        /// <param name="onClick">EventHandler onClick</param>
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClick) : this(title)
        {
            OnClick += onClick;
        }
        /// <summary>
        /// 适用于ConsoleApplication的菜单类
        /// </summary>
        /// <param name="title">菜单标题</param>
        /// <param name="onClick">EventHandler onClick</param>
        /// <param name="key">激活菜单的快捷键，只能是字母或数字</param>
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClick, char key) : this(title, onClick)
        {
            Key = key;
        }
        ///// <summary>
        ///// 适用于ConsoleApplication的菜单类
        ///// </summary>
        ///// <param name="title">菜单标题</param>
        ///// <param name="onClick">EventHandler onClick</param>
        //public MenuItem(string title, Action onClick) : this(title)
        //{
        //    OnClick += (s, e) => onClick();
        //}
        ///// <summary>
        ///// 适用于ConsoleApplication的菜单类
        ///// </summary>
        ///// <param name="title">菜单标题</param>
        ///// <param name="onClick">EventHandler onClick</param>
        ///// <param name="key">激活菜单的快捷键，只能是字母或数字</param>
        //public MenuItem(string title, Action onClick, char key) : this(title, onClick)
        //{
        //    Key = key;
        //}

        /// <summary>
        /// 保存console上下文
        /// </summary>
        private MenuContext Context { get; }

        ///// <summary>
        ///// 默认的禁用样式
        ///// </summary>
        //protected ConsoleColor DisabledItemForegroundColor = ConsoleColor.DarkGray;

        ///// <summary>
        ///// 指示当前菜单是否正在展示
        ///// </summary>
        //protected bool IsActived { get; set; }

        ///// <summary>
        ///// 激活此菜单
        ///// </summary>
        ///// <param name="menuStyle">菜单样式</param>
        //public void Active(MenuStyle menuStyle)
        //{
        //    Context = new MenuContext(menuStyle);

        //    Active();
        //}

        ///// <summary>
        ///// 激活此菜单
        ///// </summary>
        ///// <param name="menuContext">菜单上下文，应由父菜单传递给子菜单</param>
        //protected void Active(MenuContext menuContext)
        //{
        //    Context = menuContext;

        //    Active();
        //}

        ///// <summary>
        ///// 内部激活
        ///// </summary>
        //private void Active()
        //{
        //    Enter();

        //    RePaint();

        //    LoopMessage();
        //}

        /// <summary>
        /// 依据给定的menu style，做菜单绘制前准备
        /// </summary>
        private void PrepareDrawingContext()
        {
            switch (Context.DrawingStyle)
            {
                case MenuDrawingStyle.ClearSreen:
                    Console.Clear();
                    break;
                case MenuDrawingStyle.InheritStartPoint:
                    var lastCursor = new CursorStatus();
                    if (Context.CursorStatus.Left > lastCursor.Left || Context.CursorStatus.Top > lastCursor.Top)
                    {
                        throw new Exception("consolemenu 启动环境信息已被破坏");
                    }
                    if (lastCursor.Top >= Console.BufferHeight - 1)
                    {
                        throw new Exception("console 缓冲区已用尽");
                    }
                    else
                    {
                        for (int i = Context.CursorStatus.Top; i < lastCursor.Top; i++)
                        {
                            Console.MoveBufferArea(0, Console.BufferHeight - 1, Console.BufferWidth, 1, 0, i);
                        }
                        Context.SetToConsole();
                    }
                    break;
                case MenuDrawingStyle.FollowSystem:
                    break;
                default:
                    throw new NotImplementedException(string.Format("未实现的绘制样式：{0}", Enum.GetName(typeof(MenuDrawingStyle), Context.DrawingStyle)));
            }
        }

        /// <summary>
        /// 恢复保存的环境
        /// </summary>
        private void RestoreContext()
        {
            PrepareDrawingContext();
        }

        ///// <summary>
        ///// 初始化一些参数
        ///// </summary>
        //private void Enter()
        //{
        //    BreakLoop = false;
        //    SelectedIndex = 0;
        //    //Console.CursorVisible = false;
        //    IsActived = true;
        //}
        ///// <summary>
        ///// 退出时恢复环境
        ///// </summary>
        //private void Exit()
        //{
        //    BreakLoop = true;
        //    IsActived = false;
        //}

        /// <summary>
        /// 调用安全写入方法，输出菜单标题
        /// </summary>
        /// <param name="showKey"></param>
        /// <param name="isSelected"></param>
        protected void SafeWriteTitle(bool showKey, bool isSelected, ConsoleColor? foregroundColor = null)
        {
            var txt = BuildTitleString(showKey);
            SafeWriteLine(txt, isSelected, foregroundColor);
        }


        /// <summary>
        /// 安全写入方法。防止颜色冲突。
        /// </summary>
        /// <param name="text"></param>
        protected void SafeWrite(string text, bool intensity = false, ConsoleColor? foregroundColor = null)
        {
            var oc = new SafeConsoleColor();
            var sc = new SafeConsoleColor(oc.BackgroundColor, foregroundColor ?? ForegroundColor, intensity);
            sc.SetToConsole();
            Console.Write(text);
            oc.SetToConsole();
        }

        /// <summary>
        /// 安全写入方法。防止颜色冲突。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="intensity"></param>
        protected void SafeWriteLine(string text, bool intensity = false, ConsoleColor? foregroundColor = null)
        {
            var oc = new SafeConsoleColor();
            var sc = new SafeConsoleColor(oc.BackgroundColor, foregroundColor ?? ForegroundColor, intensity);
            sc.SetToConsole();
            Console.WriteLine(text);
            oc.SetToConsole();
        }

        /// <summary>
        /// 根据需求生成标题样式
        /// </summary>
        /// <param name="showKey"></param>
        /// <returns></returns>
        private string BuildTitleString(bool showKey)
        {
            if (!showKey)
            {
                return Title;
            }

            switch (Context.KeyStyle)
            {
                case MenuItemKeyStyle.BeforeTitle:
                    return char.IsLetterOrDigit(Key) ? $"{Key.ToString().ToUpper()} - {Title}" : $"? - {Title}";
                case MenuItemKeyStyle.AfterTitle:
                    return char.IsLetterOrDigit(Key) ? $"{Title}({Key.ToString().ToUpper()})" : Title;
                default:
                    throw new NotImplementedException($"Context.KeyStyle未实现:{Context.KeyStyle.ToString()}");
            }
        }

        /// <summary>
        /// 绘制 菜单路径
        /// </summary>
        private void ShowParentPath()
        {
            var path = new List<IMenuItem>();
            GetMenuPath(ref path);

            SafeWrite("菜单路径：");
            if (path.Count > 0)
            {
                path.Reverse();//反转path
                foreach (var item in path)
                {
                    SafeWrite(item.GetShortTitle(Context.MaxLengthOfShortTitle) + " >> ", false, item.ForegroundColor);
                }
            }

            //SafeWrite(ShortTitle);
        }



        /// <summary>
        /// 获取父菜单列表，由近及远排列
        /// </summary>
        /// <returns></returns>
        public void GetMenuPath(ref List<IMenuItem> paths)
        {
            if (Parent == null)
            {
                return;
            }
            else
            {
                paths.Add(Parent);
                Parent.GetMenuPath(ref paths);
            }
        }


        public string Header { get; set; }
        public string Footer { get; set; }

        /// <summary>
        /// 重新绘制菜单内容
        /// </summary>
        protected void Paint()
        {
            PrepareDrawingContext();

            ShowHeader();

            ShowParentPath();

            SafeWriteTitle(false, false);

            ShowChildren();

            ShowFooter();

            ShowDescription();
        }

        private void ShowFooter()
        {
            if (Footer != null)
            {
                SafeWriteLine(Footer);
            }
        }

        private void ShowHeader()
        {
            if (Header != null)
            {
                SafeWriteLine(Header);
            }
        }

        /// <summary>
        /// 子菜单项是否可供用户选择
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsCanBeSelectItem(IMenuItem menuItem) => menuItem.Visible && !menuItem.Disable;

        /// <summary>
        /// 获取上一子菜单项
        /// </summary>
        /// <returns></returns>
        private int GetUpChildItemIndex()
        {
            //需要修改
            if (!SelectedIndexIsValid)
            {
                var i = _children.FindLastIndex(x => IsCanBeSelectItem(x));
                if (IsValidChildIndex(i))
                {
                    return i;
                }
            }
            else
            {
                var up = SelectedSubmenuIndex - 1;
                if (IsValidChildIndex(up))
                {
                    var i = _children.FindLastIndex(up, x => IsCanBeSelectItem(x));
                    if (IsValidChildIndex(i))
                    {
                        return i;
                    }
                }
                else
                {
                    var last = _children.FindLastIndex(x => IsCanBeSelectItem(x));
                    if (SelectedSubmenuIndex != last)
                    {
                        return last;
                    }
                }
            }

            return SelectedSubmenuIndex;
        }
        /// <summary>
        /// 获取下一子菜单项
        /// </summary>
        /// <returns></returns>
        private int GetDownChildItemIndex()
        {
            if (!SelectedIndexIsValid)
            {
                var i = _children.FindIndex(x => IsCanBeSelectItem(x));
                if (IsValidChildIndex(i))
                {
                    return i;
                }
            }
            else
            {
                var down = SelectedSubmenuIndex + 1;
                if (IsValidChildIndex(down))
                {
                    var i = _children.FindIndex(down, x => IsCanBeSelectItem(x));
                    if (IsValidChildIndex(i))
                    {
                        return i;
                    }
                }
                else
                {
                    var first = _children.FindIndex(x => IsCanBeSelectItem(x));
                    if (SelectedSubmenuIndex != first)//当前前面还有元素
                    {
                        return first;
                    }
                }
            }

            return SelectedSubmenuIndex;
        }
        /// <summary>
        /// 当前选择的索引是否是有效的
        /// </summary>
        protected bool SelectedIndexIsValid
        {
            get { return IsValidChildIndex(SelectedSubmenuIndex); }
        }
        /// <summary>
        /// 是否是当前有效的索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsValidChildIndex(int index)
        {
            return index >= 0 && index < Children.Count;
        }
        ///// <summary>
        ///// 退出消息循环
        ///// </summary>
        //private bool BreakLoop { get; set; }

        //[Obsolete]
        ///// <summary>
        ///// 循环读取用户按键，响应事件
        ///// </summary>
        //private void LoopMessage()
        //{
        //    while (true)
        //    {
        //        var readKeyInfo = Console.ReadKey(true);
        //        var key = readKeyInfo.Key;
        //        if (key == ConsoleKey.UpArrow)
        //        {
        //            SelectChildItem(GetUpChildItemIndex());
        //        }
        //        else if (key == ConsoleKey.DownArrow)
        //        {
        //            SelectChildItem(GetDownChildItemIndex());
        //        }
        //        else if (key == ConsoleKey.Enter)
        //        {
        //            if (SelectedIndexIsValid)
        //            {
        //                Children[SelectedIndex].Click();
        //            }
        //        }
        //        else if (key == ConsoleKey.Escape)
        //        {
        //            Close();
        //            //BreakLoop = true;
        //        }
        //        else
        //        {
        //            //其它按键
        //            var items = Children.Where(x => char.IsLetterOrDigit(x.Key) && IsCanBeSelectItem(x) && string.Equals(x.Key.ToString(), readKeyInfo.KeyChar.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
        //            if (items.Count == 0)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                var i = _children.FindIndex(x => x == items[0]);
        //                SelectChildItem(i);
        //                if (items.Count == 1)
        //                {
        //                    //唯一项
        //                    items.First().Click();
        //                }
        //            }
        //        }

        //        //
        //        if (BreakLoop)
        //        {
        //            break;
        //        }
        //    }
        //}

        //private void AppendParent(MenuItem parent, List<MenuItem> parents)
        //{
        //    if (parent == null)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        parents.Add(parent);
        //        AppendParent(parent.Parent, parents);
        //    }
        //}

        ///// <summary>
        ///// 通知父菜单，有更新，应当重绘
        ///// </summary>
        //private void NoticeParentToRepaint()
        //{
        //    Parent?.RePaint();
        //}

        private bool _disable = false;
        /// <summary>
        /// if true，不响应click,select
        /// </summary>
        public bool Disable
        {
            get => _disable;
            set
            {
                if (_disable != value)
                {
                    _disable = value;
                }
            }
        }

        private bool _visible = true;
        /// <summary>
        /// if false，不显示，且不响应click,select
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                }
            }
        }

        /// <summary>
        /// 当前选择的child项
        /// </summary>
        public int SelectedSubmenuIndex { get; protected set; }

        private void ShowDescription()
        {
            if (Description != null)
            {
                SafeWriteLine(Description, false);
            }
        }
        private void ShowChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Visible)
                {
                    SafeWriteTitle(true, SelectedIndexIsValid && SelectedSubmenuIndex == i, Children[i].ForegroundColor);
                }
            }
        }

        /// <summary>
        /// 父菜单，顶层菜单时为null。
        /// </summary>
        public IMenuItem Parent { get; protected set; }

        private List<IMenuItem> _children = new List<IMenuItem>();
        /// <summary>
        /// 子菜单集合
        /// </summary>
        public IReadOnlyList<IMenuItem> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// 是否有子菜单
        /// </summary>
        private bool HasAnyChild
        {
            get { return _children.Count > 0; }
        }


        private char _key;
        /// <summary>
        /// 激活菜单的快捷键，只能是字母或数字
        /// </summary>
        public char Key
        {
            get => _key;
            set
            {
                if (char.IsLetterOrDigit(value))
                {
                    if (_key != value)
                    {
                        _key = value;
                    }
                }
                else
                {
                    throw new ArgumentException("Key must be a Letter or Digit");
                }
            }
        }

        private string _title;
        /// <summary>
        /// 菜单标题，不能为空。
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (_title != value)
                    {
                        _title = value;
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(Title));
                }
            }
        }

        /// <summary>
        /// 菜单项说明文字，nullable
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 返回短标题
        /// </summary>
        public string GetShortTitle(int shortTitleMaxLength)
        {

            if (Title.Length > shortTitleMaxLength)
            {
                return Title.Substring(0, shortTitleMaxLength) + "~";
            }
            else
            {
                return Title;
            }

        }

        public void SelectSubmenuItem(int childMenuItemIndex)
        {
            if (IsValidChildIndex(childMenuItemIndex))
            {
                SelectedSubmenuIndex = childMenuItemIndex;
            }
        }

        public void AddSubMenu(params IMenuItem[] subMenuItems)
        {
            _children.AddRange(subMenuItems);
        }

        public void AddSubMenu(int index, params IMenuItem[] subMenuItems)
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            _children.InsertRange(index, subMenuItems);
        }

        public void AttachParentMenu(IMenuItem parentMenuItem)
        {
            parentMenuItem.AddSubMenu(this);
            Parent = parentMenuItem;
        }

        public void Active(MenuContext menuContext)
        {
            if (HasAnyChild)
            {
                
            }
            else
            {

            }
        }

        private ConsoleColor? _color;
        /// <summary>
        /// 菜单前景色颜色
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get => _color ?? Console.ForegroundColor;
            set => _color = value;
        }
        ///// <summary>
        ///// 获取当菜单的绘制颜色
        ///// </summary>
        ///// <returns></returns>
        //protected ConsoleColor GetColor()
        //{
        //    return Disable ? DisabledItemForegroundColor : ForegroundColor;
        //}

        /// <summary>
        /// 菜单被点击时的事件。当子菜单集合非空时，直接激活为前台菜单，而不会引发此事件。
        /// </summary>
        public event EventHandler<MenuItemEventArgs> OnClick;

    }
}
