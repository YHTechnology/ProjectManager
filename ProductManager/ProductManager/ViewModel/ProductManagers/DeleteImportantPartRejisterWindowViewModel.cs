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
    public class DeleteImportantPartRejisterWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ImportantPartRejesterEntity ImportantPartRejesterEntity { get; set; }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public String Title { get; set; }

        public DeleteImportantPartRejisterWindowViewModel(ChildWindow aChildWindow, ImportantPartRejesterEntity aImportantPartRejesterEntity)
        {
            childWindow = aChildWindow;
            ImportantPartRejesterEntity = aImportantPartRejesterEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);

            Title = "删除：" + aImportantPartRejesterEntity.ImportantPartName;
        }

        private void OnOKCommand()
        {
            ImportantPartRejesterEntity.IsDelete = true;
            App app = Application.Current as App;

            ImportantPartRejesterEntity.DeleteUserID = app.UserInfo.UserID;
            ImportantPartRejesterEntity.DeleteDateTime = DateTime.Now;

            ImportantPartRejesterEntity.DUpdate();
            childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            ImportantPartRejesterEntity.Update();
            ImportantPartRejesterEntity.RaisALL();
            childWindow.DialogResult = false;
        }

    }
}
