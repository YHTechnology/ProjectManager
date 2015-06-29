using System;
using System.Collections.Generic;
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
    public class ChangeManufactureNumberWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProductEntity ProductEntity { get; set; }

        public ObservableCollection<String> ManufactureNumberList { get; set; }

        public String Title { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public ChangeManufactureNumberWindowViewModel(ChildWindow aChildWindow, ProductEntity aProductEntity)
        {
            childWindow = aChildWindow;
            ProductEntity = aProductEntity;
            Title = "修改产品 " + ProductEntity.ProductName + " 生产令号：" + ProductEntity.ManufactureNumber;

            ManufactureNumberList = new ObservableCollection<String>();
            foreach (KeyValuePair<String,ProjectEntity> ProjectEntityPair in ProductEntity.ProjectEntityDictionary)
            {
                ManufactureNumberList.Add(ProjectEntityPair.Key);
            }

            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnOKCommand()
        {
            if (ProductEntity.Validate())
            {
                ProductEntity.DUpdate();
                childWindow.DialogResult = true;
            }
        }

        private void OnCancelCommand()
        {
            ProductEntity.DUpdate();
            ProductEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
