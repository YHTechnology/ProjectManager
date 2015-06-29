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
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;
using ProductManager.FileUploader;
using ProductManager.Controls;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("ProductDocView")]
    public class ProductDocViewViewModel : NotifyPropertyChanged
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

        private String filterContent;
        public String FilterContent
        {
            get
            {
                return filterContent;
            }
            set
            {
                if (filterContent != value)
                {
                    filterContent = value;
                    UpdateChanged("FilterContent");
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        ProjectFilesEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                        UpdateChanged("ProjectFilesEntityList");
                    }
                    
                }
            }
        }

        private bool isFilter;
        public bool IsFilter
        {
            get
            {
                return isFilter;
            }
            set
            {
                if (isFilter != value)
                {
                    isFilter = value;
                    UpdateChanged("IsFilter");
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        ProjectFilesEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                        UpdateChanged("ProjectFilesEntityList");
                    }
                }
            }
        }

        public ObservableCollection<String> FilterList { get; set; }

        private String selectFilerList;
        public String SelectFilerList
        {
            get
            {
                return selectFilerList;
            }
            set
            {
                if (selectFilerList != value)
                {
                    selectFilerList = value;
                    UpdateChanged("SelectFilerList");
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        ProjectFilesEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                        UpdateChanged("ProjectFilesEntityList");
                    }
                }
            }
        }

        public ICommand OnRefash { get; private set; }

        private void OnRefashCommand()
        {
            selectProjectEntity = null;

            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }

        public ProductDocViewViewModel()
        {
            DepartmentEntityDictionary = new Dictionary<int, DepartmentEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            ProjectFilesEntityList = new ObservableCollection<ProjectFilesEntity>();
            FileTypeEntityDictionary = new Dictionary<int, FileTypeEntity>();
            FileTypeEntityList = new ObservableCollection<FileTypeEntity>();
            FilterList = new ObservableCollection<string>();

            OnRefash = new DelegateCommand(OnRefashCommand);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);

            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";
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
                this.projectView.MoveToFirstPage();
            }
            
            this.IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            if (!isFilter)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.ProductDomainContext.GetProjectQuery();
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.ProductDomainContext.GetProjectQuery();
                if (selectFilerList == "生产令号")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        lQuery = lQuery.Where(e => e.manufacture_number.Contains(filterContent));
                    }
                }
                if (selectFilerList == "项目名称")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        lQuery = lQuery.Where(e => e.project_name.Contains(filterContent));
                    }
                }
                if (selectFilerList == "备注")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        lQuery = lQuery.Where(e => e.remark.Contains(filterContent));
                    }
                }
                if (selectFilerList == "年份")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        try
                        {
                            int year = Convert.ToInt32(filterContent);
                            lQuery = lQuery.Where(e => e.year_number == year);
                        }
                        catch (System.Exception ex)
                        {
                            NotifyWindow notifyWindow = new NotifyWindow("错误", "输入年份不合法");
                            notifyWindow.Show();
                        }

                    }
                }

                if (selectFilerList == "记录时间")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        try
                        {
                            DateTime remarkDate = Convert.ToDateTime(filterContent);
                            lQuery = lQuery.Where(e => e.record_date.Value.Year == remarkDate.Year && e.record_date.Value.Month == remarkDate.Month && e.record_date.Value.Day == remarkDate.Day);
                        }
                        catch (System.Exception ex)
                        {
                            NotifyWindow notifyWindow = new NotifyWindow("错误", "记录时间不合法 (YYYY-MM-DD)");
                            notifyWindow.Show();
                        }
                    }
                }
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project> aLoadOperation)
        {
            ProjectEntityList.Clear();
            ProjectFilesEntityList.Clear();
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
            UpdateChanged("ProjectFilesEntityList");
            UpdateChanged("RecorderCount");
            this.IsBusy = false;
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
                projectFilesEntity.FileUrl = CustomUri.GetAbsoluteUrl(projectFilesEntity.ManufactureNumber + "/" + projectFilesEntity.FileName);
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
                FileTypeEntity lFileTypeEntity;
                if (FileTypeEntityDictionary.TryGetValue(projectFilesEntity.FileTypeID, out lFileTypeEntity))
                {
                    projectFilesEntity.FileTypeName = lFileTypeEntity.FileTypeName;
                }
                ProjectFilesEntityList.Add(projectFilesEntity);
            }
            IsBusy = false;
        }

        private bool showExpander = false;
        public bool ShowExpander
        {
            get
            {
                return showExpander;
            }
            set
            {
                //if (showExpander != value)
                {
                    showExpander = value;
                    UpdateChanged("ShowExpander");
                }
            }
        }

        public void HideExpander()
        {
            ShowExpander = false;
        }

        public ICommand DoubleClickProject { get; private set; }

        private void DoubleClickProjectCommand()
        {
            ShowExpander = true;
        }

        public String RecorderCount
        {
            get
            {
                if (projectView != null)
                {
                    return "总共 " + projectView.ItemCount.ToString() + " 个项目";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
