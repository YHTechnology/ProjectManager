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
    public enum MenuType : uint
    {
        //Main Menu
        SYSTEM_MANAGER_MAIN = 1,
        PRODUCT_MANAGER_MAIN = 2,
        PLAN_MANAGER_MAIN = 4,

        MAIN_MAX = 99,
        //SubMenu
        RIGHT_MANAGER_SUBMENU = 100,
        USER_MANAGER_SUBMENU = 101,
        ROLERIGHT_MANAGER_SUPMENU = 102,
        SYSTEM_MANAGER_SUBMENU_MAX = 199,
        

        PLAN_EIDT_SUBMENU = 200,
        PLAN_TRACE_SUBMENU = 201,
        PLAN_EVALUATE_SUBMENU = 201,
        PLAN_MANAGER_SUBMENU_MAX = 299
    }
}
