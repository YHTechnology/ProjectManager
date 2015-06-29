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
    public class DeleteFileWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProjectFilesEntity ProjectFilesEntity { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public String Title { get; set; }

        public DeleteFileWindowViewModel(ChildWindow aChildWindow, ProjectFilesEntity aProjectFilesEntity)
        {
            childWindow = aChildWindow;
            ProjectFilesEntity = aProjectFilesEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);

            Title = "删除文件：" + aProjectFilesEntity.FileName;
        }

        private void OnOKCommand()
        {
            ProjectFilesEntity.FileDelete = true;
            App app = Application.Current as App;

            ProjectFilesEntity.FileDeletePersionID = app.UserInfo.UserID;
            ProjectFilesEntity.FileDeletePersionName = app.UserInfo.UserName;
            ProjectFilesEntity.FileDeleteTime = DateTime.Now;

            ProjectFilesEntity.DUpdate();

            childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            ProjectFilesEntity.Update();
            ProjectFilesEntity.RaisALL();
            childWindow.DialogResult = false;
        }

    }
}
