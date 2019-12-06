using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Bushuosx.ConsoleMenu
{
    public class MenuItem
    {
        public MenuItem(string title)
        {
            Title = title;
            //OnClosing += (s, e) => Reset();
        }
        public MenuItem(string title, EventHandler<MenuItemEventArgs> onClicked) : this(title)
        {
            OnClick += onClicked;
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

        protected bool ShouldScreen { get; set; }
        public void Active(bool clearScreen = true)
        {
            Init();
            ShouldScreen = clearScreen;

            RePaint();

            LoopMessage();
        }

        private void Init()
        {
            PointOnActive = CatchCursorPoint();
            ColorOnActive = CatchConsoleColor();
            CursorVisibleOnActive = Console.CursorVisible;

            BreakLoop = false;
            SelectedIndex = 0;
            //Console.CursorVisible = false;
        }

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
            var f = Disabled ? DisabledItemForegroundColor : GetColor();
            var sc = GetSafeColor(oc.BackgroundColor, f, false);
            SetConsoleColor(sc);
            Console.Write(text);
            SetConsoleColor(oc);
        }

        protected void SafeWriteLine(string text, bool intensity)
        {
            var oc = CatchConsoleColor();
            var f = Disabled ? DisabledItemForegroundColor : GetColor();
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


        public List<MenuItem> GetParents()
        {
            var rst = new List<MenuItem>(10);//业务上一般10层够用了，默认10层为了减少内存分配次数
            AppendParent(Parent, rst);
            //rst.Reverse();//反转数据
            return rst;
        }

        protected void RePaint()
        {
            if (ShouldScreen)
            {
                Console.Clear();
            }
            //Console.SetCursorPosition(PointOnActive.Left, PointOnActive.Top);

            ShowParentPath();
            Console.WriteLine();

            SafeWriteTitle(false, false);
            ShowDescription();

            Console.WriteLine();
            ShowChildren();
        }


        private void SelectUpItem()
        {
            if (!SelectedIndexIsValid)
            {
                if (IsValidChildIndex(0))
                {
                    SelectedIndex = 0;
                    RePaint();
                }
            }
            else
            {
                var i = SelectedIndex - 1;
                if (IsValidChildIndex(i))
                {
                    SelectedIndex = i;
                    RePaint();
                }
                else
                {
                    var last = Children.Count - 1;//末尾元素
                    if (SelectedIndex < last)
                    {
                        SelectedIndex = last;
                        RePaint();
                    }
                }
            }
        }
        private void SelectDownItem()
        {
            if (!SelectedIndexIsValid)
            {
                if (IsValidChildIndex(0))
                {
                    SelectedIndex = 0;
                    RePaint();
                }
            }
            else
            {
                var i = SelectedIndex + 1;
                if (IsValidChildIndex(i))
                {
                    SelectedIndex = i;
                    RePaint();
                }
                else
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex = 0;
                        RePaint();
                    }
                }
            }
        }

        protected bool SelectedIndexIsValid
        {
            get { return IsValidChildIndex(SelectedIndex); }
        }
        private bool IsValidChildIndex(int index)
        {
            return index >= 0 && index < Children.Count;
        }

        protected bool BreakLoop { get; set; }
        private void LoopMessage()
        {
            while (true)
            {
                var readKeyInfo = Console.ReadKey(true);
                var key = readKeyInfo.Key;
                if (key == ConsoleKey.UpArrow)
                {
                    SelectUpItem();
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    SelectDownItem();
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
                    var items = Children.Where(x => x.Key.HasValue && string.Equals(x.Key.Value.ToString(), readKeyInfo.KeyChar.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                    if (items.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        var i = _children.FindIndex(x => x == items[0]);
                        RaiseSelectChange(i);
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

        public bool Disabled { get; set; }

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
                Children[i].SafeWriteTitle(true, SelectedIndexIsValid && SelectedIndex == i);
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
            return Color ?? Console.ForegroundColor;
        }


        public event EventHandler<MenuItemEventArgs> OnClick;
        protected void RaiseClick(MenuItemEventArgs eventArgs)
        {
            if (HasAnyChild)
            {
                Active(Parent.ShouldScreen);
            }
            else
            {
                RaiseEvent(OnClick, eventArgs);
            }
        }
        public void Click()
        {
            RaiseClick(null);
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
                Parent?.Active(ShouldScreen);
            }
        }

        public event EventHandler<MenuItemEventArgs> OnSelectChange;
        //public void SelectItem(int index)
        //{
        //    if (IsValidChildIndex(index))
        //    {
        //        SelectedIndex = index;
        //        RaiseEvent(OnSelectChange, null);
        //        RePaint();
        //    }
        //}
        protected void RaiseSelectChange(int index)
        {
            if (IsValidChildIndex(index))
            {
                SelectedIndex = index;
                RaiseEvent(OnSelectChange, null);
                RePaint();
            }
        }
    }
}
