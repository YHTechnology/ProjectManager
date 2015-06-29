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
    public enum ImportantPartWindowState : uint
    {
        Add = 0,
        ARIVE = 1,
        OUT = 2,
        MODIFY = 3,
        VIEW = 4
    }

    public class ImportantPartWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ImportantPartEntity ImportantPartEntity { get; set; }

        public ImportantPartWindowState WindowState { get; set; }

        public ICommand OnOk { get; private set; }

        public ICommand OnCancel { get; private set; }

        public bool CanModifyBaseInfo
        {
            get
            {
                if (WindowState != ImportantPartWindowState.VIEW)
                {
                    return true;
                }
                return false;
                /*switch (WindowState)
                {
                    case ImportantPartWindowState.Add:
                        return true;
                    case ImportantPartWindowState.MODIFY:
                        return true;
                }
                return false;*/
            }
        }

        public bool CanModfiyAriveTime
        {
            get
            {
                if (WindowState != ImportantPartWindowState.VIEW)
                {
                    return true;
                }
                return false;
                /*
                switch (WindowState)
                {
                    case ImportantPartWindowState.ARIVE:
                        return true;
                    case ImportantPartWindowState.MODIFY:
                        {
                            //if (ImportantPartEntity.IsArive)
                            {
                                return true;
                            }
                            //else
                            {
                            //    return false;
                            }
                        }
                }
                return false;*/
            }
        }

        public bool CanModifyOutInfo
        {
            get
            {
                if (WindowState != ImportantPartWindowState.VIEW)
                {
                    return true;
                }
                return false;
                /*switch (WindowState)
                {
                    case ImportantPartWindowState.OUT:
                        return true;
                    case ImportantPartWindowState.MODIFY:
                        {
                            //if (ImportantPartEntity.IsOut)
                            {
                                return true;
                            }
                            //else
                            {
                            //    return false;
                            }
                        }
                }
                return false;*/
            }
        }

        public String Title { get; set; }

        public ImportantPartWindowViewModel(ChildWindow aChildWndow, ImportantPartWindowState aImportantPartWindowState, ImportantPartEntity aImportantPartEntity)
        {
            childWindow = aChildWndow;
            WindowState = aImportantPartWindowState;
            ImportantPartEntity = aImportantPartEntity;
            Title = "生产令号：" + aImportantPartEntity.ManufactureNumber;
            OnOk = new DelegateCommand(OnOkCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnOkCommand()
        {
            switch (WindowState)
            {
                case ImportantPartWindowState.Add:
                    {
                        App app = Application.Current as App;
                        ImportantPartEntity.AriveUserId = app.UserInfo.UserID;
                        ImportantPartEntity.AriveInputTime = DateTime.Now;
                    }
                    break;
//                 case ImportantPartWindowState.ARIVE:
//                     {
//                         App app = Application.Current as App;
//                         ImportantPartEntity.AriveUserId = app.UserInfo.UserID;
//                         ImportantPartEntity.AriveInputTime = DateTime.Now;
//                         //ImportantPartEntity.IsArive = true;
//                     }
//                     break;
                case ImportantPartWindowState.MODIFY:
                    {
                        App app = Application.Current as App;
                        ImportantPartEntity.ModifyUserID = app.UserInfo.UserID;
                        ImportantPartEntity.ModifyDateTime = DateTime.Now;
                    }
                    break;
//                 case ImportantPartWindowState.OUT:
//                     {
//                         App app = Application.Current as App;
//                         //ImportantPartEntity.PartOutputUserId = app.UserInfo.UserID;
//                         //ImportantPartEntity.PartOutputInputTime = DateTime.Now;
//                         //ImportantPartEntity.IsOut = true;
//                     }
//                     break;
                case ImportantPartWindowState.VIEW:
                    break;
            }
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
