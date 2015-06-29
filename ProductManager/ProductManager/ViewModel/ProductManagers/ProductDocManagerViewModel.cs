using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Microsoft.Windows.Data.DomainServices;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Views.ProductManagers;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("ProductDocManager")]
    public class ProductDocManagerViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext;// = new ProductDomainContext();
        private SystemManageDomainContext SystemManageDomainContext;// = new SystemManageDomainContext();

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> prjectSource;

        private Dictionary<int, DepartmentEntity> DepartmentEntityDictionary;
        private Dictionary<int, UserEntity> UserEntityDictionary;

        private Dictionary<int, FileTypeEntity> FileTypeEntityDictionary;
        private ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        public ObservableCollection<ProjectEntity> ProjectEntityList { get; set; }
        public ObservableCollection<ProjectFilesEntity> ProjectFilesEntityList { get; set; }
        
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

        private ProjectEntity selectProjectEntity;
        public ProjectEntity SelectProjectEntity
        {
            get
            {
                return selectProjectEntity;
            }
            set
            {
                if (selectProjectEntity != value && value != null)
                {
                    selectProjectEntity = value;
                    UpdateChanged("SelectProjectEntity");
                    (OnUpdateFile as DelegateCommand).RaiseCanExecuteChanged();
                    LoadProjectFiles();
                }
            }
        }

        public ICollectionView CollectionProjectView
        {
            get
            {
                return projectView;
            }
        }

        public ProjectFilesEntity AddProjectFilesEntity { get; set; }
        public ICommand OnUpdateFile { get; private set; }
        private ProjectFilesEntity selectProjectFilesEntity;
        public ProjectFilesEntity SelectProjectFilesEntity
        {
            get
            {
                return selectProjectFilesEntity;
            }
            set
            {
                if (selectProjectFilesEntity != value)
                {
                    selectProjectFilesEntity = value;
                    if (selectProjectFilesEntity != null)
                    {
                        selectProjectFilesEntity.Update();
                    }
                    UpdateChanged("SelectProjectFilesEntity");
                    (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnDeleteFile { get; private set; }

        private void OnDeleteFileCommand()
        {
            DeleteFileWindow deleteFileWindow = new DeleteFileWindow(SelectProjectFilesEntity);
            deleteFileWindow.Closed += deleteFileWindow_Closed;
            deleteFileWindow.Show();
        }

        void deleteFileWindow_Closed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = ProductDomainContext.SubmitChanges();
            submitOperation.Completed += submitOperation_Completed;
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "删除失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("删除成功", "删除成功！");
                notifyWindow.Show();
                LoadData();
            }
        }

        private bool CanDeleteFileCommand(object aObject)
        {
            if (selectProjectFilesEntity != null && !selectProjectFilesEntity.FileDelete)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand OnViewDeleteFile { get; private set; }

        private void OnViewDeleteFileCommand()
        {
            DeleteFileViewWindow deleteFileViewWindow = new DeleteFileViewWindow(SelectProjectFilesEntity);
            deleteFileViewWindow.Show();
        }

        private bool CanViewDeleteFileCommand(object aObject)
        {
            if (selectProjectFilesEntity != null && selectProjectFilesEntity.FileDelete)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ProductDocManagerViewModel()
        {
            DepartmentEntityDictionary = new Dictionary<int, DepartmentEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            ProjectFilesEntityList = new ObservableCollection<ProjectFilesEntity>();
            FileTypeEntityDictionary = new Dictionary<int,FileTypeEntity>();
            FileTypeEntityList = new ObservableCollection<FileTypeEntity>();
            OnUpdateFile = new DelegateCommand(OnUpdateFileCommand, CanUpdateFileCommand);
            OnDeleteFile = new DelegateCommand(OnDeleteFileCommand, CanDeleteFileCommand);
            OnViewDeleteFile = new DelegateCommand(OnViewDeleteFileCommand, CanViewDeleteFileCommand);
        }

        public void LoadData()
        {
            IsBusy = true;
            ProductDomainContext = new ProductDomainContext();
            SystemManageDomainContext = new SystemManageDomainContext();
            selectProjectEntity = null;
            ProjectFilesEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                SystemManageDomainContext.Load<ProductManager.Web.Model.department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            DepartmentEntityDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmentEntityDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
            }

            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                SystemManageDomainContext.Load<ProductManager.Web.Model.user>(SystemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                UserEntityDictionary.Add(userEntity.UserID, userEntity);
            }

            LoadOperation<ProductManager.Web.Model.filetype> loadOperationFiletype =
                SystemManageDomainContext.Load<ProductManager.Web.Model.filetype>(SystemManageDomainContext.GetFiletypeQuery());
            loadOperationFiletype.Completed += loadOperationFileType_Completed;

        }

        void loadOperationFileType_Completed(object sender, EventArgs e)
        {
            FileTypeEntityDictionary.Clear();
            FileTypeEntityList.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.filetype filetype in loadOperation.Entities)
            {
                FileTypeEntity fileTypeEntity = new FileTypeEntity();
                fileTypeEntity.FileType = filetype;
                fileTypeEntity.Update();
                FileTypeEntityDictionary.Add(fileTypeEntity.FileTypeID, fileTypeEntity);
                FileTypeEntityList.Add(fileTypeEntity);
            }

            this.prjectSource = new EntityList<ProductManager.Web.Model.project>(this.ProductDomainContext.projects);
            this.projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                this.LoadProjectEntities,
                this.LoadOperationProjectCompleted);
            this.projectView = new DomainCollectionView<ProductManager.Web.Model.project>(this.projectLoader, this.prjectSource);
            using (this.CollectionProjectView.DeferRefresh())
            {
                //this.projectView.PageSize = 5;
                this.projectView.MoveToFirstPage();
            }
            this.IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            this.IsBusy = true;
            EntityQuery<ProductManager.Web.Model.project> lQuery = this.ProductDomainContext.GetProjectQuery();
            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project> aLoadOperation)
        {
            ProjectEntityList.Clear();
            foreach (ProductManager.Web.Model.project project in aLoadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();
                ProjectEntityList.Add(projectEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectEntityList");
            UpdateChanged("CollectionProjectView");
            this.IsBusy = false;
        }

        private void OnUpdateFileCommand()
        {
            AddProjectFilesEntity = new ProjectFilesEntity();
            App app = Application.Current as App;
            AddProjectFilesEntity.UserID = app.UserInfo.UserID;
            AddProjectFilesEntity.DepartmentID = app.UserInfo.DepartmentID;
            AddProjectFilesEntity.DepartmentName = app.UserInfo.UserDepartment;
            AddProjectFilesEntity.UserName = app.UserInfo.UserName;
            AddProjectFilesEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddProjectFilesEntity.FileTypeID = 1;
            AddProjectFilesEntity.ProjectFiles = new ProductManager.Web.Model.project_files();
            UpdateFileWindow updateFileWindow = new UpdateFileWindow(FileTypeEntityList, ProjectFilesEntityList, AddProjectFilesEntity);
            updateFileWindow.Closed += updateFileWindow_Closed;
            updateFileWindow.Show();
        }

        void updateFileWindow_Closed(object sender, EventArgs e)
        {
            UpdateFileWindow updateFileWindow = sender as UpdateFileWindow;
            if (updateFileWindow.DialogResult == true)
            {
                ProjectFilesEntityList.Add(AddProjectFilesEntity);
                AddProjectFilesEntity.DUpdate();
                ProductDomainContext.project_files.Add(AddProjectFilesEntity.ProjectFiles);
                IsBusy = true;
                SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
                subOperation.Completed += SubOperationCommpleted;
            }
        }

        private void SubOperationCommpleted(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        private bool CanUpdateFileCommand(object aObject)
        {
            if (SelectProjectEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadProjectFiles()
        {
            IsBusy = true;
            LoadOperation<ProductManager.Web.Model.project_files> loadOperationProjectFiles =
                ProductDomainContext.Load<ProductManager.Web.Model.project_files>(ProductDomainContext.GetProject_filesByIDQuery(SelectProjectEntity.ManufactureNumber));
            loadOperationProjectFiles.Completed += loadOperationProjectFiles_Completed;
        }

        private void loadOperationProjectFiles_Completed(object sender, EventArgs e)
        {
            ProjectFilesEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.project_files> loadOperation = sender as LoadOperation<ProductManager.Web.Model.project_files>;
            foreach (ProductManager.Web.Model.project_files project_files in loadOperation.Entities)
            {
                ProjectFilesEntity projectFilesEntity = new ProjectFilesEntity();
                projectFilesEntity.ProjectFiles = project_files;
                projectFilesEntity.Update();
                DepartmentEntity lDepartmentEntity;
                if (DepartmentEntityDictionary.TryGetValue(projectFilesEntity.DepartmentID, out lDepartmentEntity))
                {
                    projectFilesEntity.DepartmentName = lDepartmentEntity.DepartmentName;
                }
                UserEntity lUserEntity;
                if (UserEntityDictionary.TryGetValue(projectFilesEntity.UserID, out lUserEntity))
                {
                    projectFilesEntity.UserName = lUserEntity.CUserName;
                }
                if (UserEntityDictionary.TryGetValue(projectFilesEntity.FileDeletePersionID, out lUserEntity))
                {
                    projectFilesEntity.FileDeletePersionName = lUserEntity.CUserName;
                }
                FileTypeEntity lFileTypeEntity;
                if (FileTypeEntityDictionary.TryGetValue(projectFilesEntity.FileTypeID, out lFileTypeEntity))
                {
                    projectFilesEntity.FileTypeName = lFileTypeEntity.FileTypeName;
                }
                ProjectFilesEntityList.Add(projectFilesEntity);
            }
            IsBusy = false;
        }
    }
}
