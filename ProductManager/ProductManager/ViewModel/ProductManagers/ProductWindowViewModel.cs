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
using ProductManager.ViewData.Entity;

namespace ProductManager.ViewModel.ProductManagers
{
    public enum ProductWindowType : uint
    {
        ADD = 0,
        Modify = 1,
        SetOutPutNumber = 2,
        View = 3
    }

    public class ProductWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProductEntity ProductEntity { get; set; }

        public ObservableCollection<ProductTypeEntity> ProductTypeEntityList { get; set; }

        //public ObservableCollection<UserEntity> UserEntityList { get; set; }

        public String Title { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        private ProductWindowType productWindowType { get; set; }

        public bool IsAdd { get; set; }

        public bool IsView { get; set; }
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
                }
            }
        }

        public bool CanSetOutputNumber
        {
            get
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010600);
            } 
        }

        public bool IsModify
        {
            get
            {
                if (productWindowType == ProductWindowType.ADD
                    || productWindowType == ProductWindowType.View
                    || productWindowType == ProductWindowType.Modify)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ProductWindowViewModel(ChildWindow aChildWindow, ObservableCollection<ProductTypeEntity> aProductTypeEntityList, /*ObservableCollection<UserEntity> aUserEntityList,*/ ProductWindowType aProductWindowType, ProductEntity aProductEntity)
        {
            childWindow = aChildWindow;
            ProductEntity = aProductEntity;
            productWindowType = aProductWindowType;
            //UserEntityList = aUserEntityList;
            ProductTypeEntityList = aProductTypeEntityList;

            //App app = Application.Current as App;
            //CanSetOutputNumber = app.UserInfo.GetUerRight(2010600);

            if (productWindowType == ProductWindowType.ADD)
            {
                IsAdd = true;
            } 
            else
            {
                IsAdd = false;
            }

            if (productWindowType == ProductWindowType.View)
            {
                IsView = true;
            }
            else
            {
                IsView = false;
            }

            if (!String.IsNullOrWhiteSpace(ProductEntity.ManufactureNumber))
            {
                Title = "生产令号：" + ProductEntity.ManufactureNumber;
            }
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.ProductEntity.Validate())
            {
                this.ProductEntity.DUpdate();
                if (selectProductTypeEntity != null)
                {
                    this.ProductEntity.ProductTypeString = selectProductTypeEntity.ProductTypeName;
                }
                else
                {
                    foreach (ProductTypeEntity productTypeEntity in ProductTypeEntityList)
                    {
                        if (this.ProductEntity.ProductTypeID == productTypeEntity.ProductTypeID)
                        {
                            this.ProductEntity.ProductTypeString = productTypeEntity.ProductTypeName;
                        }
                    }
                }

                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.ProductEntity.Update();
            this.ProductEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }

    }
}
