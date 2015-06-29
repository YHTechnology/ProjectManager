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
    public enum ImportantPartRejesterWindowState : uint
    {
        Add = 0,
        MODIFY = 1,
        VIEW = 2
    }

    public class ImportantPartRejesterWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ImportantPartRejesterEntity ImportantPartRejesterEntity { get; set; }

        public ImportantPartRejesterWindowState WindowState { get; set; }

        public ICommand OnOk { get; private set; }

        public ICommand OnCancel { get; private set; }

        public bool IsView
        {
            get
            {
                if (WindowState != ImportantPartRejesterWindowState.VIEW)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public String Title { get; set; }

        public ImportantPartRejesterWindowViewModel(ChildWindow aChildWindow, ImportantPartRejesterWindowState aWindowState, ImportantPartRejesterEntity aImportPartRejesterEntity)
        {
            childWindow = aChildWindow;
            WindowState = aWindowState;
            ImportantPartRejesterEntity = aImportPartRejesterEntity;
            Title = "生产令号：" + aImportPartRejesterEntity.ManufactureNumber;
            OnOk = new DelegateCommand(OnOkCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnOkCommand()
        {
            switch (WindowState)
            {
                case ImportantPartRejesterWindowState.Add:
                    {
                        App app = Application.Current as App;
                        ImportantPartRejesterEntity.InputUserID = app.UserInfo.UserID;
                        ImportantPartRejesterEntity.InputDateTime = DateTime.Now;
                    }
                    break;
                case ImportantPartRejesterWindowState.MODIFY:
                    {
                        App app = Application.Current as App;
                        ImportantPartRejesterEntity.ModifyUserID = app.UserInfo.UserID;
                        ImportantPartRejesterEntity.ModifyDateTime = DateTime.Now;
                    }
                    break;
                case ImportantPartRejesterWindowState.VIEW:
                    break;
            }
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
