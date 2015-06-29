using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client;
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
using ProductManager.Views.SystemManager;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.SystemManager
{
    [Export("FileTypeManager")]
    public class FileTypeManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;// = new SystemManageDomainContext();

        public ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        private FileTypeEntity selectFileTypeEntity;
        public FileTypeEntity SelectFileTypeEntity
        {
            get { return selectFileTypeEntity; }
            set
            {
                if (selectFileTypeEntity != value)
                {
                    selectFileTypeEntity = value;
                    UpdateChanged("SelectFileTypeEntity");
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnAdd { get; private set; }
        public ICommand OnModify { get; private set; }
        public ICommand OnSave { get; private set; }
        public ICommand DoubleClick { get; private set; }

        public FileTypeEntity AddFileTypeEntity { get; set; }

        private void OnAddCommand()
        {
            AddFileTypeEntity = new FileTypeEntity();
            AddFileTypeEntity.FileType = new ProductManager.Web.Model.filetype();
            FileTypeWindow fileTypeWindow = new FileTypeWindow(AddFileTypeEntity);
            fileTypeWindow.Closed += fileTypeWindow_Closed;
            fileTypeWindow.Show();
        }

        void fileTypeWindow_Closed(object sender, EventArgs e)
        {
            FileTypeWindow fileTypeWindow = sender as FileTypeWindow;
            if (fileTypeWindow.DialogResult == true)
            {
                FileTypeEntityList.Add(AddFileTypeEntity);
                systemManageDomainContext.filetypes.Add(AddFileTypeEntity.FileType);
                OnSaveCommand();
            }
        }

        private void OnModifyCommand()
        {
            if (SelectFileTypeEntity != null)
            {
                FileTypeWindow fileTypeWindow = new FileTypeWindow(SelectFileTypeEntity);
                fileTypeWindow.Closed += fileTypeWindowOnModify_Closed;
                fileTypeWindow.Show();
            }
        }

        void fileTypeWindowOnModify_Closed(object sender, EventArgs e)
        {
            FileTypeWindow fileTypeWindow = sender as FileTypeWindow;
            if (fileTypeWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyCommand(object aObject)
        {
            if (selectFileTypeEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnSaveCommand()
        {
            IsBusy = true;
            SubmitOperation submitOperation = systemManageDomainContext.SubmitChanges();
            submitOperation.Completed += submitOperation_Completed;
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        private bool CanSaveCommand(object aObject)
        {
            if (systemManageDomainContext.HasChanges)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoubleClickCommand()
        {
            if (SelectFileTypeEntity != null)
            {
                FileTypeWindow fileTypeWindow = new FileTypeWindow(SelectFileTypeEntity);
                fileTypeWindow.Closed += fileTypeWindowOnModify_Closed;
                fileTypeWindow.Show();
            }
        }

        public FileTypeManagerViewModel()
        {
            FileTypeEntityList = new ObservableCollection<FileTypeEntity>();

            OnAdd = new DelegateCommand(OnAddCommand);
            OnModify = new DelegateCommand(OnModifyCommand, CanModifyCommand);
            OnSave = new DelegateCommand(OnSaveCommand, CanSaveCommand);
            DoubleClick = new DelegateCommand(DoubleClickCommand);

            //systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
        }

        void systemManageDomainContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        public void LoadData()
        {
            IsBusy = true;
            systemManageDomainContext = new SystemManageDomainContext();
            systemManageDomainContext.PropertyChanged -= systemManageDomainContext_PropertyChanged;
            systemManageDomainContext.PropertyChanged += systemManageDomainContext_PropertyChanged;
            FileTypeEntityList.Clear();
            selectFileTypeEntity = null;
            LoadOperation<ProductManager.Web.Model.filetype> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.filetype>(systemManageDomainContext.GetFiletypeQuery());
            loadOperationDepartment.Completed += loadOperationFileType_Completed;
        }

        void loadOperationFileType_Completed(object sender, EventArgs e)
        {
            FileTypeEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.filetype filetype in loadOperation.Entities)
            {
                FileTypeEntity fileTypeEntity = new FileTypeEntity();
                fileTypeEntity.FileType = filetype;
                fileTypeEntity.Update();
                FileTypeEntityList.Add(fileTypeEntity);
            }
            UpdateChanged("FileTypeEntityList");
            IsBusy = false;
        }

        public void ConfirmLeave()
        {
            if (systemManageDomainContext.HasChanges)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow("保存", "有改变，是否保存？");
                confirmWindow.Closed += new EventHandler(Confirm_Closed);
                confirmWindow.Show();
            }
        }

        void Confirm_Closed(object sender, EventArgs e)
        {
            ConfirmWindow confirmWindow = sender as ConfirmWindow;
            if (confirmWindow.DialogResult == true)
            {
                IsBusy = true;
                systemManageDomainContext.SubmitChanges();
            }
            else
            {
                systemManageDomainContext.RejectChanges();
            }
        }

    }
}
