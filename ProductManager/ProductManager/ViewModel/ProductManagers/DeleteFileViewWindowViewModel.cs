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
    public class DeleteFileViewWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProjectFilesEntity ProjectFileEntity { get; set; }

        public String Title { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public DeleteFileViewWindowViewModel(ChildWindow aChildWindow, ProjectFilesEntity aProjectFileEntity)
        {
            childWindow = aChildWindow;
            ProjectFileEntity = aProjectFileEntity;
            Title = "已删除文件：" + ProjectFileEntity.FileName;

            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnOKCommand()
        {
            childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            childWindow.DialogResult = false;
        }
    }
}
