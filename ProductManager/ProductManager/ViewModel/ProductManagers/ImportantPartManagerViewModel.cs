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
using Microsoft.Windows.Data.DomainServices;
using ProductManager.Views.ProductManagers;
using ProductManager.Controls;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("ImportantPartManager")]
    public class ImportantPartManagerViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext productDomainContext;
        private SystemManageDomainContext systemManageDomainContext;

        private DomainCollectionView<ProductManager.Web.Model.important_part> importantPartView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.important_part> importantPartLoader;
        private EntityList<ProductManager.Web.Model.important_part> importantPartSource;

        public ObservableCollection<ImportantPartEntity> ImportantPartEntityList { get; set; }

        public Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
        public ObservableCollection<UserEntity> UserEntityList { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public ImportantPartEntity AddImportantPartEntity { get; set; }

        public ICommand OnAddImportantPart { get; private set; }
        public ICommand OnArivePart { get; private set; }
        public ICommand OnOutPart { get; private set; }
        public ICommand OnModifyImportantPart { get; private set; }
        public ICommand OnViewImportantPart { get; private set; }

        private ImportantPartEntity selectImportantPartEntity;
        public ImportantPartEntity SelectImportantPartEntity
        {
            get
            {
                return selectImportantPartEntity;
            }
            set
            {
                if (selectImportantPartEntity != value)
                {
                    selectImportantPartEntity = value;
                    UpdateChanged("SelectImportantPartEntity");
                    (OnArivePart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnOutPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnModifyImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ImportantPartManagerViewModel()
        {
            ImportantPartEntityList = new ObservableCollection<ImportantPartEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            UserEntityList = new ObservableCollection<UserEntity>();

            OnAddImportantPart = new DelegateCommand(OnAddImportantPartCommand, CanAddImportantPartCommand);
            OnArivePart = new DelegateCommand(OnArivePartCommand, CanArivePartCommand);
            OnOutPart = new DelegateCommand(OnOutPartCommand, CanOnOutPartCommand);
            OnModifyImportantPart = new DelegateCommand(OnModifyImportantPartCommand, CanModifyImportantPartCommand);
            OnViewImportantPart = new DelegateCommand(OnViewImportantPartCommand, CanViewImportantPartCommand);
        }

        public void LoadData()
        {
            IsBusy = true;
            productDomainContext = new ProductDomainContext();
            systemManageDomainContext = new SystemManageDomainContext();

            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                systemManageDomainContext.Load<ProductManager.Web.Model.user>(systemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;

        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            UserEntityList.Clear();
            
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                UserEntityDictionary.Add(userEntity.UserID, userEntity);
                UserEntityList.Add(userEntity);
            }

            this.importantPartSource = new EntityList<ProductManager.Web.Model.important_part>(this.productDomainContext.important_parts);
            this.importantPartLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.important_part>(
                this.LoadImportantPartEntities,
                this.LoadImportantPartCompleted);
            this.importantPartView = new DomainCollectionView<ProductManager.Web.Model.important_part>(this.importantPartLoader, this.importantPartSource);
            using (this.importantPartView.DeferRefresh())
            {
                this.importantPartView.MoveToFirstPage();
            }
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.important_part> LoadImportantPartEntities()
        {
            IsBusy = true;
            EntityQuery<ProductManager.Web.Model.important_part> lQuery = this.productDomainContext.GetImportant_partQuery();
            return this.productDomainContext.Load(lQuery.SortAndPageBy(this.importantPartView));
        }

        private void LoadImportantPartCompleted(LoadOperation<ProductManager.Web.Model.important_part> aLoadOperation)
        {
            ImportantPartEntityList.Clear();
            foreach (ProductManager.Web.Model.important_part important_part in aLoadOperation.Entities)
            {
                ImportantPartEntity importantPartEntity = new ImportantPartEntity();
                importantPartEntity.ImportantPart = important_part;
                importantPartEntity.UserEntityDictionary = UserEntityDictionary;
                importantPartEntity.Update();
                ImportantPartEntityList.Add(importantPartEntity);
            }
            IsBusy = false;
        }

        private void OnAddImportantPartCommand()
        {
            AddImportantPartEntity = new ImportantPartEntity();
            AddImportantPartEntity.UserEntityDictionary = UserEntityDictionary;
            AddImportantPartEntity.ImportantPart = new ProductManager.Web.Model.important_part();
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.Add, AddImportantPartEntity);
            importantPartWindow.Closed += importantPartWindow_Closed;
            importantPartWindow.Show();
        }

        void importantPartWindow_Closed(object sender, EventArgs e)
        {
            ImportantPartWindow importantPartWindow = sender as ImportantPartWindow;
            if (importantPartWindow.DialogResult == true)
            {
                ImportantPartEntityList.Add(AddImportantPartEntity);
                productDomainContext.important_parts.Add(AddImportantPartEntity.ImportantPart);
                IsBusy = true;
                SubmitOperation submitOperation = productDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        void submitOperation_Completed(object sender, EventArgs e)
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
            IsBusy = false;
        }

        private bool CanAddImportantPartCommand(object aObject)
        {
            App app = Application.Current as App;
            return app.UserInfo.GetUerRight(2050100);
        }

        private void OnArivePartCommand()
        {
            SelectImportantPartEntity.AriveTime = DateTime.Now;
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.ARIVE, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        void importantPartWindowModify_Closed(object sender, EventArgs e)
        {
            ImportantPartWindow importantPartWindow = sender as ImportantPartWindow;
            if (importantPartWindow.DialogResult == true)
            {
                IsBusy = true;
                SubmitOperation submitOperation = productDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        private bool CanArivePartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null /*&& !SelectImportantPartEntity.IsArive*/)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050200);
            }
            return false;
        }

        private void OnOutPartCommand()
        {
            //SelectImportantPartEntity.PartOutTime = DateTime.Now;
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.OUT, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        private bool CanOnOutPartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null /*&& SelectImportantPartEntity.IsArive && !SelectImportantPartEntity.IsOut*/)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050300);
            }
            return false;
        }

        private void OnModifyImportantPartCommand()
        {
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.MODIFY, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        private bool CanModifyImportantPartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050400);
            }
            return false;
        }

        private void OnViewImportantPartCommand()
        {
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.VIEW, SelectImportantPartEntity);
            importantPartWindow.Show();
        }

        private bool CanViewImportantPartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
