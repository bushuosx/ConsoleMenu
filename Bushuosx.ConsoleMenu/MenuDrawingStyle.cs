using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 菜单激活时，处理console屏幕的方式
    /// </summary>
    public enum MenuDrawingStyle
    {
        /// <summary>
        /// 每次激活菜单前清屏
        /// </summary>
        ClearSreen = 0,
        /// <summary>
        /// 继承父菜单启动点
        /// </summary>
        InheritStartPoint = 1,
        /// <summary>
        /// 依照系统的console流模式
        /// </summary>
        FollowSystem
    }
}
