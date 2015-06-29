using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager
{
    [Export(typeof(IMainMenu))]
    public class SystemManagerMainMenu : IMainMenu
    {
        public String Content
        {
            get
            {
                return "系统管理";
            }
        }

        public String NavigateUri
        {
            get
            {
                return "SystemManager";
            }
        }

        public MenuType MenuType
        {
            get
            {
                return MenuType.SYSTEM_MANAGER_MAIN;
            }
        }

        public int ActionID
        {
            get
            {
                return 1000000;
            }
        }

        public int OrderNumber
        {
            get
            {
                return 3;
            }
        }
    }
}
