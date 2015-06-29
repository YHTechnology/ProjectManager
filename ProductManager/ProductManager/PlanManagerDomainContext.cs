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

namespace ProductManager.Web.Service
{
    public partial class PlanManagerDomainContext
    {
        partial void OnCreated()
        {
            TimeSpan lTimeSpan = new TimeSpan(0, 10, 0);
            TimeoutUtility.ChangeSentTimeout(this, lTimeSpan);
        }
    }
}
