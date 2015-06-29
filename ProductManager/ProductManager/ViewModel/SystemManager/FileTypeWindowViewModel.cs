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
    public class FileTypeWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public FileTypeEntity FileTypeEntity { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public FileTypeWindowViewModel(ChildWindow aChildWindow, FileTypeEntity aFileTypeEntity)
        {
            childWindow = aChildWindow;
            FileTypeEntity = aFileTypeEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.FileTypeEntity.Validate())
            {
                this.FileTypeEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.FileTypeEntity.Update();
            this.FileTypeEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
