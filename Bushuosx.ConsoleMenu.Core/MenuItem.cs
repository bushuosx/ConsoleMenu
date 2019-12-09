using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 适用于ConsoleApplication的菜单类
    /// </summary>
    public class MenuItem
    {
        public MenuItem(string title)
        {
            Title = title;
        }
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClick) : this(title)
        {
            OnClick += onClick;
        }
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClick, char key) : this(title, onClick)
        {
            Key = key;
        }
        public MenuItem(string title, Action onClick) : this(title)
        {
            OnClick += (s, e) => onClick();
        }
        public MenuItem(string title, Action onClick, char key) : this(title, onClick)
        {
            Key = key;
        }
        protected MenuContext Context { get; private set; }

        //protected MenuStyle Style { get; set; }

        //protected struct CursorStatus
        //{
        //    public int Left;
        //    public int Top;
        //    public int Size;
        //    public bool Visible;
        //}
        //private CursorStatus CatchCursorStatus()
        //{
        //    return new CursorStatus { Left = Console.CursorLeft, Top = Console.CursorTop, Size = Console.CursorSize, Visible = Console.CursorVisible };
        //}
        //private void SetCursorStatus(CursorStatus cursorStatus)
        //{
        //    Console.SetCursorPosition(cursorStatus.Left, cursorStatus.Top);
        //    Console.CursorSize = cursorStatus.Size;
        //    Console.CursorVisible = cursorStatus.Visible;
        //}

        //protected struct SafeConsoleColor
        //{
        //    public ConsoleColor BackgroundColor;
        //    public ConsoleColor ForegroundColor;
        //}

        //protected SafeConsoleColor CatchConsoleColor()
        //{
        //    return new SafeConsoleColor { BackgroundColor = Console.BackgroundColor, ForegroundColor = Console.ForegroundColor };
        //}
        //protected void SetConsoleColor(SafeConsoleColor color)
        //{
        //    Console.BackgroundColor = color.BackgroundColor;
        //    Console.ForegroundColor = color.ForegroundColor;
        //}

        //protected SafeConsoleColor BuildSafeColor(ConsoleColor bColor, ConsoleColor fColor, bool intensity)
        //{
        //    var rst = new SafeConsoleColor { BackgroundColor = bColor, ForegroundColor = fColor };

        //    if (fColor == bColor)
        //    {
        //        if (rst.BackgroundColor == ConsoleColor.White)
        //        {
        //            rst.ForegroundColor = ConsoleColor.Black;
        //        }
        //        else if (rst.BackgroundColor == ConsoleColor.Black)
        //        {
        //            rst.ForegroundColor = ConsoleColor.White;
        //        }
        //        else
        //        {
        //            rst.BackgroundColor = ConsoleColor.Black;
        //        }
        //    }
        //    else if (intensity)
        //    {
        //        rst.BackgroundColor = fColor;
        //        rst.ForegroundColor = bColor;
        //    }
        //    return rst;
        //}

        ///// <summary>
        ///// console menu 环境
        ///// </summary>
        //protected struct ConsoleMenuContext
        //{
        //    public MenuStyle MenuStyle;
        //    public CursorStatus CursorStatus;
        //    public SafeConsoleColor ConsoleColor;
        //}


        protected ConsoleColor DisabledItemForegroundColor = ConsoleColor.DarkGray;

        //private CursorStatus? PointOnActive { get; set; }
        //private SafeConsoleColor? ColorOnActive { get; set; }
        //private bool? CursorVisibleOnActive { get; set; }

        //protected bool ShouldClearScreen { get; set; }

        /// <summary>
        /// 指示当前菜单是否正在展示
        /// </summary>
        protected bool IsActived { get; set; }

        /// <summary>
        /// 激活此菜单
        /// </summary>
        /// <param name="clearScreen"></param>
        public void Active(MenuStyle menuStyle)
        {
            Context = new MenuContext(menuStyle);

            Active();
        }

        protected void Active(MenuContext menuContext)
        {
            Context = menuContext;

            Active();
        }

        private void Active()
        {
            Enter();

            RePaint();

            LoopMessage();
        }

        private void PrepareDrawingContext()
        {
            switch (Context.Style.DrawingStyle)
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
                    throw new NotImplementedException(string.Format("未实现的绘制样式：{0}", Enum.GetName(typeof(MenuDrawingStyle), Context.Style.DrawingStyle)));
            }
        }

        /// <summary>
        /// 恢复保存的环境
        /// </summary>
        private void RestoreContext()
        {
            PrepareDrawingContext();
        }

        /// <summary>
        /// 初始化一些参数
        /// </summary>
        private void Enter()
        {
            BreakLoop = false;
            SelectedIndex = 0;
            //Console.CursorVisible = false;
            IsActived = true;
        }
        private void Exit()
        {
            BreakLoop = true;
            IsActived = false;
        }

        protected void SafeWriteTitle(bool showKey, bool isSelected)
        {
            var txt = BuildTitleString(showKey);
            SafeWriteLine(txt, isSelected);
        }

        protected void SafeWrite(string text)
        {
            var oc = new SafeConsoleColor();
            var f = GetColor();
            var sc = new SafeConsoleColor(oc.BackgroundColor, f, false);
            sc.SetToConsole();
            Console.Write(text);
            oc.SetToConsole();
        }

        protected void SafeWriteLine(string text, bool intensity)
        {
            var oc = new SafeConsoleColor();
            var f = GetColor();
            var sc = new SafeConsoleColor(oc.BackgroundColor, f, intensity);
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

            var titleStyle = Context == null ? Parent.Context.Style.TitleStyle : Context.Style.TitleStyle;

            switch (titleStyle)
            {
                case MenuItemTitleStyle.KeyFront:
                    return char.IsLetterOrDigit(Key) ? $"{Key.ToString().ToUpper()} - {Title}" : $"? - {Title}";
                case MenuItemTitleStyle.KeyEnd:
                    return char.IsLetterOrDigit(Key) ? $"{Title}({Key.ToString().ToUpper()})" : Title;
                default:
                    throw new NotImplementedException($"{titleStyle.ToString()}未实现");
            }
        }

        private void ShowParentPath()
        {
            var ps = GetParents();
            SafeWrite("菜单路径：");

            if (ps.Count > 0)
            {
                ps.Reverse();//反转path
                foreach (var item in ps)
                {
                    item.SafeWrite(item.ShortTitle + " >> ");
                }
            }

            SafeWrite(ShortTitle);
            //Console.Write(Console.Out.NewLine);
        }

        /// <summary>
        /// 获取父菜单列表，由近及远排列
        /// </summary>
        /// <returns></returns>
        public List<MenuItem> GetParents()
        {
            var rst = new List<MenuItem>();
            AppendParent(Parent, rst);
            return rst;
        }

        protected void RePaint()
        {
            if (IsActived)
            {
                PrepareDrawingContext();

                OnBeforePaint?.Invoke(this, null);

                ShowParentPath();
                Console.WriteLine();

                SafeWriteTitle(false, false);
                ShowDescription();

                Console.WriteLine();
                ShowChildren();

                OnAfterPaint?.Invoke(this, null);
            }
        }

        //protected void RePaintOnChanged()
        //{
        //    if (IsActived)
        //    {
        //        RePaint();
        //    }
        //}

        private bool IsCanBeSelectItem(MenuItem item)
        {
            return item.Visible && !item.Disabled;
        }
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
                var up = SelectedIndex - 1;
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
                    if (SelectedIndex != last)
                    {
                        return last;
                    }
                }
            }

            return SelectedIndex;
        }

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
                var down = SelectedIndex + 1;
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
                    if (SelectedIndex != first)//当前前面还有元素
                    {
                        return first;
                    }
                }
            }

            return SelectedIndex;
        }

        protected bool SelectedIndexIsValid
        {
            get { return IsValidChildIndex(SelectedIndex); }
        }
        private bool IsValidChildIndex(int index)
        {
            return index >= 0 && index < Children.Count;
        }

        private bool BreakLoop { get; set; }

        /// <summary>
        /// 循环读取用户按键，响应事件
        /// </summary>
        private void LoopMessage()
        {
            while (true)
            {
                var readKeyInfo = Console.ReadKey(true);
                var key = readKeyInfo.Key;
                if (key == ConsoleKey.UpArrow)
                {
                    SelectChildItem(GetUpChildItemIndex());
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    SelectChildItem(GetDownChildItemIndex());
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (SelectedIndexIsValid)
                    {
                        Children[SelectedIndex].Click();
                    }
                }
                else if (key == ConsoleKey.Escape)
                {
                    Close();
                    //BreakLoop = true;
                }
                else
                {
                    //其它按键
                    var items = Children.Where(x => char.IsLetterOrDigit(x.Key) && IsCanBeSelectItem(x) && string.Equals(x.Key.ToString(), readKeyInfo.KeyChar.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                    if (items.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        var i = _children.FindIndex(x => x == items[0]);
                        SelectChildItem(i);
                        if (items.Count == 1)
                        {
                            //唯一项
                            items.First().Click();
                        }
                    }
                }

                ///
                if (BreakLoop)
                {
                    break;
                }
            }
        }

        private void AppendParent(MenuItem parent, List<MenuItem> parents)
        {
            if (parent == null)
            {
                return;
            }
            else
            {
                parents.Add(parent);
                AppendParent(parent.Parent, parents);
            }
        }

        /// <summary>
        /// 通知父菜单，有更新，应当重绘
        /// </summary>
        private void NoticeParentToRepaint()
        {
            Parent?.RePaint();
        }

        private bool _disabled = false;
        /// <summary>
        /// if true，不响应click,select
        /// </summary>
        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    NoticeParentToRepaint();
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
                    NoticeParentToRepaint();
                }
            }
        }

        /// <summary>
        /// 当前选择的child项
        /// </summary>
        public int SelectedIndex { get; protected set; }

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
                    Children[i].SafeWriteTitle(true, SelectedIndexIsValid && SelectedIndex == i);
                }
            }
        }


        public MenuItem Parent { get; protected set; }

        protected List<MenuItem> _children = new List<MenuItem>();
        public IReadOnlyList<MenuItem> Children
        {
            get { return _children; }
        }

        public bool HasAnyChild
        {
            get { return _children.Count > 0; }
        }

        public void AddSubMenu(MenuItem menu)
        {
            AddSubMenu(_children.Count, menu);
        }
        public void AddSubMenu(int index, MenuItem menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            menu.Parent = this;
            _children.Insert(index, menu);

            //动态更新
            RePaint();
        }

        public void AddSubMenu(int index, IEnumerable<MenuItem> menus)
        {
            if (menus == null)
            {
                throw new ArgumentNullException(nameof(menus));
            }
            foreach (var item in menus)
            {
                item.Parent = this;
            }
            _children.InsertRange(index, menus);

            //动态更新
            RePaint();
        }
        public void AddSubMenu(IEnumerable<MenuItem> menus)
        {
            AddSubMenu(_children.Count, menus);
        }

        public void AtachToMenu(MenuItem parentMenu)
        {
            if (parentMenu == null)
            {
                throw new ArgumentNullException(nameof(parentMenu));
            }
            parentMenu.AddSubMenu(this);
        }

        protected void RaiseEvent(EventHandler<MenuItemEventArgs> eventHandler, MenuItemEventArgs eventArgs)
        {
            eventHandler?.Invoke(this, eventArgs);
        }

        private char _key;
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
                        NoticeParentToRepaint();
                    }
                }
                else
                {
                    throw new ArgumentException("Key must be a Letter or Digit");
                }
            }
        }

        private string _title;
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
                        NoticeParentToRepaint();
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(Title));
                }
            }
        }

        public string Description { get; set; }

        public string ShortTitle
        {
            get
            {
                if (Title.Length > ShortTitleLength)
                {
                    return Title.Substring(0, ShortTitleLength) + "~";
                }
                else
                {
                    return Title;
                }
            }
        }

        protected int ShortTitleLength { get; set; } = 6;

        private ConsoleColor? _color;
        public ConsoleColor? Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    NoticeParentToRepaint();
                }
            }
        }
        protected ConsoleColor GetColor()
        {
            return Disabled ? DisabledItemForegroundColor : Color ?? Console.ForegroundColor;
        }


        public event EventHandler<MenuItemEventArgs> OnClick;
        protected void RaiseClick(MenuItemEventArgs eventArgs)
        {
            if (HasAnyChild)
            {
                //active
                Active(Parent?.Context);
            }
            else
            {
                RaiseEvent(OnClick, eventArgs);
            }
        }
        public void Click()
        {
            if (Visible && !Disabled)
            {
                RaiseClick(null);
            }
        }

        //public EventHandler<MenuItemEventArgs> OnChildrenChanged { get; }
        //protected void RaiseChildrenChange(MenuItemEventArgs eventArgs)
        //{
        //    RaiseEvent(OnChildrenChanged, eventArgs);
        //}

        public event EventHandler<MenuItemEventArgs> OnClosing;
        public void Close()
        {
            MenuItemEventArgs eventArgs = new MenuItemEventArgs();

            RaiseEvent(OnClosing, eventArgs);

            if (!eventArgs.IsCanceled)
            {
                //跳出本地循环
                RestoreContext();
                Exit();
                Parent?.Active();
            }
        }

        public event EventHandler<MenuItemEventArgs> OnSelectChanged;
        //public void SelectItem(int index)
        //{
        //    if (IsValidChildIndex(index))
        //    {
        //        SelectedIndex = index;
        //        RaiseEvent(OnSelectChange, null);
        //        RePaint();
        //    }
        //}
        protected void SelectChildItem(int index)
        {
            if (SelectedIndex != index && IsValidChildIndex(index))
            {
                SelectedIndex = index;
                RaiseEvent(OnSelectChanged, null);
                RePaint();
            }
        }

        public event EventHandler<MenuItemEventArgs> OnBeforePaint;
        public event EventHandler<MenuItemEventArgs> OnAfterPaint;
    }
}
