using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ModifyFileWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        public FileUploader.UserFile UserFile { get; set; }

        public ProjectFilesEntity ProjectFilesEntity { get; set; }

        public List<String> FileTypes { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        private FileTypeEntity selectFileTypeEntity;
        public FileTypeEntity SelectFileTypeEntity
        {
            get
            {
                return selectFileTypeEntity;
            }
            set
            {
                if (selectFileTypeEntity != value)
                {
                    selectFileTypeEntity = value;
                    UpdateChanged("SelectFileTypeEntity");
                }
            }
        }

        public ModifyFileWindowViewModel(ChildWindow aChildWindow, ObservableCollection<FileTypeEntity> aFileTypeEntityList, ProjectFilesEntity aProjectFileEntity)
        {
            childWindow = aChildWindow;
            FileTypeEntityList = aFileTypeEntityList;

            ProjectFilesEntity = aProjectFileEntity;

            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        void OnOKCommand()
        {
            ProjectFilesEntity.DUpdate();
            childWindow.DialogResult = true;
        }

        void OnCancelCommand()
        {
            ProjectFilesEntity.Update();
            childWindow.DialogResult = false;
        }
    }
}
