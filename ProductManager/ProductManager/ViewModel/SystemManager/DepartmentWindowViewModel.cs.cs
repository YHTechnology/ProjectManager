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
    public class DepartmentWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public DepartmentEntity DepartmentEntity { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public DepartmentWindowViewModel(ChildWindow aChildWindow, DepartmentEntity aDepartmentEntity)
        {
            childWindow = aChildWindow;
            DepartmentEntity = aDepartmentEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.DepartmentEntity.Validate())
            {
                this.DepartmentEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.DepartmentEntity.Update();
            this.DepartmentEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
