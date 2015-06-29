using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager.Module
{
    public class UserInfo
    {
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public String UserName { get; set; }
        public String UserDepartment { get; set; }
        public String UserPassword { get; set; }
        public bool IsManager { get; set; }
        public Dictionary<int, bool> UserAction { get; set; }


        public bool GetUerRight(int aActionID)
        {
            if (UserAction != null)
            {
                bool lRight;
                if (UserAction.TryGetValue(aActionID, out lRight))
                {
                    return lRight;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
