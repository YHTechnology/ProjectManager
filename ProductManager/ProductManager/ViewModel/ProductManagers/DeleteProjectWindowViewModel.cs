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
    public class DeleteProjectWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        public ProjectEntity ProjectEntity { get; set; }
        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }
        public String Title { get; set; }

        public DeleteProjectWindowViewModel(ChildWindow aChildWindow, ProjectEntity aProjectEntity)
        {
            childWindow = aChildWindow;
            ProjectEntity = aProjectEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            Title = "删除生产项目：" + aProjectEntity.ProjectName;
        }

        private void OnOKCommand()
        {
            ProjectEntity.IsDelete = true;
            ProjectEntity.RaisALL();
            ProjectEntity.DUpdate();
            childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            ProjectEntity.Update();
            ProjectEntity.RaisALL();
            childWindow.DialogResult = true;
        }
    }
}
