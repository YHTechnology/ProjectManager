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
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;
using System.Windows.Data;
using ProductManager.Controls;

namespace ProductManager.ViewModel.SystemManager
{
    public delegate void FinishLoaded();

    [Export("RightManager")]
    public class RightManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;// = new SystemManageDomainContext();

        public List<UserEntity> UserList { get; set; }

        public ObservableCollection<ActionEntity> ActionEntityList { get; set; }

        public ObservableCollection<ActionAndUserActionEntity> ActionAndUserActionEntityList { get; set; }

        public Dictionary<int, ActionAndUserActionEntity> ActionAndUserActionEntityDictionary { get; set; }

        public ObservableCollection<RoleActionEntity> RoleActionEntityList { get; set; }

        public ObservableCollection<UserActionEntity> UserActionEntityList { get; set; }

        public ActionAndUserActionEntity RootActionAndUserActionEntity { get; set; }

        private PagedCollectionView userDataView;
        public PagedCollectionView UserDataView
        {
            get
            {
                return userDataView;
            }
            set
            {
                if (userDataView != value)
                {
                    userDataView = value;
                    UpdateChanged("UserDataView");
                }
            }
        }

        private Dictionary<int, DepartmentEntity> DepartmentDictionary { get; set; }

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

        private FinishLoaded finishLoaded;

