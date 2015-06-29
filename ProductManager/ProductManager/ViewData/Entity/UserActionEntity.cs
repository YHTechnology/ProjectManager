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
    public class UserActionEntity : NotifyPropertyChanged
    {
        private int userActionID;
        public int UserActionID
        {
            get
            {
                return userActionID;
            }
            set
            {
                if (userActionID != value)
                {
                    userActionID = value;
                    UpdateChanged("UserActionID");
                }
            }
        }

        private int userID;
        public int UserID
        {
            get
            {
                return userID;
            }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    UpdateChanged("UserID");
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
            this.userActionID = UserAction.user_action_id;
            this.userID = UserAction.user_id.GetValueOrDefault(0);
            this.actionID = UserAction.action_id.GetValueOrDefault(0);
            this.isPermit = UserAction.isPermit.GetValueOrDefault(false);
        }

        public void DUpdate()
        {
            UserAction.user_action_id = this.userActionID;
            UserAction.user_id = this.userID;
            UserAction.action_id = this.actionID;
            UserAction.isPermit = this.isPermit;
        }

        public ProductManager.Web.Model.user_action UserAction { get; set; }
    }
}
