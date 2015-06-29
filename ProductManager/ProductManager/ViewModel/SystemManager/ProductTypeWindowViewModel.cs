using System;
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

namespace ProductManager.ViewModel.SystemManager
{
    public class ProductTypeWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProductTypeEntity ProductTypeEntity { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public ProductTypeWindowViewModel(ChildWindow aChildWindow, ProductTypeEntity aProductTypeEntity)
        {
            childWindow = aChildWindow;
            ProductTypeEntity = aProductTypeEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.ProductTypeEntity.Validate())
            {
                this.ProductTypeEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.ProductTypeEntity.Update();
            this.ProductTypeEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
