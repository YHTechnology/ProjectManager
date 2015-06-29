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
    public class ActionAndRoleActionEntity : NotifyPropertyChanged
    {
        public ICommand OnCheckBox { get; set; }

        public bool IsAccessSet { set; get; }

        public bool IsAccess
        {
            get
            {
                if (RoleActionEntity == null)
                {
                    return false;
                }
                else
                {
                    return RoleActionEntity.IsPermit;
                }
            }
            set
            {
                IsAccessSet = value;
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
                    UpdateChanged("IsAccess");
                    UpdateChanged("RoleActionEntity");
                }
            }
        }

        public ActionEntity ActionEntity { get; set; }

        public ObservableCollection<ActionAndRoleActionEntity> ChildList { get; set; }

        public ProductManager.Web.Service.SystemManageDomainContext SystemManageDomainContext { get; set; }

        public RoleEntity CurrentRoleEntity { get; set; }

        public ActionAndRoleActionEntity ParentActionAndRoleActionEntity { get; set; }

        public ActionAndRoleActionEntity()
        {
            OnCheckBox = new DelegateCommand(OnCheckBoxCommand);
        }

        private void CheckBoxCommand()
        {
            if (RoleActionEntity == null)
            {
                RoleActionEntity = new RoleActionEntity();
                RoleActionEntity.IsPermit = IsAccessSet;
                RoleActionEntity.RoleID = CurrentRoleEntity.RoleID;
                RoleActionEntity.ActionID = ActionEntity.ActionID;
                RoleActionEntity.RoleAction = new ProductManager.Web.Model.role_action();
                RoleActionEntity.DUpdate();
                SystemManageDomainContext.role_actions.Add(RoleActionEntity.RoleAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                RoleActionEntity.IsPermit = IsAccessSet;
                RoleActionEntity.DUpdate();
            }
        }

        private void OnCheckBoxCommand()
        {
            if (RoleActionEntity == null)
            {
                RoleActionEntity = new RoleActionEntity();
                RoleActionEntity.IsPermit = IsAccessSet;
                RoleActionEntity.RoleID = CurrentRoleEntity.RoleID;
                RoleActionEntity.ActionID = ActionEntity.ActionID;
                RoleActionEntity.RoleAction = new ProductManager.Web.Model.role_action();
                RoleActionEntity.DUpdate();
                SystemManageDomainContext.role_actions.Add(RoleActionEntity.RoleAction);
                UpdateChanged("IsAccess");
            }
            else
            {
                RoleActionEntity.IsPermit = IsAccessSet;
                RoleActionEntity.DUpdate();
            }

            if (!IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndRoleActionEntity actionAndRoleActionEntity in ChildList)
                    {
                        if (actionAndRoleActionEntity.RoleActionEntity != null)
                        {
                            actionAndRoleActionEntity.RoleActionEntity.IsPermit = IsAccessSet;
                            actionAndRoleActionEntity.IsAccessSet = IsAccessSet;
                            if (actionAndRoleActionEntity.ChildList != null)
                            {
                                actionAndRoleActionEntity.OnCheckBoxCommand();
                            }
                            else
                            {
                                actionAndRoleActionEntity.CheckBoxCommand();
                            }
                            actionAndRoleActionEntity.UpdateChanged("IsAccess");
                        }
                    }
                }
            }

            if (IsAccessSet)
            {
                if (ChildList != null)
                {
                    foreach (ActionAndRoleActionEntity actionAndRoleActionEntity in ChildList)
                    {
                        actionAndRoleActionEntity.IsAccessSet = IsAccessSet;
                        actionAndRoleActionEntity.OnCheckBoxCommand();
                    }
                }
            }


            if (ParentActionAndRoleActionEntity != null)
            {
                if (IsAccessSet == true)
                {
                    if (ParentActionAndRoleActionEntity.IsAccessSet == false)
                    {
                        ParentActionAndRoleActionEntity.IsAccessSet = true;
                        ParentActionAndRoleActionEntity.CheckBoxCommand();
                    }
                }
                ParentActionAndRoleActionEntity.UpdateChanged("IsAccess");
            }

            UpdateChanged("IsAccess");
        }
    }
}
