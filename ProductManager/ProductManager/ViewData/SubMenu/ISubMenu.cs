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
    public interface ISubMenu
    {
        String Content { get; }
        String NavigateUri { get; }
        SubMenuType MenuType { get; }
        int ActionID { get; }
        int OrderNumber { get; }
    }
}
