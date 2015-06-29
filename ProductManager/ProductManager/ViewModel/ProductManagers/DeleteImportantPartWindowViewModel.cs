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
    public class DeleteImportantPartWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ImportantPartEntity ImportantPartEntity { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public String Title { get; set; }

        public DeleteImportantPartWindowViewModel(ChildWindow aChildWindow, ImportantPartEntity aImportantPartEntity)
        {
            childWindow = aChildWindow;
            ImportantPartEntity = aImportantPartEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);

            Title = "删除：" + aImportantPartEntity.ImportantPartName;
        }

        private void OnOKCommand()
        {
            ImportantPartEntity.IsDelete = true;
            App app = Application.Current as App;

            ImportantPartEntity.DeleteUserID = app.UserInfo.UserID;
            ImportantPartEntity.DeleteTime = DateTime.Now;

            ImportantPartEntity.DUpdate();
            childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            ImportantPartEntity.Update();
            ImportantPartEntity.RaisALL();
            childWindow.DialogResult = false;
        }

    }
}
