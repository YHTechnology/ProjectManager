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

namespace ProductManager.ViewData.Entity
{
    public class RoleEntity : NotifyPropertyChanged
    {
        private int roleID;
        public int RoleID
        {
            get
            {
                return roleID;
            }
            set
            {
                if (roleID != value)
                {
                    roleID = value;
                    UpdateChanged("RoleID");
                }
            }
        }

        private String roleName;
        public String RoleName
        {
            get
            {
                return roleName;
            }
            set
            {
                if (roleName != value)
                {
                    roleName = value;
                    UpdateChanged("RoleName");
                }
            }
        }

        public void Update()
        {
            this.roleID = Role.role_id;
            this.roleName = Role.role_name;
        }

        public ProductManager.Web.Model.role Role { get; set; }
    }
}
