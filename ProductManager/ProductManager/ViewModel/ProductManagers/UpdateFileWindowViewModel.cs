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
using ProductManager.Controls;
using ProductManager.ViewData.Entity;

namespace ProductManager.ViewModel.ProductManagers
{
    public class UpdateFileWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ObservableCollection<ProjectFilesEntity> ProjectFilesEntityList { get; set; }

        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        public FileUploader.UserFile UserFile { get; set; }

        public ProjectFilesEntity ProjectFilesEntity { get; set; }

        public List<String> FileTypes { get; set; }

        public ICommand OnOpenFile { get; private set; }

        public ICommand OnUpdate { get; private set; }

        public ICommand OnCancel { get; private set; }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public String Title { get; set; }

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

        public UpdateFileWindowViewModel(ChildWindow aChildWindow, ObservableCollection<FileTypeEntity> aFileTypeEntityList, ObservableCollection<ProjectFilesEntity> aProjectFilesEntityList, ProjectFilesEntity aProjectFileEntity)
        {
            UserFile = new FileUploader.UserFile();
            ProjectFilesEntity = aProjectFileEntity;
            ProjectFilesEntityList = aProjectFilesEntityList;
            FileTypeEntityList = aFileTypeEntityList;
            childWindow = aChildWindow;
            FileTypes = new List<String>();
            App app = Application.Current as App;
            bool lIsPermis;
            if (app.UserInfo.UserAction.TryGetValue(2020100, out lIsPermis))
            {
                if (lIsPermis)
                {
                    FileTypes.Add("合同协议文件");
                    ProjectFilesEntity.fileType = "合同协议文件";
                }
            }

            if (app.UserInfo.UserAction.TryGetValue(2020200, out lIsPermis))
            {
                if (lIsPermis)
                {
                    FileTypes.Add("配置文件");
                    ProjectFilesEntity.fileType = "配置文件";
                }
            }

            Title = "上传文件 生产令号：" + ProjectFilesEntity.ManufactureNumber;

            OnOpenFile = new DelegateCommand(OnOpenFileCommand);
            OnUpdate = new DelegateCommand(OnUpdateCommand, CanUpdateCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnOpenFileCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                String fileName = ofd.File.Name;

                if(fileName.Contains("\"")
                    || fileName.Contains("#")
                    || fileName.Contains("%")
                    || fileName.Contains("&")
                    || fileName.Contains("\'")
                    || fileName.Contains("~")
                    || fileName.Contains("|")
                    || fileName.Contains(">")
                    || fileName.Contains("<")
                    || fileName.Contains("[")
                    || fileName.Contains("]")
                    || fileName.Contains("^")
                    || fileName.Contains("{")
                    || fileName.Contains("}"))
                {
                    NotifyWindow notificationWindow = new NotifyWindow("错误", "文件名包含 \"#%&\'~|><[]^{} 等非法字符！");
                    notificationWindow.Show();
                    return;
                }

                foreach (ProjectFilesEntity projectFilesEntity in ProjectFilesEntityList)
                {
                    if (projectFilesEntity.fileName == fileName && !projectFilesEntity.FileDelete)
                    {
                        NotifyWindow notificationWindow = new NotifyWindow("错误", "已上传相同的文件！");
                        notificationWindow.Show();
                        return;
                    }
                }
                ProjectFilesEntity.FileName = fileName;
                UserFile = new FileUploader.UserFile();
                UserFile.FileName = fileName;
                UserFile.FolderName = ProjectFilesEntity.ManufactureNumber;
                UserFile.FileStream = ofd.File.OpenRead();
                ProjectFilesEntity.FileBytes = UserFile.FileStream.Length;
                (OnUpdate as DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        private void OnUpdateCommand()
        {
            IsBusy = true;
            UserFile.FinishUpdate += UserFile_FinishUpdate;
            UserFile.Upload("", childWindow.Dispatcher);
        }

        private bool CanUpdateCommand(object aObject)
        {
            if (UserFile != null && !String.IsNullOrEmpty(UserFile.FileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UserFile_FinishUpdate(object sender, EventArgs e)
        {
            ProjectFilesEntity.FileUpdateTime = DateTime.Now;
            if (SelectFileTypeEntity != null)
            {
                ProjectFilesEntity.FileTypeName = SelectFileTypeEntity.FileTypeName;
            }
            else
            {
                foreach(FileTypeEntity fileTypeEntity in FileTypeEntityList)
                {
                    if (ProjectFilesEntity.FileTypeID == fileTypeEntity.FileTypeID)
                    {
                        ProjectFilesEntity.FileTypeName = fileTypeEntity.FileTypeName;
                    }
                }
            }
            NotifyWindow notificationWindow = new NotifyWindow("上传文件", "上传文件完成！");
            notificationWindow.Show();
            IsBusy = false;
            this.childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            this.childWindow.DialogResult = false;
        }
    }
}
