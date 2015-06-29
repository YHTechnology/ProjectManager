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
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.ComponentModel;

using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;
using Microsoft.Windows.Data.DomainServices;

using ProductManager.Web.Model;
using ProductManager.Controls;

namespace ProductManager.ViewModel.ProductManagers
{
    public class LinkFileViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<ProjectEntity> ProjectEntityList { get; set; }
        public ObservableCollection<ProjectFilesEntity> ProjectFilesEntityList { get; set; }

        public ObservableCollection<ProjectEntity> ProjectLinkEntityList { get; set; }

        private ProductDomainContext ProductDomainContextForFile;
        private Dictionary<int, DepartmentEntity> DepartmentDictionary { get; set; }
        private Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
        private Dictionary<int, FileTypeEntity> FileTypeEntityDictionary { get; set; }
        public string ManufactureNumber;

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> projectSource;

        private DomainCollectionView<ProductManager.Web.Model.project_files> projectFileView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project_files> projectFileLoader;
        private EntityList<ProductManager.Web.Model.project_files> projectFileSource;

        public String FilterContext { get; set; }

        public ICommand OnReflash { get; private set; }
        public ICommand OnAddToProject { get; private set; }
        public ICommand OnRemoveProject { get; private set; }

        public LinkFileViewModel( string aManufactureNumber, ObservableCollection<ProjectEntity> aProjectEntityList,
                    Dictionary<int, DepartmentEntity> aDepartmentDictionary,
                    Dictionary<int, UserEntity> aUserEntityDictionary,
                    Dictionary<int, FileTypeEntity> aFileTypeEntityDictionary)
        {
            ManufactureNumber = aManufactureNumber;

            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            ProjectLinkEntityList = new ObservableCollection<ProjectEntity>();

            ProjectFilesEntityList = new ObservableCollection<ProjectFilesEntity>();
            ProductDomainContextForFile = new ProductDomainContext();
            DepartmentDictionary = aDepartmentDictionary;
            UserEntityDictionary = aUserEntityDictionary;
            FileTypeEntityDictionary = aFileTypeEntityDictionary;

            OnReflash = new DelegateCommand(OnReflashCommand);

            OnAddToProject = new DelegateCommand(OnAddToProjectCommand);
            OnRemoveProject = new DelegateCommand(OnRemoveProjectCommand);

            projectSource = new EntityList<ProductManager.Web.Model.project>(ProductDomainContextForFile.projects);
            projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                LoadProjectEntities,
                LoadOperationProjectCompleted
                );
            projectView = new DomainCollectionView<ProductManager.Web.Model.project>(projectLoader, projectSource);

