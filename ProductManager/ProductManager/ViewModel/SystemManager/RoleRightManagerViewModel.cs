using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.SystemManager
{
    [Export("RoleRightManager")]
    public class RoleRightManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;// = new SystemManageDomainContext();

        private OperationDomainContext operationDomainComtext = new OperationDomainContext();

        public ObservableCollection<ActionEntity> ActionEntityList { get; set; }

        public ObservableCollection<RoleActionEntity> RoleActionEntityList { get; set; }

        public ObservableCollection<RoleEntity> RoleEntityList { get; set; }

        public ObservableCollection<ActionAndRoleActionEntity> ActionAndRoleActionEntityList { get; set; }

        public Dictionary<int, ActionAndRoleActionEntity> ActionAndRoleAnctionEntityDictionary { get; set; }

        public ActionAndRoleActionEntity RootActionAndRoleActionEntity { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }
       
        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            RootActionAndRoleActionEntity.ChildList.Clear();
            ActionAndRoleAnctionEntityDictionary.Clear();
            RoleEntityList.Clear();
            ActionEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.action> loadOperationAction
                = systemManageDomainContext.Load<ProductManager.Web.Model.action>(systemManageDomainContext.GetActionQuery());
            loadOperationAction.Completed += LoadOperation_ActionCompleted;
        }

        void LoadOperation_ActionCompleted(object sender, EventArgs e)
        {
            ActionEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.action action in loadOperation.Entities)
            {
                ActionEntity actionEntity = new ActionEntity();
                actionEntity.Action = action;
                actionEntity.Update();
                ActionEntityList.Add(actionEntity);
            }
            /*LoadOperation<ProductManager.Web.Model.role> loadOperationRole
                = systemManageDomainContext.Load<ProductManager.Web.Model.role>(systemManageDomainContext.GetRoleQuery());
            loadOperationRole.Completed += loadOperation_RoleCompleted;*/
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment
                = systemManageDomainContext.Load<ProductManager.Web.Model.department>(systemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            RoleEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                RoleEntity roleEntity = new RoleEntity();
                roleEntity.RoleID = department.department_id;
                roleEntity.RoleName = department.department_name/* + "员工"*/;
                RoleEntityList.Add(roleEntity);
            }
            IsBusy = false;
        }

        private RoleEntity selectRoleEntity;
        public RoleEntity SelectRoleEntity
        {
            get
            {
                return selectRoleEntity;
            }
            set
            {
                if (selectRoleEntity != value && value != null)
                {
                    selectRoleEntity = value;
                    UpdateChanged("SelectRoleEntity");
                    UpdateRoleAndRoleAction();
                    UpdateRolAction();
                }
            }
        }

        private void UpdateRolAction()
        {
            LoadOperation<ProductManager.Web.Model.role_action> loadOperationRole
                           = systemManageDomainContext.Load<ProductManager.Web.Model.role_action>(systemManageDomainContext.GetRole_actionByRoleIDQuery(selectRoleEntity.RoleID));
            loadOperationRole.Completed += loadOperation_RoleActionCompleted;
        }

        void loadOperation_RoleActionCompleted(object sender, EventArgs e)
        {
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.role_action role_action in loadOperation.Entities)
            {
                RoleActionEntity roleActionEntity = new RoleActionEntity();
                roleActionEntity.RoleAction = role_action;
                roleActionEntity.Update();
                ActionAndRoleActionEntity actionAndRoleActionEntity;
                if (ActionAndRoleAnctionEntityDictionary.TryGetValue(roleActionEntity.ActionID, out actionAndRoleActionEntity))
                {
                    actionAndRoleActionEntity.RoleActionEntity = roleActionEntity;
                }
            }

            RootActionAndRoleActionEntity.ChildList.Clear();
            foreach (KeyValuePair<int, ActionAndRoleActionEntity> actionAndRoleActionEntityPair in ActionAndRoleAnctionEntityDictionary)
            {
                actionAndRoleActionEntityPair.Value.CurrentRoleEntity = SelectRoleEntity;
                int supperActionID = actionAndRoleActionEntityPair.Value.ActionEntity.SupperActionID;
                if (supperActionID == 0)
                {
                    RootActionAndRoleActionEntity.ChildList.Add(actionAndRoleActionEntityPair.Value);
                }
            }
            UpdateChanged("RootActionAndRoleActionEntity");
            IsBusy = false;
        }

        private void UpdateRoleAndRoleAction()
        {
            IsBusy = true;
            ActionAndRoleAnctionEntityDictionary.Clear();
            ActionAndRoleActionEntityList.Clear();
            foreach (ActionEntity actionEntity in ActionEntityList)
            {
                ActionAndRoleActionEntity actionAndRoleActionEntity = new ActionAndRoleActionEntity();
                actionAndRoleActionEntity.ActionEntity = actionEntity;
                actionAndRoleActionEntity.SystemManageDomainContext = systemManageDomainContext;
                ActionAndRoleAnctionEntityDictionary.Add(actionAndRoleActionEntity.ActionEntity.ActionID, actionAndRoleActionEntity);
            }

            foreach (KeyValuePair<int, ActionAndRoleActionEntity> actionAndRoleActionEntityPair in ActionAndRoleAnctionEntityDictionary)
            {
                int supperActionID = actionAndRoleActionEntityPair.Value.ActionEntity.SupperActionID;
                if (supperActionID != 0)
                {
                    ActionAndRoleActionEntity supperActionAndRoleActionEntity;
                    if (ActionAndRoleAnctionEntityDictionary.TryGetValue(supperActionID, out supperActionAndRoleActionEntity))
                    {
                        AddRoleAndRoleAction(supperActionAndRoleActionEntity, actionAndRoleActionEntityPair.Value);
                    }
                }
            }
        }

        private void AddRoleAndRoleAction(ActionAndRoleActionEntity supperActionAndRoleActionEntity, ActionAndRoleActionEntity actionAndRoleActionEntity)
        {
            if (supperActionAndRoleActionEntity.ChildList == null)
            {
                supperActionAndRoleActionEntity.ChildList = new ObservableCollection<ActionAndRoleActionEntity>();
            }
            supperActionAndRoleActionEntity.ChildList.Add(actionAndRoleActionEntity);
            actionAndRoleActionEntity.ParentActionAndRoleActionEntity = supperActionAndRoleActionEntity;
        }

        public ICommand OnSave { get; private set; }

        public RoleRightManagerViewModel()
        {
            ActionEntityList = new ObservableCollection<ActionEntity>();
            RoleEntityList = new ObservableCollection<RoleEntity>();
            ActionAndRoleActionEntityList = new ObservableCollection<ActionAndRoleActionEntity>();
            ActionAndRoleAnctionEntityDictionary = new Dictionary<int, ActionAndRoleActionEntity>();
            RootActionAndRoleActionEntity = new ActionAndRoleActionEntity();
            RootActionAndRoleActionEntity.ChildList = new ObservableCollection<ActionAndRoleActionEntity>();
            OnSave = new DelegateCommand(OnSaveCommand, CanSave);
            
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        private bool CanSave(object aObject)
        {
            return systemManageDomainContext.HasChanges;
        }

        private void OnSaveCommand()
        {
            IsBusy = true;
            SubmitOperation subOperation = systemManageDomainContext.SubmitChanges();
            subOperation.Completed += subOperation_Completed;
        }

        void subOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
                LoadData();
            }

            
        }

        public void ConfirmLeave()
        {
            if (systemManageDomainContext.HasChanges)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow("保存", "有改变，是否保存？");
                confirmWindow.Closed += new EventHandler(Confirm_Closed);
                confirmWindow.Show();
            }
        }

        void Confirm_Closed(object sender, EventArgs e)
        {
            ConfirmWindow confirmWindow = sender as ConfirmWindow;
            if (confirmWindow.DialogResult == true)
            {
                IsBusy = true;
                systemManageDomainContext.SubmitChanges();
            }
            else
            {
                systemManageDomainContext.RejectChanges();
            }
        }
    }
}
