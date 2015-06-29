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
    //[Export(typeof(ISubMenu))]
    public class PlanStatisticsSubMenu : ISubMenu
    {
        public String Content
        {
            get
            {
                return "计划查询";
            }
        }

        public String NavigateUri
        {
            get
            {
                return "PlanStatistics";
            }
        }

        public SubMenuType MenuType
        {
            get
            {
                return SubMenuType.PLAN_MANAGER_SUBMENU;
            }
        }

        public int ActionID
        {
            get
            {
                return 3040000;
            }
        }

        public int OrderNumber
        {
            get
            {
                return 4;
            }
        }
    }
}