            projectFileSource = new EntityList<ProductManager.Web.Model.project_files>(ProductDomainContextForFile.project_files);
            projectFileLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project_files>(
                LoadProjectFileEntities,
                loadOperationProjectFiles_Completed
                );
            projectFileView = new DomainCollectionView<ProductManager.Web.Model.project_files>(projectFileLoader, projectFileSource);

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
                if (selectProjectEntity != value)
                {
                    selectProjectEntity = value;
                    if (null != selectProjectEntity)
                    {
                        //LoadData();
                        using (projectFileView.DeferRefresh())
                        {
                            projectFileView.MoveToFirstPage();
                        }
                    }
                    UpdateChanged("SelectProjectEntity");
                }
            }
        }

        private ProjectEntity selectLinkProjectEntity;
        public ProjectEntity SelectLinkProjectEntity
        {
            get
            {
                return selectLinkProjectEntity;
            }
            set
            {
                if (selectLinkProjectEntity != value)
                {
                    selectLinkProjectEntity = value;
                    UpdateChanged("SelectLinkProjectEntity");
                }
            }
        }

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
                    UpdateChanged("SelectProjectFilesEntity");
                    canConfirm = isLinkProject ? (ProjectFilesEntityList.Count > 0) : (null != SelectProjectFilesEntity);
                    UpdateChanged("CanConfirm");
                }
            }
        }

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

        private bool canConfirm = false;
        public bool CanConfirm
        {
            get { return canConfirm; }
            set
            {
                if (canConfirm != value)
                {
                    canConfirm = value;
                    UpdateChanged("CanConfirm");
                }
            }
        }

        private bool isLinkProject = true;
        public bool IsLinkProject
        {
            get { return isLinkProject; }
            set
            {
                if (isLinkProject != value)
                {
                    isLinkProject = value;
                    UpdateChanged("IsLinkProject");
                    UpdateChanged("IsLinkFile");
                    canConfirm = isLinkProject ? (ProjectFilesEntityList.Count > 0) : (null != SelectProjectFilesEntity);
                    UpdateChanged("CanConfirm");
                }
            }
        }

        public bool IsLinkFile
        {
            get { return !isLinkProject; }
        }

        public void LoadData()
        {
            IsBusy = true;
            //if (SelectProjectEntity == null)
            //{
            //    ProjectFilesEntityList.Clear();
            //}
            //else
            // {
                //LoadOperation<ProductManager.Web.Model.project_files> loadOperationProjectFiles =
                //    ProductDomainContextForFile.Load<ProductManager.Web.Model.project_files>(ProductDomainContextForFile.GetProject_filesByIDQuery(SelectProjectEntity.ManufactureNumber));
                //loadOperationProjectFiles.Completed += loadOperationProjectFiles_Completed;
            //}
            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }

        void OnReflashCommand()
        {
            IsBusy = true;
            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }

        void OnAddToProjectCommand()
        {
            if (SelectProjectEntity != null )
            {
                bool lIsAdded = false;
                foreach (ProjectEntity projectEntity in ProjectLinkEntityList)
                {
                    if (projectEntity.ManufactureNumber == SelectProjectEntity.ManufactureNumber)
                    {
                        lIsAdded = true;
                        break;
                    }
                }

                if (!lIsAdded)
                {
                    ProjectLinkEntityList.Add(selectProjectEntity);
                }
            }
        }

        void OnRemoveProjectCommand()
        {
            if (selectLinkProjectEntity != null)
            {
                ProjectLinkEntityList.Remove(selectLinkProjectEntity);
            }
        }

        private void loadOperationProjectFiles_Completed(LoadOperation<ProductManager.Web.Model.project_files> aLoadOperation)
        {
            ProjectFilesEntityList.Clear();
            //LoadOperation<ProductManager.Web.Model.project_files> loadOperation = sender as LoadOperation<ProductManager.Web.Model.project_files>;
            foreach (ProductManager.Web.Model.project_files project_files in aLoadOperation.Entities)
            {
                ProjectFilesEntity projectFilesEntity = new ProjectFilesEntity();
                projectFilesEntity.ProjectFiles = project_files;
                projectFilesEntity.Update();
                DepartmentEntity lDepartmentEntity;
                if (DepartmentDictionary.TryGetValue(projectFilesEntity.DepartmentID, out lDepartmentEntity))
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
            UpdateChanged("ProjectFilesEntityList");
            canConfirm = isLinkProject ? (ProjectFilesEntityList.Count > 0) : (null != SelectProjectFilesEntity);
            UpdateChanged("CanConfirm");
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            EntityQuery<ProductManager.Web.Model.project> lQuery = ProductDomainContextForFile.GetProjectQuery();
            
            if (FilterContext != null)
            {
                lQuery = lQuery.Where(c => c.manufacture_number.ToLower().Contains(FilterContext.ToLower()) || c.project_name.ToLower().Contains(FilterContext.ToLower()));
            }
           
            return ProductDomainContextForFile.Load(lQuery.SortAndPageBy(this.projectView));
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
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project_files> LoadProjectFileEntities()
        {
            IsBusy = true;
            //NotifyWindow lNotifyWindow = new NotifyWindow("11", "111");
            //lNotifyWindow.Show();
//             try
//             {
            EntityQuery<ProductManager.Web.Model.project_files> lQuery = ProductDomainContextForFile.GetProject_filesQuery();
            if (SelectProjectEntity != null)
            {
                lQuery = lQuery.Where(c => c.manufacture_number == selectProjectEntity.ManufactureNumber);
            }
            else
            {
                lQuery = lQuery.Where(c => c.manufacture_number == "-1");
            }
            return ProductDomainContextForFile.Load(lQuery.SortAndPageBy(this.projectFileView));
//             }
//             catch (Exception e)
//             {
//                 NotifyWindow lNotifyWindow = new NotifyWindow("", e.Message);
//                 lNotifyWindow.Show();
//             }
            //NotifyWindow lNotifyWindow2 = new NotifyWindow("11", "1112222");
            //lNotifyWindow2.Show();
            //return null;
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project_files> aLoadOperation)
        {
            ProjectFilesEntityList.Clear();
            foreach (ProductManager.Web.Model.project_files project_files in aLoadOperation.Entities)
            {
                ProjectFilesEntity projectFilesEntity = new ProjectFilesEntity();
                projectFilesEntity.ProjectFiles = project_files;
                projectFilesEntity.Update();
                DepartmentEntity lDepartmentEntity;
                if (DepartmentDictionary.TryGetValue(projectFilesEntity.DepartmentID, out lDepartmentEntity))
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
            UpdateChanged("ProjectFilesEntityList");
            canConfirm = isLinkProject ? (ProjectFilesEntityList.Count > 0) : (null != SelectProjectFilesEntity);
            UpdateChanged("CanConfirm");
            IsBusy = false;
        }
    }
}
