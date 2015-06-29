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
using ProductManager.Views.SystemManager;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.SystemManager
{
    [Export("ProductTypeManager")]
    public class ProductTypeManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext = new SystemManageDomainContext();

        public ObservableCollection<ProductTypeEntity> ProductTypeEntityList { get; set; }

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

        private ProductTypeEntity selectProductTypeEntity;
        public ProductTypeEntity SelectProductTypeEntity
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
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ProductTypeManagerViewModel()
        {
            ProductTypeEntityList = new ObservableCollection<ProductTypeEntity>();
            //ProductTypeEntityDictionary = new Dictionary<String, ProductTypeEntity>();

            OnAdd = new DelegateCommand(OnAddCommand);
            OnModify = new DelegateCommand(OnModifyCommand, CanModifyCommand);
            OnSave = new DelegateCommand(OnSaveCommand, CanSaveCommand);
            DoubleClick = new DelegateCommand(DoubleClickCommand);

            //systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        public ICommand OnAdd { get; private set; }
        public ICommand OnModify { get; private set; }
        public ICommand OnSave { get; private set; }
        public ICommand DoubleClick { get; private set; }

        public ProductTypeEntity AddProductTypeEntity { get; set; }

        private void OnAddCommand()
        {
            AddProductTypeEntity = new ProductTypeEntity();
            AddProductTypeEntity.ProductType = new ProductManager.Web.Model.product_type();
            ProductTypeWindow productTypeWindow = new ProductTypeWindow(AddProductTypeEntity);
            productTypeWindow.Closed += productTypeWindow_Closed;
            productTypeWindow.Show();
        }

        void productTypeWindow_Closed(object sender, EventArgs e)
        {
            ProductTypeWindow productTypeWindow = sender as ProductTypeWindow;
            if (productTypeWindow.DialogResult == true)
            {
                ProductTypeEntityList.Add(AddProductTypeEntity);
                systemManageDomainContext.product_types.Add(AddProductTypeEntity.ProductType);
                OnSaveCommand();
            }
        }

        private void OnModifyCommand()
        {
            if (SelectProductTypeEntity != null)
            {
                ProductTypeWindow productTypeWindow = new ProductTypeWindow(SelectProductTypeEntity);
                productTypeWindow.Closed += productTypeModify_Close;
                productTypeWindow.Show();
            }
        }

        void productTypeModify_Close(object sender, EventArgs e)
        {
            ProductTypeWindow productTypeWindow = sender as ProductTypeWindow;
            if (productTypeWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyCommand(object aObject)
        {
            if (SelectProductTypeEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnSaveCommand()
        {
            IsBusy = true;
            SubmitOperation submitOperation = systemManageDomainContext.SubmitChanges();
            submitOperation.Completed += submitOperation_Completed;
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
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        private bool CanSaveCommand(object aObject)
        {
            if (systemManageDomainContext.HasChanges)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoubleClickCommand()
        {
            if (SelectProductTypeEntity != null)
            {
                ProductTypeWindow productTypeWindow = new ProductTypeWindow(SelectProductTypeEntity);
                productTypeWindow.Closed += productTypeModify_Close;
                productTypeWindow.Show();
            }
        }


        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            ProductTypeEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.product_type> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.product_type>(systemManageDomainContext.GetProduct_typeQuery());
            loadOperationDepartment.Completed += loadOperationProductType_Completed;
        }

        private void loadOperationProductType_Completed(object sender, EventArgs e)
        {
            ProductTypeEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product_type product_type in loadOperation.Entities)
            {
                ProductTypeEntity productTypeEntity = new ProductTypeEntity();
                productTypeEntity.ProductType = product_type;
                productTypeEntity.Update();
                ProductTypeEntityList.Add(productTypeEntity);
            }
            UpdateChanged("ProductTypeEntityList");
            IsBusy = false;
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
