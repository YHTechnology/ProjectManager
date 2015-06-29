using System;
using System.ComponentModel.Composition;

namespace ProductManager
{
    [Export(typeof(ISubMenu))]
    public class ProductTypeSubMenu : ISubMenu
    {
        public String Content
        {
            get
            {
                return "产品类型管理";
            }
        }

        public String NavigateUri
        {
            get
            {
                return "ProductTypeManager";
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
                return 1050000;
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
