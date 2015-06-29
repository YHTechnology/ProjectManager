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
    [Export(typeof(ISubMenu))]
    public class RoleRightManagerSubMenu: ISubMenu
    {
        public String Content
        {
            get
            {
                return "角色权限管理";
            }
        }

        public String NavigateUri
        {
            get
            {
                return "RoleRightManager";
            }
        }

        public SubMenuType MenuType
        {
            get
            {
                return SubMenuType.SYSTEM_MANAGER_SUBMENU;
            }
        }

        public int ActionID
        {
            get
            {
                return 1030000;
            }
        }

        public int OrderNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
