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
            //OnClosing += (s, e) => Reset();
        }
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClick) : this(title)
        {
            OnClick += onClick;
        }
        public MenuItem(string title, Action onClick) : this(title)
        {
            OnClick += (s, e) => onClick();
        }

        private struct Point
        {
            public Point(int left, int top)
            {
                Left = left;
                Top = top;
            }
            public int Left;
            public int Top;
        }

        private Point CatchCursorPoint()
        {
            return new Point(Console.CursorLeft, Console.CursorTop);
        }

        private Point? PointOnActive { get; set; }
        private SafeColor? ColorOnActive { get; set; }
        private bool? CursorVisibleOnActive { get; set; }

        protected bool ShouldClearScreen { get; set; }

        /// <summary>
        /// 激活此菜单
        /// </summary>
        /// <param name="clearScreen"></param>
        public void Active(bool clearScreen = true)
        {
            Init();
            ShouldClearScreen = clearScreen;

            RePaint();

            LoopMessage();
        }

        /// <summary>
        /// 保存环境，初始化一些参数
        /// </summary>
        private void Init()
        {
            PointOnActive = CatchCursorPoint();
            ColorOnActive = CatchConsoleColor();
            CursorVisibleOnActive = Console.CursorVisible;

            BreakLoop = false;
            SelectedIndex = 0;
            //Console.CursorVisible = false;
        }

        /// <summary>
        /// 恢复保存的环境
        /// </summary>
        private void Reset()
        {
            if (ColorOnActive.HasValue)
            {
                Console.BackgroundColor = ColorOnActive.Value.BackgroundColor;
                Console.ForegroundColor = ColorOnActive.Value.ForegroundColor;
            }
            if (CursorVisibleOnActive.HasValue)
            {
                Console.CursorVisible = CursorVisibleOnActive.Value;
            }
        }


        protected void SafeWriteTitle(bool showKey, bool isSelected)
        {
            var txt = BuildTitleString(showKey);
            SafeWriteLine(txt, isSelected);
        }

        protected void SafeWrite(string text)
        {
            var oc = CatchConsoleColor();
            var f = GetColor();
            var sc = GetSafeColor(oc.BackgroundColor, f, false);
            SetConsoleColor(sc);
            Console.Write(text);
            SetConsoleColor(oc);
        }

        protected void SafeWriteLine(string text, bool intensity)
        {
            var oc = CatchConsoleColor();
            var f = GetColor();
            var sc = GetSafeColor(oc.BackgroundColor, f, intensity);
            SetConsoleColor(sc);
            Console.WriteLine(text);
            SetConsoleColor(oc);
        }

        public MenuItemTitleStyle TitleStyle { get; set; } = MenuItemTitleStyle.KeyEnd;
        private string BuildTitleString(bool showKey)
        {
            if (!showKey)
            {
                return Title;
            }

            if (TitleStyle == MenuItemTitleStyle.KeyFront)
            {
                return Key.HasValue ? $"{Key.Value.ToString().ToUpper()} - {Title}" : $"  - {Title}";

            }
            else if (TitleStyle == MenuItemTitleStyle.KeyEnd)
            {
                return Key.HasValue ? $"{Title}({Key.Value.ToString().ToUpper()})" : Title;

            }

            throw new NotImplementedException($"{TitleStyle.ToString()}未实现");
        }

        private void ShowParentPath()
        {
            var ps = GetParents();
            SafeWrite("路径：");

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
            var rst = new List<MenuItem>(10);//业务上一般10层够用了，默认10层为了减少内存分配次数
            AppendParent(Parent, rst);
            //rst.Reverse();//反转数据
            return rst;
        }

        protected void RePaint()
        {
            if (ShouldClearScreen)
            {
                Console.Clear();
            }
            //Console.SetCursorPosition(PointOnActive.Left, PointOnActive.Top);

            OnBeforePaint?.Invoke(this, null);

            ShowParentPath();
            Console.WriteLine();

            SafeWriteTitle(false, false);
            ShowDescription();

            Console.WriteLine();
            ShowChildren();

            OnAfterPaint?.Invoke(this, null);
        }

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
                    var items = Children.Where(x => x.Key.HasValue && IsCanBeSelectItem(x) && string.Equals(x.Key.Value.ToString(), readKeyInfo.KeyChar.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
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
        /// if true，不响应click,select
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// if false，不显示，且不响应click,select
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 当前选择的child项
        /// </summary>
        public int SelectedIndex { get; set; }

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

        protected ConsoleColor DisabledItemForegroundColor = ConsoleColor.DarkGray;

        protected SafeColor CatchConsoleColor()
        {
            return new SafeColor(Console.BackgroundColor, Console.ForegroundColor);
        }
        protected void SetConsoleColor(SafeColor color)
        {
            Console.BackgroundColor = color.BackgroundColor;
            Console.ForegroundColor = color.ForegroundColor;
        }

        protected struct SafeColor
        {
            public SafeColor(ConsoleColor bColor, ConsoleColor fColor)
            {
                BackgroundColor = bColor;
                ForegroundColor = fColor;
            }
            public ConsoleColor BackgroundColor;
            public ConsoleColor ForegroundColor;
        }

        protected SafeColor GetSafeColor(ConsoleColor bColor, ConsoleColor fColor, bool intensity)
        {
            var rst = new SafeColor(bColor, fColor);

            if (fColor == bColor)
            {
                if (rst.BackgroundColor == ConsoleColor.White)
                {
                    rst.ForegroundColor = ConsoleColor.Black;
                }
                else if (rst.BackgroundColor == ConsoleColor.Black)
                {
                    rst.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    rst.BackgroundColor = ConsoleColor.Black;
                }
            }
            else if (intensity)
            {
                rst.BackgroundColor = fColor;
                rst.ForegroundColor = bColor;
            }
            return rst;
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
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }
            //var m = menu.MemberwiseClone() as MenuItem;
            menu.Parent = this;
            _children.Add(menu);
        }

        public void AddSubMenu(IEnumerable<MenuItem> menus)
        {
            if (menus == null)
            {
                throw new ArgumentNullException(nameof(menus));
            }
            foreach (var item in menus)
            {
                item.Parent = this;
                _children.Add(item);
            }
        }

        protected void RaiseEvent(EventHandler<MenuItemEventArgs> eventHandler, MenuItemEventArgs eventArgs)
        {
            eventHandler?.Invoke(this, eventArgs);
        }

        private char? _key = null;
        public char? Key
        {
            get { return _key; }
            set
            {
                if (value.HasValue && char.IsLetterOrDigit(value.Value))
                {
                    _key = value.Value;
                }
            }
        }

        private string _title = "-- no title --";
        public string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _title = value;
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
                    return Title.Substring(0, ShortTitleLength) + "…";
                }
                else
                {
                    return Title;
                }
            }
        }

        protected int ShortTitleLength { get; set; } = 6;

        public ConsoleColor? Color { get; set; }
        protected ConsoleColor GetColor()
        {
            return Disabled ? DisabledItemForegroundColor : Color ?? Console.ForegroundColor;
        }


        public event EventHandler<MenuItemEventArgs> OnClick;
        protected void RaiseClick(MenuItemEventArgs eventArgs)
        {
            if (HasAnyChild)
            {
                Active(Parent.ShouldClearScreen);
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
                Reset();
                BreakLoop = true;
                Parent?.Active(ShouldClearScreen);
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
