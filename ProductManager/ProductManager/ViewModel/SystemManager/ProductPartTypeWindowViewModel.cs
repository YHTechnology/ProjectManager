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

    public class ProductPartTypeWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProductPartTypeEntity ProductPartTypeEntity { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public ProductPartTypeWindowViewModel(ChildWindow aChildWindow, ProductPartTypeEntity aProductPartTypeEntity)
        {
            childWindow = aChildWindow;
            ProductPartTypeEntity = aProductPartTypeEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.ProductPartTypeEntity.Validate())
            {
                this.ProductPartTypeEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.ProductPartTypeEntity.Update();
            this.ProductPartTypeEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
