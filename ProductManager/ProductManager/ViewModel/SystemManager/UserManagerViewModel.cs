using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using ProductManager.Web.Service;
using ProductManager.ViewData.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProductManager.Views.SystemManager;
using ProductManager.Controls;
using System.ComponentModel;
using Microsoft.Windows.Data.DomainServices;


namespace ProductManager.ViewModel.SystemManager
{
    [Export("UserManager")]
    public class UserManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;// = new SystemManageDomainContext();

        private OperationDomainContext operationDomainComtext = new OperationDomainContext();

        public ObservableCollection<UserEntity> UserList { get; set; }

        public ObservableCollection<DepartmentEntity> DepartmemtList { get; set; }

        private Dictionary<int, DepartmentEntity> DepartmentDictionary { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.user> userView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.user> userLoader;
        private EntityList<ProductManager.Web.Model.user> userSource;

        private Dictionary<String, UserEntity> UserEntityDictionary { get; set; }

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

        public UserEntity AddUserEntity { get; set; }

        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            //systemManageDomainContext.RejectChanges();
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.department>(systemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperation_Completed(LoadOperation<ProductManager.Web.Model.user> sender)
        {
            UserList.Clear();
            //LoadOperation loadOperation = sender as LoadOperation;
            this.userSource.Source = sender.Entities;
            foreach (ProductManager.Web.Model.user user in sender.Entities)
            {
                if (user.user_name == "admin")
                {
                    continue;
                }
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.UserNameDictionary = UserEntityDictionary;
                userEntity.Update();
                UserList.Add(userEntity);
                DepartmentEntity departmentEntity;
                if (DepartmentDictionary.TryGetValue(userEntity.UserDepartmentID, out departmentEntity))
                {
                    userEntity.UserDepartment = departmentEntity.DepartmentName;
                }
                
            }

            if (sender.TotalEntityCount != -1)
            {
                this.userView.SetTotalItemCount(sender.TotalEntityCount);
            }

            UpdateChanged("UserList");
            UpdateChanged("CollectionView");
            IsBusy = false;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            DepartmemtList.Clear();
            DepartmentDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmemtList.Add(departmentEntity);
                DepartmentDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
            }
            DepartmentEntity departmentEntityZero = new DepartmentEntity();
            departmentEntityZero.DepartmentID = 0;
            departmentEntityZero.DepartmentName = "请选择部门";
            DepartmemtList.Add(departmentEntityZero);
            UpdateChanged("DepartmemtList");

            LoadOperation<ProductManager.Web.Model.user> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.user>(systemManageDomainContext.GetUserQuery());
            loadOperationDepartment.Completed += loadOperationUser_Completed;

        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                UserEntityDictionary.Add(user.user_name, null);
            }
            
            this.userSource = new EntityList<ProductManager.Web.Model.user>(this.systemManageDomainContext.users);
            this.userLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.user>(
                this.LoadUserEntities,
                this.loadOperation_Completed);
            this.userView = new DomainCollectionView<ProductManager.Web.Model.user>(this.userLoader, this.userSource);

            using (this.CollectionView.DeferRefresh())
            {
                this.userView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.user> LoadUserEntities()
        {
            this.IsBusy = true;
            EntityQuery<ProductManager.Web.Model.user> lQuery = this.systemManageDomainContext.GetUserQuery();
            if (!String.IsNullOrWhiteSpace(FilterString))
            {
                String lFilterString = FilterString.Trim();
                lQuery = lQuery.Where(e => e.user_cname.Contains(lFilterString));
            }
            return this.systemManageDomainContext.Load(lQuery.SortAndPageBy(this.userView));
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
                if (selectUserEntity != value)
                {
                    selectUserEntity = value;
                    UpdateChanged("SelectUserEntity");
                    (OnModifyUser as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICollectionView CollectionView
        {
            get
            {
                return this.userView;
            }
        }

        public ICommand OnModifyUser { get; private set; }

        public ICommand OnSave { get; private set; }

        public ICommand OnAddUser { get; private set; }

        public ICommand DoubleClick { get; private set; }

        public UserManagerViewModel() 
        {
            UserList = new ObservableCollection<UserEntity>();
            UserEntityDictionary = new Dictionary<String, UserEntity>();
            DepartmemtList = new ObservableCollection<DepartmentEntity>();
            DepartmentDictionary = new Dictionary<int, DepartmentEntity>();
            OnModifyUser = new DelegateCommand(ModifyUser, CanModifyUser);
            OnSave = new DelegateCommand(OnSaveCommand, CanSave);
            OnAddUser = new DelegateCommand(OnAddUserCommand);
            DoubleClick = new DelegateCommand(DoubleClickCommand);
            OnRefash = new DelegateCommand(OnRefashCommand);
        }

        void DoubleClickCommand()
        {
            UserEntityWindow userEntityWindow = new UserEntityWindow(UserEntityViewType.Modify, SelectUserEntity, DepartmemtList);
            userEntityWindow.Closed += userEntityWindow_Closed;
            userEntityWindow.Show();
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        private bool CanSave(object aObject)
        {
            return systemManageDomainContext.HasChanges;
        }

        private void ModifyUser()
        {
            UserEntityWindow userEntityWindow = new UserEntityWindow(UserEntityViewType.Modify, SelectUserEntity, DepartmemtList);
            userEntityWindow.Closed += userEntityWindow_Closed;
            userEntityWindow.Show();
        }

        void userEntityWindow_Closed(object sender, EventArgs e)
        {
            SelectUserEntity.RaisALL();
            UserEntityWindow lUserEntityWindow = sender as UserEntityWindow;
            if (lUserEntityWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyUser(object aObject)
        {
            return SelectUserEntity != null;
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
                LoadData();
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        private void OnAddUserCommand()
        {
            //InvokeOperation<int> loadMaxUserID = operationDomainComtext.GetMaxUserID();
            //loadMaxUserID.Completed += loadMaxUserID_Completed;
            AddUserEntity = new UserEntity();
            ProductManager.Web.Model.user user = new ProductManager.Web.Model.user();
            AddUserEntity.User = user;
            AddUserEntity.UserNameDictionary = UserEntityDictionary;
            user.user_password = Cryptography.MD5CryptoServiceProvider.GetMd5String("123456");
            AddUserEntity.Update();
            UserEntityWindow userEntityWindow = new UserEntityWindow(UserEntityViewType.ADD, AddUserEntity, DepartmemtList);
            userEntityWindow.Closed += new EventHandler(AddUser_Closed);

            userEntityWindow.Show();
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
        
        void AddUser_Closed(object sender, EventArgs e)
        {
            UserEntityWindow userEntityWindow = sender as UserEntityWindow;
            if (userEntityWindow.DialogResult == true)
            {
                UserList.Add(AddUserEntity);
                UserEntityDictionary.Add(AddUserEntity.UserName, null);
                systemManageDomainContext.users.Add(AddUserEntity.User);
                OnSaveCommand();
            }
        }

        public ICommand OnRefash { get; private set; }

        private void OnRefashCommand()
        {
            using (this.CollectionView.DeferRefresh())
            {
                this.userView.MoveToFirstPage();
            }
        }

        private String filterString;
        public String FilterString
        {
            get
            {
                return filterString;
            }
            set
            {
                if (filterString != value)
                {
                    filterString = value;
                    UpdateChanged("FilterString");
                }
            }
        }
    }
}

