using System;
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
    [Export("ProductPartTypeManager")]
    public class ProductPartTypeManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;

        public ObservableCollection<ProductPartTypeEntity> ProductPartTypeEntityList { get; set; }

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

        private ProductPartTypeEntity selectProductPartTypeEntity;
        public ProductPartTypeEntity SelectProductPartTypeEntity
        {
            get { return selectProductPartTypeEntity; }
            set
            {
                if (selectProductPartTypeEntity != value)
                {
                    selectProductPartTypeEntity = value;
                    UpdateChanged("SelectProductPartTypeEntity");
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAdd { get; private set; }
        public ICommand OnModify { get; private set; }
        public ICommand OnSave { get; private set; }
        public ICommand DoubleClick { get; private set; }

        public ProductPartTypeEntity AddProductPartTypeEntity { get; set; }

        public ProductPartTypeManagerViewModel()
        {
            ProductPartTypeEntityList = new ObservableCollection<ProductPartTypeEntity>();
            OnAdd = new DelegateCommand(OnAddCommand);
            OnModify = new DelegateCommand(OnModifyCommand, CanModifyCommand);
            OnSave = new DelegateCommand(OnSaveCommand, CanSaveCommand);
            DoubleClick = new DelegateCommand(DoubleClickCommand);
        }

        private void OnAddCommand()
        {
            AddProductPartTypeEntity = new ProductPartTypeEntity();
            AddProductPartTypeEntity.ProductPartType = new ProductManager.Web.Model.product_part_type();
            ProductPartTypeWindow productPartTypeWindow = new ProductPartTypeWindow(AddProductPartTypeEntity);
            productPartTypeWindow.Closed += productPartTypeWindow_Closed;
            productPartTypeWindow.Show();
        }

        void productPartTypeWindow_Closed(object sender, EventArgs e)
        {
            ProductPartTypeWindow productPartTypeWindow = sender as ProductPartTypeWindow;
            if (productPartTypeWindow.DialogResult == true)
            {
                ProductPartTypeEntityList.Add(AddProductPartTypeEntity);
                systemManageDomainContext.product_part_types.Add(AddProductPartTypeEntity.ProductPartType);
                OnSaveCommand();
            }
        }

        private void OnModifyCommand()
        {
            if (SelectProductPartTypeEntity != null)
            {
                ProductPartTypeWindow productPartTypeWindow = new ProductPartTypeWindow(SelectProductPartTypeEntity);
                productPartTypeWindow.Closed += productPartTypeModify_Closed;
                productPartTypeWindow.Show();
            }
        }

        private void productPartTypeModify_Closed(object sender, EventArgs e)
        {
            ProductPartTypeWindow productPartTypeWindow = sender as ProductPartTypeWindow;
            if (productPartTypeWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyCommand(object aObject)
        {
            if (SelectProductPartTypeEntity != null)
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
            if (SelectProductPartTypeEntity != null)
            {
                ProductPartTypeWindow productPartTypeWindow = new ProductPartTypeWindow(SelectProductPartTypeEntity);
                productPartTypeWindow.Closed += productPartTypeModify_Closed;
                productPartTypeWindow.Show();
            }
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            ProductPartTypeEntityList.Clear();

            selectProductPartTypeEntity = null;
            LoadOperation<ProductManager.Web.Model.product_part_type> loadOperationProductPartType =
                systemManageDomainContext.Load<ProductManager.Web.Model.product_part_type>(systemManageDomainContext.GetProduct_part_typeQuery());
            loadOperationProductPartType.Completed += loadOperationFileType_Completed;
        }

        void loadOperationFileType_Completed(object sender, EventArgs e)
        {
            ProductPartTypeEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product_part_type product_part_type in loadOperation.Entities)
            {
                ProductPartTypeEntity productPartTypeEntity = new ProductPartTypeEntity();
                productPartTypeEntity.ProductPartType = product_part_type;
                productPartTypeEntity.Update();
                ProductPartTypeEntityList.Add(productPartTypeEntity);
            }
            UpdateChanged("FileTypeEntityList");
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
