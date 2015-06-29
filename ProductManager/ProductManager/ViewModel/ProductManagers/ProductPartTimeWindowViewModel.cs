using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Microsoft.Windows.Data.DomainServices;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    public enum ProductPartTimeWindowState : uint
    {
        NO = 0,
        Add = 1,
        Modify = 2
    }

    public class ProductPartTimeWindowViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext;// = new ProductDomainContext();

        private SystemManageDomainContext SystemManageDomainContext;//; = new SystemManageDomainContext();

        private ChildWindow childWindow;

        private ProductEntity ProductEntity { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.product_part_time> productPartTimeView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.product_part_time> productPartTimeLoader;
        private EntityList<ProductManager.Web.Model.product_part_time> productPartTimeSource;

        public ObservableCollection<ProductPartTimeEntity> ProductPartTimeEntityList { get; set; }

        public ObservableCollection<ProductPartTypeEntity> ProductPartTypeEntityList { get; set; }
        public Dictionary<int, ProductPartTypeEntity> ProductPartTypeDictionary { get; set; }

        public ICollectionView CollectionProductPartTimeView
        {
            get
            {
                return this.productPartTimeView;
            }
        }

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

        private ProductPartTimeEntity selectProductPartTimeEntity;
        public ProductPartTimeEntity SelectProductPartTimeEntity
        {
            get
            {
                return selectProductPartTimeEntity;
            }
            set
            {
                if (selectProductPartTimeEntity != value)
                {
                    selectProductPartTimeEntity = value;
                    UpdateChanged("SelectProductPartTimeEntity");
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private ProductPartTypeEntity selectProductTypeEntity;
        public ProductPartTypeEntity SelectProductTypeEntity
        {
            get
            {
                return selectProductTypeEntity;
            }
            set
            {
                if (selectProductTypeEntity != value)
                {
                    selectProductTypeEntity = value;
                    UpdateChanged("SelectProductTypeEntity");
                }
            }
        }

        private ProductPartTimeWindowState productPartTimeWindowState { get; set; }

        private bool isNotAddorModify;
        public bool IsNotAddorModify
        {
            get
            {
                return isNotAddorModify;
            }
            set
            {
                if (isNotAddorModify != value)
                {
                    isNotAddorModify = value;
                    UpdateChanged("IsAddorModify");
                    UpdateChanged("IsNotAddorModify");
                    (OnQuit as DelegateCommand).RaiseCanExecuteChanged();
                    (OnAdd as DelegateCommand).RaiseCanExecuteChanged();
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                    (OnSave as DelegateCommand).RaiseCanExecuteChanged();
                    (OnCancel as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsAddorModify
        {
            get
            {
                return !isNotAddorModify;
            }
        }

        public String Title { get; set; }

        public ICommand OnQuit { get; private set; }
        public ICommand OnAdd { get; private set; }
        public ICommand OnModify { get; private set; }
        public ICommand OnSave { get; private set; }
        public ICommand OnCancel { get; private set; }

        private void OnQuitCommand()
        {
            childWindow.DialogResult = true;
            ProductEntity.Update();
        }

        private bool CanQuitCommand(object aObject)
        {
            return IsNotAddorModify;
        }

        private void OnAddCommand()
        {
            IsNotAddorModify = false;
            SelectProductPartTimeEntity = new ProductPartTimeEntity();
            SelectProductPartTimeEntity.ProductPartTime = new ProductManager.Web.Model.product_part_time();
            SelectProductPartTimeEntity.ProductPartTime.start_time = DateTime.Now;
            SelectProductPartTimeEntity.StartTime = DateTime.Now;
            SelectProductPartTimeEntity.Update();
            SelectProductPartTimeEntity.ProductPartID = 1;
            SelectProductPartTimeEntity.ProductID = ProductEntity.ProductID;
            productPartTimeWindowState = ProductPartTimeWindowState.Add;
        }

        private bool CanAddCommand(object aObject)
        {
            return IsNotAddorModify;
        }

        private void OnModifyCommand()
        {
            IsNotAddorModify = false;
            SelectProductPartTimeEntity.Update();
            productPartTimeWindowState = ProductPartTimeWindowState.Modify;
        }

        private bool CanModifyCommand(object aObject)
        {
            if (SelectProductPartTimeEntity != null && productPartTimeWindowState == ProductPartTimeWindowState.NO)
            {
                return IsNotAddorModify;
            }
            else
            {
                return false;
            }
        }

        private void OnSaveCommand()
        {
            IsBusy = true;
            if (productPartTimeWindowState == ProductPartTimeWindowState.Add)
            {
                SelectProductPartTimeEntity.DUpdate();

                if (SelectProductTypeEntity != null)
                {
                    SelectProductPartTimeEntity.ProductPartName = SelectProductTypeEntity.ProductPartTypeName;
                }
                else
                {
                    ProductPartTypeEntity productPartTypeEntity;
                    if (ProductPartTypeDictionary.TryGetValue(SelectProductPartTimeEntity.ProductPartID, out productPartTypeEntity))
                    {
                        SelectProductPartTimeEntity.ProductPartName = productPartTypeEntity.ProductPartTypeName;
                    }
                }

                SelectProductPartTimeEntity.RaisALL();
                ProductPartTimeEntityList.Add(SelectProductPartTimeEntity);
                ProductDomainContext.product_part_times.Add(SelectProductPartTimeEntity.ProductPartTime);
                productPartTimeWindowState = ProductPartTimeWindowState.NO;
            }
            if (productPartTimeWindowState == ProductPartTimeWindowState.Modify)
            {
                SelectProductPartTimeEntity.DUpdate();
                if (SelectProductTypeEntity != null)
                {
                    SelectProductPartTimeEntity.ProductPartName = SelectProductTypeEntity.ProductPartTypeName;
                }
                else
                {
                    ProductPartTypeEntity productPartTypeEntity;
                    if (ProductPartTypeDictionary.TryGetValue(SelectProductPartTimeEntity.ProductPartID, out productPartTypeEntity))
                    {
                        SelectProductPartTimeEntity.ProductPartName = productPartTypeEntity.ProductPartTypeName;
                    }
                }
                SelectProductPartTimeEntity.RaisALL();
                productPartTimeWindowState = ProductPartTimeWindowState.NO;
            }
            SubmitOperation submitOperation = ProductDomainContext.SubmitChanges();
            submitOperation.Completed += SubOperationCommpleted;
            IsNotAddorModify = true;
        }

        private void SubOperationCommpleted(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        private bool CanSaveCommand(object aObject)
        {
            return !IsNotAddorModify;
        }

        private void OnCancelCommand()
        {
            if (productPartTimeWindowState == ProductPartTimeWindowState.Modify)
            {
                SelectProductPartTimeEntity.Update();
                SelectProductPartTimeEntity.RaisALL();
                productPartTimeWindowState = ProductPartTimeWindowState.NO;
            }
            IsNotAddorModify = true;
        }

        private bool CanCancelCommand(object aObject)
        {
            return !IsNotAddorModify;
        }

        public ProductPartTimeWindowViewModel(ChildWindow aChildWindow, ProductEntity aProductEntity)
        {
            childWindow = aChildWindow;
            ProductEntity = aProductEntity;
            productPartTimeWindowState = ProductPartTimeWindowState.NO;
            ProductPartTimeEntityList = new ObservableCollection<ProductPartTimeEntity>();
            ProductPartTypeEntityList = new ObservableCollection<ProductPartTypeEntity>();
            ProductPartTypeDictionary = new Dictionary<int, ProductPartTypeEntity>();

            Title = "产品 " + ProductEntity.ProductID + " : " + ProductEntity.ProductName + " 阶段";

            OnQuit = new DelegateCommand(OnQuitCommand, CanQuitCommand);
            OnAdd = new DelegateCommand(OnAddCommand, CanAddCommand);
            OnModify = new DelegateCommand(OnModifyCommand, CanModifyCommand);
            OnSave = new DelegateCommand(OnSaveCommand, CanSaveCommand);
            OnCancel = new DelegateCommand(OnCancelCommand, CanCancelCommand);

            IsNotAddorModify = true;
            LoadData();
        }

        private void LoadData()
        {
            IsBusy = true;
            ProductDomainContext = new ProductDomainContext();
            SystemManageDomainContext = new SystemManageDomainContext();
            LoadOperation<ProductManager.Web.Model.product_part_type> loadOperationProductPartType =
                SystemManageDomainContext.Load<ProductManager.Web.Model.product_part_type>(SystemManageDomainContext.GetProduct_part_typeQuery());
            loadOperationProductPartType.Completed += loadOperationProductPartType_Completed;
        }

        private void loadOperationProductPartType_Completed(object sender, EventArgs e)
        {
            ProductPartTypeEntityList.Clear();
            ProductPartTypeDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product_part_type product_part_type in loadOperation.Entities)
            {
                ProductPartTypeEntity productPartTypeEntity = new ProductPartTypeEntity();
                productPartTypeEntity.ProductPartType = product_part_type;
                productPartTypeEntity.Update();
                ProductPartTypeEntityList.Add(productPartTypeEntity);
                ProductPartTypeDictionary.Add(productPartTypeEntity.ProductPartTypeID, productPartTypeEntity);
            }

            this.productPartTimeSource = new EntityList<ProductManager.Web.Model.product_part_time>(this.ProductDomainContext.product_part_times);
            this.productPartTimeLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.product_part_time>(
                this.LoadProductPartTimeEntities,
                this.LoadOperationProductPartTimeCompleted);
            this.productPartTimeView = new DomainCollectionView<ProductManager.Web.Model.product_part_time>(this.productPartTimeLoader, this.productPartTimeSource);
            using (this.CollectionProductPartTimeView.DeferRefresh())
            {
                this.productPartTimeView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.product_part_time> LoadProductPartTimeEntities()
        {
            if (ProductEntity != null)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.product_part_time> lQuery = this.ProductDomainContext.GetProduct_part_timeQuery();
                lQuery = lQuery.Where(e => e.product_id == ProductEntity.ProductID);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productPartTimeView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.product_part_time> lQuery = this.ProductDomainContext.GetProduct_part_timeQuery();
                lQuery = lQuery.Where(e => e.product_id == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productPartTimeView));
            }
        }

        private void LoadOperationProductPartTimeCompleted(LoadOperation<ProductManager.Web.Model.product_part_time> aLoadOperation)
        {
            ProductPartTimeEntityList.Clear();
            foreach (ProductManager.Web.Model.product_part_time product_part_time in aLoadOperation.Entities)
            {
                ProductPartTimeEntity productPartTimeEntity = new ProductPartTimeEntity();
                productPartTimeEntity.ProductPartTime = product_part_time;
                productPartTimeEntity.Update();
                ProductPartTypeEntity productPartTypeEntity;
                if (ProductPartTypeDictionary.TryGetValue(productPartTimeEntity.ProductPartID, out productPartTypeEntity))
                {
                    productPartTimeEntity.ProductPartName = productPartTypeEntity.ProductPartTypeName;
                }
                ProductPartTimeEntityList.Add(productPartTimeEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.productPartTimeView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProductPartTimeEntityList");
            this.IsBusy = false;
        }
    }
}
