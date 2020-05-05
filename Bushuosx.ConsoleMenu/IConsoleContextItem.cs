using System;
using System.Collections.Generic;
using System.Text;

namespace Bushuosx.ConsoleMenu
{
    /// <summary>
    /// 用于保存console环境的项
    /// </summary>
    public interface IConsoleContextItem
    {
        /// <summary>
        /// 恢复到console
        /// </summary>
        void SetToConsole();
    }
}
