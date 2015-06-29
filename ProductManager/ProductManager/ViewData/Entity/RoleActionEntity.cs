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
    public class RoleActionEntity : NotifyPropertyChanged
    {
        private int roleActionID;
        public int RoleActionID
        {
            get
            {
                return roleActionID;
            }
            set
            {
                if (roleActionID != value)
                {
                    roleActionID = value;
                    UpdateChanged("RoleActionID");
                }
            }
        }

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

        private int actionID;
        public int ActionID
        {
            get
            {
                return actionID;
            }
            set
            {
                if (actionID != value)
                {
                    actionID = value;
                    UpdateChanged("ActionID");
                }
            }
        }

        private bool isPermit;
        public bool IsPermit
        {
            get
            {
                return isPermit;
            }
            set
            {
                if (isPermit != value)
                {
                    isPermit = value;
                    UpdateChanged("IsPermit");
                }
            }
        }

        public void Update()
        {
            this.roleActionID = RoleAction.role_action_id;
            this.roleID = RoleAction.role_id.GetValueOrDefault(0);
            this.actionID = RoleAction.action_id.GetValueOrDefault(0);
            this.isPermit = RoleAction.isPermit.GetValueOrDefault(false);
        }

        public void DUpdate()
        {
            RoleAction.role_action_id = this.roleActionID;
            RoleAction.role_id = this.roleID;
            RoleAction.action_id = this.actionID;
            RoleAction.isPermit = this.isPermit;
        }

        public ProductManager.Web.Model.role_action RoleAction { get; set; }
    }
}
