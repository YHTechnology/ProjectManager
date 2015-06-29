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
    public class ActionAndUserActionEntity : NotifyPropertyChanged
    {

        public ICommand OnCheckBox { get; set; }

        public bool IsAccessSet { set; get; }

        public bool IsAccess
        {
            get
            {
                if (UserActionEntity != null)
                {
                    return UserActionEntity.IsPermit;
                }
                if (RoleActionEntity != null)
                {
                    return RoleActionEntity.IsPermit;
                }
                return false;
            }
            set
            {
                IsAccessSet = value;
            }

        }

        private UserActionEntity userActionEntity;
        public UserActionEntity UserActionEntity
        {
            get
            {
                return userActionEntity;
            }
            set
            {
                if (userActionEntity != value)
                {
                    userActionEntity = value;
                    UpdateChanged("IsAccess");
                    UpdateChanged("UserActionEntity");
                }
            }
        }

        private RoleActionEntity roleActionEntity;
        public RoleActionEntity RoleActionEntity
        {
            get
            {
                return roleActionEntity;
            }
            set
            {
                if (roleActionEntity != value)
                {
                    roleActionEntity = value;
                    //UpdateChanged("IsAccess");
                    UpdateChanged("RoleActionEntity");
                }
            }
        }

        public ActionEntity ActionEntity { get; set; }

        public ObservableCollection<ActionAndUserActionEntity> ChildList { get; set; }

        public ActionAndUserActionEntity ParentActionAdnUserActionEntity { get; set; }

        public ProductManager.Web.Service.SystemManageDomainContext SystemManageDomainContext { get; set; }

        public UserEntity CurrentSelectUserEntity { get; set; }

        public ActionAndUserActionEntity()
        {
            OnCheckBox = new DelegateCommand(OnCheckBoxCommand);
        }

        private void CheckBoxCommand()
        {
            if (UserActionEntity == null)
            {
                UserActionEntity = new UserActionEntity();
                UserActionEntity.IsPermit = IsAccessSet;
                UserActionEntity.ActionID = ActionEntity.ActionID;
                UserActionEntity.UserID = CurrentSelectUserEntity.UserID;
                UserActionEntity.UserAction = new ProductManager.Web.Model.user_action();
                UserActionEntity.DUpdate();
                SystemManageDomainContext.user_actions.Add(UserActionEntity.UserAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                UserActionEntity.IsPermit = IsAccessSet;
                UserActionEntity.DUpdate();
            }
        }

        private void OnCheckBoxCommand()
        {
            if (UserActionEntity == null)
            {
                UserActionEntity = new UserActionEntity();
                UserActionEntity.IsPermit = IsAccessSet;
                UserActionEntity.ActionID = ActionEntity.ActionID;
                UserActionEntity.UserID = CurrentSelectUserEntity.UserID;
                UserActionEntity.UserAction = new ProductManager.Web.Model.user_action();
                UserActionEntity.DUpdate();
                SystemManageDomainContext.user_actions.Add(UserActionEntity.UserAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                UserActionEntity.IsPermit = IsAccessSet;
                UserActionEntity.DUpdate();
            }

            if (!IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndUserActionEntity actionAndUserActionEntity in ChildList)
                    {
                        if (actionAndUserActionEntity.UserActionEntity != null)
                        {
                            actionAndUserActionEntity.UserActionEntity.IsPermit = IsAccessSet;
                            actionAndUserActionEntity.IsAccessSet = IsAccessSet;
                            if (actionAndUserActionEntity.ChildList != null)
                            {
                                actionAndUserActionEntity.OnCheckBoxCommand();
                            }
                            else
                            {
                                actionAndUserActionEntity.CheckBoxCommand();
                            }
                            actionAndUserActionEntity.UpdateChanged("IsAccess");
                        }
                    }
                }
            }

            if (IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndUserActionEntity actionAndUserActionEntity in ChildList)
                    {
                        actionAndUserActionEntity.IsAccessSet = IsAccessSet;
                        actionAndUserActionEntity.OnCheckBoxCommand();
                    }
                }
            }

            if (ParentActionAdnUserActionEntity != null)
            {
                if (IsAccessSet == true)
                {
                    if (ParentActionAdnUserActionEntity.IsAccessSet == false)
                    {
                        ParentActionAdnUserActionEntity.IsAccessSet = true;
                        ParentActionAdnUserActionEntity.CheckBoxCommand();
                    }
                    ParentActionAdnUserActionEntity.UpdateChanged("IsAccess");
                }
            }

            UpdateChanged("IsAccess");
        }
    }
}
