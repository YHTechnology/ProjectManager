using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;
using ProductManager.Views.ProductManagers;

namespace ProductManager.ViewModel.ProductManagers
{
    public enum LinkType
    {
        LinkProject,
        LinkPerson
    }

    public class LinkResponsePersonModel : NotifyPropertyChanged
    {
        public ObservableCollection<ProjectEntity> ProjectEntityList { get; set; }
        public ObservableCollection<ProjectEntity> ProjectLinkEntityList { get; set; }
        public ObservableCollection<ProjectResponsibleEntity> ProjectResponsibleEntityList { get; set; }

        public ObservableCollection<ProjectResponsibleEntity> ProjectResponsibleEntityALLList { get; set; }

        public String FilterContext { get; set; }

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
                    using (projectResponsibleView.DeferRefresh())
                    {
                        projectResponsibleView.MoveToFirstPage();
                    }

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
                }
            }
        }

        private LinkType linkType = LinkType.LinkProject;
        public LinkType LinkTypes
        {
            get
            {
                return linkType;
            }
            set
            {
                if (linkType != value)
                {
                    linkType = value;
                    if (linkType == LinkType.LinkProject)
                    {
                        IsEnable = false;
                    }
                    else if (linkType == LinkType.LinkPerson)
                    {
                        IsEnable = true;
                    }

                }
            }
        }

        private bool isEnable = false;
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    UpdateChanged("IsEnable");
                }
            }
        }

        private ProjectResponsibleEntity selectProjectResponsibleEntity;
        public ProjectResponsibleEntity SelectProjectResponsibleEntity
        {
            get
            {
                return selectProjectResponsibleEntity;
            }
            set
            {
                if (selectProjectResponsibleEntity != value)
                {
                    selectProjectResponsibleEntity = value;
                }
            }
        }

        public ICommand OnReflash { get; private set; }
        public ICommand OnAddToProject { get; private set; }
        public ICommand OnRemoveProject { get; private set; }
        public ICommand OnOk { get; private set; }
        public ICommand OnCancel { get; private set; }

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> projectSource;

        private DomainCollectionView<ProductManager.Web.Model.project_responsible> projectResponsibleView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible> projectResponsibLoader;
        private EntityList<ProductManager.Web.Model.project_responsible> projectResponsibSource;


        private Dictionary<int, DepartmentEntity> DepartmentEntityDictionary;
        private Dictionary<String, UserEntity> UserEntityDictionary;

        private ProductDomainContext ProductDomainContext;
        private ChildWindow childWidow;

        public LinkResponsePersonModel(ChildWindow aChildWindow
                                      , ProductDomainContext aProductDomainContext
                                      , Dictionary<int, DepartmentEntity> aDepartmentEntityDictionary
                                      , Dictionary<String, UserEntity> aUserEntityDictionary)
        {
            ProductDomainContext = aProductDomainContext;
            childWidow = aChildWindow;
            DepartmentEntityDictionary = aDepartmentEntityDictionary;
            UserEntityDictionary = aUserEntityDictionary;

            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            ProjectLinkEntityList = new ObservableCollection<ProjectEntity>();
            ProjectResponsibleEntityList = new ObservableCollection<ProjectResponsibleEntity>();
            ProjectResponsibleEntityALLList = new ObservableCollection<ProjectResponsibleEntity>();


            OnReflash = new DelegateCommand(OnReflashCommand);
            OnAddToProject = new DelegateCommand(OnAddToProjectCommand);
            OnRemoveProject = new DelegateCommand(OnRemoveProjectCommand);
            OnOk = new DelegateCommand(OnOkCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);

            projectSource = new EntityList<ProductManager.Web.Model.project>(ProductDomainContext.projects);
            projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                LoadProjectEntities,
                LoadOperationProjectCompleted
                );
            projectView = new DomainCollectionView<ProductManager.Web.Model.project>(projectLoader, projectSource);

            projectResponsibSource = new EntityList<ProductManager.Web.Model.project_responsible>(ProductDomainContext.project_responsibles);
            projectResponsibLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible>(
                LoadProjectResponseEntities,
                LoadOperationProjectResponseCompleted
                );
            projectResponsibleView = new DomainCollectionView<ProductManager.Web.Model.project_responsible>(projectResponsibLoader, projectResponsibSource);
        }


        public void LoadData()
        {
            LoadOperation<ProductManager.Web.Model.project_responsible> loadOperationProject =
                ProductDomainContext.Load<ProductManager.Web.Model.project_responsible>(ProductDomainContext.GetProject_responsibleQuery());
            loadOperationProject.Completed += loadOperationProjectReponse_Completed;
        }

        private void loadOperationProjectReponse_Completed(Object sender, EventArgs e)
        {
            ProjectResponsibleEntityALLList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.project_responsible project_responsible in loadOperation.Entities)
            {
                ProjectResponsibleEntity projectResponsibleEntity = new ProjectResponsibleEntity();
                projectResponsibleEntity.ProjectResponsible = project_responsible;
                projectResponsibleEntity.Update();
                ProjectResponsibleEntityALLList.Add(projectResponsibleEntity);
            }

            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }


        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            EntityQuery<ProductManager.Web.Model.project> lQuery = ProductDomainContext.GetProjectQuery();

            if (FilterContext != null)
            {
                lQuery = lQuery.Where(c => c.manufacture_number.ToLower().Contains(FilterContext.ToLower()) || c.project_name.ToLower().Contains(FilterContext.ToLower()));
            }

            return ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
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
        }

        private LoadOperation<ProductManager.Web.Model.project_responsible> LoadProjectResponseEntities()
        {
            EntityQuery<ProductManager.Web.Model.project_responsible> lQuery = ProductDomainContext.GetProject_responsibleQuery();

            if (SelectProjectEntity != null)
            {
                lQuery = lQuery.Where(c => c.manufacture_number == SelectProjectEntity.ManufactureNumber);
            }

            return ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectResponsibleView));
        }

        private void LoadOperationProjectResponseCompleted(LoadOperation<ProductManager.Web.Model.project_responsible> aLoadOperation)
        {
            ProjectResponsibleEntityList.Clear();
            foreach (ProductManager.Web.Model.project_responsible projectResponsible in aLoadOperation.Entities)
            {
                ProjectResponsibleEntity projectResponsibleEntity = new ProjectResponsibleEntity();
                projectResponsibleEntity.ProjectResponsible = projectResponsible;
                projectResponsibleEntity.Update();

                DepartmentEntity lDepartmentEntityTemp;
                if (DepartmentEntityDictionary.TryGetValue(projectResponsibleEntity.DepartmentID, out lDepartmentEntityTemp))
                {
                    projectResponsibleEntity.DepartmentName = lDepartmentEntityTemp.DepartmentName;
                }

                if (projectResponsibleEntity.ResponsiblePersionName == null)
                {
                    continue;
                }

                UserEntity lUserEntityTemp;
                if (UserEntityDictionary.TryGetValue(projectResponsibleEntity.ResponsiblePersionName, out lUserEntityTemp))
                {
                    projectResponsibleEntity.UserPhoneNumber = lUserEntityTemp.UserPhoneNumber;
                }

                ProjectResponsibleEntityList.Add(projectResponsibleEntity);
            }
        }

        void OnReflashCommand()
        {
            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }

        void OnAddToProjectCommand()
        {
            if (SelectProjectEntity != null)
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

        void OnOkCommand()
        {
            if (linkType == LinkType.LinkProject)
            {
                foreach (ProjectResponsibleEntity lProjectResponsibleEntity in ProjectResponsibleEntityList)
                {
                    foreach (ProjectEntity lProjectEntity in ProjectLinkEntityList)
                    {
                        bool lHasAdd = false;
                        foreach (ProjectResponsibleEntity lProjectResponsibleEntityCheck in  ProjectResponsibleEntityALLList)
                        {
                            if (lProjectResponsibleEntityCheck.ManufactureNumber == lProjectEntity.ManufactureNumber
                                && lProjectResponsibleEntityCheck.DepartmentID == lProjectResponsibleEntity.DepartmentID
                                && lProjectResponsibleEntityCheck.ResponsiblePersionName == lProjectResponsibleEntity.ResponsiblePersionName)
                            {
                                lHasAdd = true;
                                break;
                            }
                        }

                        if (lHasAdd)
                        {
                            continue;
                        }
                        ProjectResponsibleEntity lProjectResponseibleEntity = new ProjectResponsibleEntity();
                        lProjectResponseibleEntity.ProjectResponsible = new ProductManager.Web.Model.project_responsible();

                        lProjectResponseibleEntity.ManufactureNumber = lProjectEntity.ManufactureNumber;
                        lProjectResponseibleEntity.DepartmentID = lProjectResponsibleEntity.DepartmentID;
                        lProjectResponseibleEntity.ResponsiblePersionName = lProjectResponsibleEntity.ResponsiblePersionName;
                        lProjectResponseibleEntity.Descript = lProjectResponsibleEntity.Descript;

                        lProjectResponseibleEntity.DUpdate();
                        ProductDomainContext.project_responsibles.Add(lProjectResponseibleEntity.ProjectResponsible);
                    }
                }
                

            }
            else if (linkType == LinkType.LinkPerson)
            {
                if (selectProjectResponsibleEntity != null)
                {
                    LinkResponsePersonWindow lLinkResponsePersonWindow = childWidow as LinkResponsePersonWindow;
                    foreach (ProjectResponsibleEntity lSProjectResponsibleEntity in lLinkResponsePersonWindow.projectPersonDataGrid.SelectedItems)
                    {
                        ProjectResponsibleEntity lProjectResponsibleEntity = lSProjectResponsibleEntity;
                        foreach (ProjectEntity lProjectEntity in ProjectLinkEntityList)
                        {
                            bool lHasAdd = false;
                            foreach (ProjectResponsibleEntity lProjectResponsibleEntityCheck in ProjectResponsibleEntityALLList)
                            {
                                if (lProjectResponsibleEntityCheck.ManufactureNumber == lProjectEntity.ManufactureNumber
                                    && lProjectResponsibleEntityCheck.DepartmentID == lProjectResponsibleEntity.DepartmentID
                                    && lProjectResponsibleEntityCheck.ResponsiblePersionName == lProjectResponsibleEntity.ResponsiblePersionName)
                                {
                                    lHasAdd = true;
                                    break;
                                }
                            }

                            if (lHasAdd)
                            {
                                continue;
                            }

                            ProjectResponsibleEntity lProjectResponseibleEntity = new ProjectResponsibleEntity();
                            lProjectResponseibleEntity.ProjectResponsible = new ProductManager.Web.Model.project_responsible();

                            lProjectResponseibleEntity.ManufactureNumber = lProjectEntity.ManufactureNumber;
                            lProjectResponseibleEntity.DepartmentID = lProjectResponsibleEntity.DepartmentID;
                            lProjectResponseibleEntity.ResponsiblePersionName = lProjectResponsibleEntity.ResponsiblePersionName;
                            lProjectResponseibleEntity.Descript = lProjectResponsibleEntity.Descript;

                            lProjectResponseibleEntity.DUpdate();
                            ProductDomainContext.project_responsibles.Add(lProjectResponseibleEntity.ProjectResponsible);
                        }
                    }
                }
            }



            childWidow.DialogResult = true;
        }

        void OnCancelCommand()
        {
            childWidow.DialogResult = false;
        }

    }
}
