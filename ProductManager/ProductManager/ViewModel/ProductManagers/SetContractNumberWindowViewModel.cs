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

namespace ProductManager.ViewModel.ProductManagers
{
    public class SetContractNumberWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProjectEntity ProjectEntity { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public SetContractNumberWindowViewModel(ChildWindow aChileWindow, ProjectEntity aProjectEntity)
        {
            childWindow = aChileWindow;
            ProjectEntity = aProjectEntity;

            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            ProjectEntity.DUpdate();
            childWindow.DialogResult = true;
        }

        public void OnCancelCommand()
        {
            ProjectEntity.Update();
            ProjectEntity.RaisALL();
            childWindow.DialogResult = false;
        }
    }
}