        public void SetCallBackLoaded(FinishLoaded afinishLoaded)
        {
            finishLoaded = afinishLoaded;
        }

        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.department>(systemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperation_Completed(object sender, EventArgs e)
        {
            UserList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                if (user.user_name == "admin")
                {
                    continue;
                }
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                DepartmentEntity departmentEntity;
                if (DepartmentDictionary.TryGetValue(userEntity.UserDepartmentID, out departmentEntity))
                {
                    userEntity.UserDepartment = departmentEntity.DepartmentName;
                    UserList.Add(userEntity);
                }
            }

            PagedCollectionView lPagedCollectionView = new PagedCollectionView(UserList);
            lPagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("UserDepartment"));
            UserDataView = lPagedCollectionView;
            UserDataView.Refresh();
            UpdateChanged("UserList");
            IsBusy = false;
            finishLoaded();
        }

        void UpdateAction()
        {
            IsBusy = true;
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
            UpdateRoleAndRoleAction();
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            //DepartmemtList.Clear();
            DepartmentDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                //DepartmemtList.Add(departmentEntity);
                DepartmentDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
            }
            UpdateChanged("DepartmemtList");
            IsBusy = true;
            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                systemManageDomainContext.Load<ProductManager.Web.Model.user>(systemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperation_Completed;
        }

        private UserEntity selectUserEntity = null;
        public UserEntity SelectUserEntity
        {
            get
            {
                return selectUserEntity;
            }
            set
            {
                if (selectUserEntity != value && value != null)
                {
                    selectUserEntity = value;
                    UpdateChanged("SelectUserEntity");
                    UpdateAction();
                }
            }
        }

        private void UpdateRoleAction()
        {
            LoadOperation<ProductManager.Web.Model.role_action> loadOperationRole
                           = systemManageDomainContext.Load<ProductManager.Web.Model.role_action>(systemManageDomainContext.GetRole_actionByRoleIDQuery(selectUserEntity.UserDepartmentID));
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
                ActionAndUserActionEntity actionAndUserActionEntity;
                if (ActionAndUserActionEntityDictionary.TryGetValue(roleActionEntity.ActionID, out actionAndUserActionEntity))
                {
                    actionAndUserActionEntity.RoleActionEntity = roleActionEntity;
                }
            }
            UpdateUserAction();
        }

        private void UpdateUserAction()
        {
            LoadOperation<ProductManager.Web.Model.user_action> loadOperationRole
                           = systemManageDomainContext.Load<ProductManager.Web.Model.user_action>(systemManageDomainContext.GetUser_actionByUserIDQuery(selectUserEntity.UserID));
            loadOperationRole.Completed += loadOperation_UserActionCompleted;
        }

        void loadOperation_UserActionCompleted(object sender, EventArgs e)
        {
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user_action user_action in loadOperation.Entities)
            {
                UserActionEntity userActionEntity = new UserActionEntity();
                userActionEntity.UserAction = user_action;
                userActionEntity.Update();
                ActionAndUserActionEntity actionAndUserActionEntity;
                if (ActionAndUserActionEntityDictionary.TryGetValue(userActionEntity.ActionID, out actionAndUserActionEntity))
                {
                    actionAndUserActionEntity.UserActionEntity = userActionEntity;
                }
            }

            RootActionAndUserActionEntity.ChildList.Clear();
            foreach (KeyValuePair<int, ActionAndUserActionEntity> actionAndUserActionEntityPair in ActionAndUserActionEntityDictionary)
            {
                actionAndUserActionEntityPair.Value.CurrentSelectUserEntity = SelectUserEntity;
                int supperActionID = actionAndUserActionEntityPair.Value.ActionEntity.SupperActionID;
                if (supperActionID == 0)
                {
                    RootActionAndUserActionEntity.ChildList.Add(actionAndUserActionEntityPair.Value);
                }
            }
            UpdateChanged("RootActionAndUserActionEntity");

            IsBusy = false;
        }

        private void UpdateRoleAndRoleAction()
        {
            ActionAndUserActionEntityDictionary.Clear();
            ActionAndUserActionEntityList.Clear();
            foreach (ActionEntity actionEntity in ActionEntityList)
            {
                ActionAndUserActionEntity actionAndUserActionEntity = new ActionAndUserActionEntity();
                actionAndUserActionEntity.ActionEntity = actionEntity;
                actionAndUserActionEntity.SystemManageDomainContext = systemManageDomainContext;
                ActionAndUserActionEntityDictionary.Add(actionAndUserActionEntity.ActionEntity.ActionID, actionAndUserActionEntity);
            }

            foreach (KeyValuePair<int, ActionAndUserActionEntity> actionAndUserActionEntityPair in ActionAndUserActionEntityDictionary)
            {
                int supperActionID = actionAndUserActionEntityPair.Value.ActionEntity.SupperActionID;
                if (supperActionID != 0)
                {
                    ActionAndUserActionEntity supperActionAndUserActionEntity;
                    if (ActionAndUserActionEntityDictionary.TryGetValue(supperActionID, out supperActionAndUserActionEntity))
                    {
                        AddRoleAndRoleAction(supperActionAndUserActionEntity, actionAndUserActionEntityPair.Value);
                    }
                }
            }
            UpdateRoleAction();
        }

        private void AddRoleAndRoleAction(ActionAndUserActionEntity supperActionAndUserActionEntity, ActionAndUserActionEntity actionAndUserActionEntity)
        {
            if (supperActionAndUserActionEntity.ChildList == null)
            {
                supperActionAndUserActionEntity.ChildList = new ObservableCollection<ActionAndUserActionEntity>();
            }
            supperActionAndUserActionEntity.ChildList.Add(actionAndUserActionEntity);
            actionAndUserActionEntity.ParentActionAdnUserActionEntity = supperActionAndUserActionEntity;
        }

        public ICommand OnSave { get; private set; }

        public RightManagerViewModel()
        {
            ActionEntityList = new ObservableCollection<ActionEntity>();
            ActionAndUserActionEntityList = new ObservableCollection<ActionAndUserActionEntity>();
            ActionAndUserActionEntityDictionary = new Dictionary<int, ActionAndUserActionEntity>();
            UserList = new List<UserEntity>();
            DepartmentDictionary = new Dictionary<int, DepartmentEntity>();
            RootActionAndUserActionEntity = new ActionAndUserActionEntity();
            RootActionAndUserActionEntity.ChildList = new ObservableCollection<ActionAndUserActionEntity>();
            OnSave = new DelegateCommand(OnSaveCommand, CanSave);
            
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
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
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
