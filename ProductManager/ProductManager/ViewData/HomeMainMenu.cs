using System;
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
    public class HomeMainMenu : IMainMenu
    {
        public String Content
        {
            get
            {
                return "首页";
            }
        }

        public String NavigateUri
        {
            get
            {
                return "Home";
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
                return 0;
            }
        }

        public int OrderNumber
        {
            get
            {
                return 0;
            }
        }
    }
}
