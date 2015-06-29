using System;
using System.Collections.ObjectModel;
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
    public class ActionEntity : NotifyPropertyChanged
    {
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

        private String actionName;
        public String ActionName
        {
            get
            {
                return actionName;
            }
            set
            {
                if (actionName != value)
                {
                    actionName = value;
                    UpdateChanged("ActionName");
                }
            }
        }

        private int actionCode;
        public int ActionCode
        {
            get
            {
                return actionCode;
            }
            set
            {
                if (actionCode != value)
                {
                    actionCode = value;
                    UpdateChanged("ActionCode");
                }
            }
        }

        private int actionType;
        public int ActionType
        {
            get
            {
                return actionType;
            }
            set
            {
                if (actionType != value)
                {
                    actionType = value;
                    UpdateChanged("ActionType");
                }
            }
        }

        private int supperActionID;
        public int SupperActionID
        {
            get
            {
                return supperActionID;
            }
            set
            {
                if (supperActionID != value)
                {
                    supperActionID = value;
                    UpdateChanged("SupperActionID");
                }
            }
        }

        public void Update()
        {
            this.actionID = Action.action_id;
            this.actionName = Action.action_name;
            if (Action.action_code.HasValue)
            {
                this.actionCode = Action.action_code.Value;
            }
            else
            {
                this.actionCode = 0;
            }
            if (Action.action_type.HasValue)
            {
                this.actionType = Action.action_type.Value;
            }
            else
            {
                this.actionType = 0;
            }
            if (Action.supper_action_id.HasValue)
            {
                this.supperActionID = Action.supper_action_id.Value;
            }
            else
            {
                this.supperActionID = 0;
            }
        }

        public ObservableCollection<ActionEntity> ChildAction { get; set; }

        public ProductManager.Web.Model.action Action { get; set; }
    }
}
