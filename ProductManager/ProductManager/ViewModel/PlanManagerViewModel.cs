using System;
using System.Collections.Generic;
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

namespace ProductManager.ViewModel
{
    [Export("PlanManager")]
    public class PlanManagerViewModel : NotifyPropertyChanged
    {
        [ImportMany(typeof(ISubMenu))]
        public List<ISubMenu> subMenuList { get; set; }

        private List<ISubMenu> _subMenuList = new List<ISubMenu>();

        public List<ISubMenu> SubMenuList
        {
            get
            {
                _subMenuList.Clear();
                foreach (ISubMenu subMenu in subMenuList)
                {
                    App app = Application.Current as App;
                    bool isPermit;
                    if (subMenu.MenuType == SubMenuType.PLAN_MANAGER_SUBMENU
                        && app.UserInfo.UserAction.TryGetValue(subMenu.ActionID, out isPermit))
                    {
                        if (isPermit)
                        {
                            _subMenuList.Add(subMenu);
                        }
                    }
                }
                _subMenuList.Sort((a, b) => a.OrderNumber.CompareTo(b.OrderNumber));
                return _subMenuList;
            }
        }

        private bool isExpand = false;
        public bool IsExpand
        {
            get
            {
                return isExpand;
            }
            set
            {
                if (isExpand != value)
                {
                    isExpand = value;
                    UpdateChanged("IsExpand");
                }
            }
        }

        private String currentPageName;
        public String CurrentPageName
        {
            get
            {
                return currentPageName;
            }
            set
            {
                if (currentPageName != value)
                {
                    currentPageName = value;
                    UpdateChanged("CurrentPageName");
                }
            }
        }
    }
}
