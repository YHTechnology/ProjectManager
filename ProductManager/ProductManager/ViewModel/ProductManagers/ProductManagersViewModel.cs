using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
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
using Lite.ExcelLibrary.SpreadSheet;
using Microsoft.Windows.Data.DomainServices;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Views.ProductManagers;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("ProductManagers")]
    public class ProductManagersViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext; //= new ProductDomainContext();

        private ProductDomainContext ProductDomainContextForFile; //= new ProductDomainContext();

        private SystemManageDomainContext SystemManageDomainContext; //= new SystemManageDomainContext();

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> prjectSource;

        private DomainCollectionView<ProductManager.Web.Model.product> productView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.product> productLoader;
        private EntityList<ProductManager.Web.Model.product> productSource;

        private DomainCollectionView<ProductManager.Web.Model.project_responsible> projectResponsibleView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible> projectResponsibleLoader;
        private EntityList<ProductManager.Web.Model.project_responsible> projectResponsibleSource;

        private DomainCollectionView<ProductManager.Web.Model.important_part> importantPartView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.important_part> importantPartLoader;
        private EntityList<ProductManager.Web.Model.important_part> importantPartSource;

        private DomainCollectionView<ProductManager.Web.Model.important_part_rejester> importantPartRejesterView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.important_part_rejester> importantPartRejesterLoader;
        private EntityList<ProductManager.Web.Model.important_part_rejester> importantPartRejesterSource;

        public ObservableCollection<ImportantPartRejesterEntity> ImportantPartRejesterEntityList { get; set; }
        public ObservableCollection<ImportantPartEntity> ImportantPartEntityList { get; set; }
        public ObservableCollection<ProjectEntity> ProjectEntityList { get; set; }
        public ObservableCollection<ProductEntity> ProductEntityList { get; set; }
        public ObservableCollection<ProjectResponsibleEntity> ProjectResponsibleEntityList { get; set; }

        public ObservableCollection<DepartmentEntity> DepartmemtList { get; set; }
        private Dictionary<int, DepartmentEntity> DepartmentDictionary { get; set; }

        private Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        private Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
        private Dictionary<String, UserEntity> UserEntityDictionaryName { get; set; }

        private Dictionary<int, FileTypeEntity> FileTypeEntityDictionary { get; set; }
        private ObservableCollection<FileTypeEntity> FileTypeEntityList { get; set; }

        private Dictionary<int, ProductTypeEntity> ProductTypeEntityDictionary { get; set; }
        private ObservableCollection<ProductTypeEntity> ProductTypeEntityList { get; set; }

        private Dictionary<String, ProductEntity> ProductEntityDictionary { get; set; }

        public ObservableCollection<ProjectFilesEntity> ProjectFilesEntityList { get; set; }

        public Dictionary<int, ProductPartTypeEntity> ProductPartTypeDictionary { get; set; }

        public Dictionary<String, UserProjectEntity> UserProjectEntityDictionary { get; set; }

        private int UserProjectCount;

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
                    UpdateChanged("SelectProjectEntity");
                    if (selectProjectEntity != null)
                    {
                        (OnModifyProject as DelegateCommand).RaiseCanExecuteChanged();
                        (OnViewProject as DelegateCommand).RaiseCanExecuteChanged();
                        (OnDeleteProject as DelegateCommand).RaiseCanExecuteChanged();
                        (OnFreezeProject as DelegateCommand).RaiseCanExecuteChanged();
                        (OnUnFreezeProject as DelegateCommand).RaiseCanExecuteChanged();
                        (OnSetDeliveryTime as DelegateCommand).RaiseCanExecuteChanged();
                        (OnSetContractNumber as DelegateCommand).RaiseCanExecuteChanged();
                        (OnSetInvoiceCompletionTime as DelegateCommand).RaiseCanExecuteChanged();
                        (OnAddProduct as DelegateCommand).RaiseCanExecuteChanged();
                        (OnAddProjectResp as DelegateCommand).RaiseCanExecuteChanged();
                        (OnLinkProjectResp as DelegateCommand).RaiseCanExecuteChanged();
                        (OnLinkFile as DelegateCommand).RaiseCanExecuteChanged();
                        (OnUpdateFile as DelegateCommand).RaiseCanExecuteChanged();
                        (OnAddImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                        (OnImportImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                        (OnAddImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                        (OnImportImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();

                        using (this.productView.DeferRefresh())
                        {
                            this.productView.MoveToFirstPage();
                        }
                        using (this.projectResponsibleView.DeferRefresh())
                        {
                            this.projectResponsibleView.MoveToFirstPage();
                        }
                        using (this.importantPartView.DeferRefresh())
                        {
                            this.importantPartView.MoveToFirstPage();
                        }
                        using (this.importantPartRejesterView.DeferRefresh())
                        {
                            this.importantPartRejesterView.MoveToFirstPage();
                        }

                        LoadProjectFiles();
                        UpdateChanged("CollectionProductView");
                        UpdateChanged("CollectionProjectResponsibleView");
                    }
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

        public ICollectionView CollectionProjectView
        {
            get { return this.projectView; }
        }

        public ICollectionView CollectionProductView
        {
            get { return this.productView; }
        }

        public ICollectionView CollectionProjectResponsibleView
        {
            get { return this.projectResponsibleView; }
        }

        public ICollectionView CollectionImportantPartView
        {
            get { return this.importantPartView; }
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
                }
            }
        }

        public ObservableCollection<String> FilterList { get; set; }
        public ObservableCollection<String> FilterType { get; set; }

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
                }
            }
        }

        private String selectFiltersType;
        public String SelectFiltersType
        {
            get
            {
                return selectFiltersType;
            }
            set
            {
                if (selectFiltersType != value)
                {
                    selectFiltersType = value;
                    UpdateChanged("SelectFiltersType");
                }
            }
        }

        private bool isUserProject;
        public bool IsUserProject
        {
            get
            {
                return isUserProject;
            }
            set
            {
                if (isUserProject != value)
                {
                    isUserProject = value;
                    UpdateChanged("IsUserProject");
                }
            }
        }

        public ICommand OnExport { get; private set; }
        public ICommand OnPrint { get; private set; }
        public ICommand OnRefash { get; private set; }
        public ICommand OnDeleteProject { get; private set; }
        public ICommand OnFreezeProject { get; private set; }
        public ICommand OnUnFreezeProject { get; private set; }
        public ICommand OnModifyProductMan { get; private set; }
        public ICommand OnProductPart { get; private set; }
        public ICommand DoubleClickProject { get; private set; }
        public ICommand DoubleClickProduct { get; private set; }
        public ICommand DoubleClickProjectResponsible { get; private set; }
        public ICommand OnSave { get; private set; }
        public ICommand OnModifyProject { get; private set; }
        public ICommand OnSetDeliveryTime { get; private set; }
        public ICommand OnSetContractNumber  { get; private set; }
        public ICommand OnSetInvoiceCompletionTime { get; private set; }
        public ICommand OnAdd { get; private set; }
        public ICommand OnViewProject { get; private set; }
        public ICommand OnAddProduct { get; private set; }
        public ICommand OnModifyProduct { get; private set; }
        public ICommand OnDeleteProduct { get; private set; }

        public ICommand OnProductSetOutPutNumber { get; private set; }
        public ICommand OnAddProjectResp { get; private set; }
        public ICommand OnModifyProjectResp { get; private set; }
        public ICommand OnLinkProjectResp { get; private set; }

        private ProjectEntity AddProjectEntity;
        private ProductEntity AddProductEntity;
        private ProjectResponsibleEntity AddProjectResponsibleEntity;

        private ProductEntity selectProductEntity;
        public ProductEntity SelectProductEntity
        {
            get
            {
                return selectProductEntity;
            }
            set
            {
                if (selectProductEntity != value)
                {
                    selectProductEntity = value;
                    if (selectProductEntity != null)
                    {
                        selectProductEntity.Update();
                    }
                    UpdateChanged("SelectProductEntity");
                    (OnModifyProduct as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteProduct as DelegateCommand).RaiseCanExecuteChanged();
                    (OnModifyProductMan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnProductPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnProductSetOutPutNumber as DelegateCommand).RaiseCanExecuteChanged();
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
                    if (selectProjectResponsibleEntity != null)
                    {
                        selectProjectResponsibleEntity.Update();
                    }
                    UpdateChanged("SelectProjectResponsibleEntity");
                    (OnModifyProjectResp as DelegateCommand).RaiseCanExecuteChanged();
                }
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
                    (OnModifyFile as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteFinalFile as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnLinkFile { get; private set; }
        public ICommand OnModifyFile { get; private set; }
        public ICommand OnDeleteFile { get; private set; }
        public ICommand OnViewDeleteFile { get; private set; }
        public ICommand OnDeleteFinalFile { get; private set; }

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

        public String RecorderCount
        {
            get
            {
                if (projectView != null)
                {
                    return "总共 " + UserProjectCount.ToString() + " 个项目";
                }
                else
                {
                    return "";
                }
            }
        }

        public ImportantPartEntity AddImportantPartEntity { get; set; }

        public ICommand OnAddImportantPart { get; private set; }
        public ICommand OnImportImportantPart { get; private set; }
        public ICommand OnArivePart { get; private set; }
        public ICommand OnOutPart { get; private set; }
        public ICommand OnModifyImportantPart { get; private set; }
        public ICommand OnViewImportantPart { get; private set; }
        public ICommand OnDeleteImportantPart { get; private set; }
        public ICommand OnFinalDeleteImportantPart { get; private set; }

        private ImportantPartEntity selectImportantPartEntity;
        public ImportantPartEntity SelectImportantPartEntity
        {
            get
            {
                return selectImportantPartEntity;
            }
            set
            {
                if (selectImportantPartEntity != value)
                {
                    selectImportantPartEntity = value;
                    if (selectImportantPartEntity != null)
                    {
                        selectImportantPartEntity.Update();
                    }
                    UpdateChanged("SelectImportantPartEntity");
                    (OnArivePart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnOutPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnModifyImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                    (OnFinalDeleteImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ImportantPartRejesterEntity AddImportantPartRejesterEntity { get; set; }

        public ICommand OnAddImportantPartRejester { get; private set; }
        public ICommand OnImportImportantPartRejester { get; private set; }
        public ICommand OnViewImportantPartRejester { get; private set; }
        public ICommand OnModifyImportantPartRejester { get; private set; }
        public ICommand OnDeleteImportantPartRejester { get; private set; }

        private ImportantPartRejesterEntity selectImportantPartRejesterEntity;
        public ImportantPartRejesterEntity SelectImportantPartRejesterEntity
        {
            get
            {
                return selectImportantPartRejesterEntity;
            }
            set
            {
                if (selectImportantPartRejesterEntity != value)
                {
                    selectImportantPartRejesterEntity = value;
                    if (selectImportantPartRejesterEntity != null)
                    {
                        selectImportantPartRejesterEntity.Update();
                    }
                    UpdateChanged("SelectImportantPartRejesterEntity");
                    (OnViewImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                    (OnModifyImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeleteImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OnImport { get; private set; }

        public ICommand OnImportProduct { get; private set; }

        public ProductManagersViewModel()
        {
            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            ProductEntityList = new ObservableCollection<ProductEntity>();
            ProjectResponsibleEntityList = new ObservableCollection<ProjectResponsibleEntity>();
            DepartmemtList = new ObservableCollection<DepartmentEntity>();
            DepartmentDictionary = new Dictionary<int, DepartmentEntity>();
            ProjectEntityDictionary = new Dictionary<String, ProjectEntity>();
            FileTypeEntityDictionary = new Dictionary<int, FileTypeEntity>();
            FileTypeEntityList = new ObservableCollection<FileTypeEntity>();
            ProjectFilesEntityList = new ObservableCollection<ProjectFilesEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            UserEntityDictionaryName = new Dictionary<String, UserEntity>();
            ProductTypeEntityDictionary = new Dictionary<int, ProductTypeEntity>();
            ProductTypeEntityList = new ObservableCollection<ProductTypeEntity>();
            ProductEntityDictionary = new Dictionary<string, ProductEntity>();
            ImportantPartEntityList = new ObservableCollection<ImportantPartEntity>();
            ProductPartTypeDictionary = new Dictionary<int, ProductPartTypeEntity>();
            UserProjectEntityDictionary = new Dictionary<string, UserProjectEntity>();
            ImportantPartRejesterEntityList = new ObservableCollection<ImportantPartRejesterEntity>();

            OnLinkFile = new DelegateCommand(OnLinkFileCommand, CanUpdateFileCommand);
            OnUpdateFile = new DelegateCommand(OnUpdateFileCommand, CanUpdateFileCommand);
            OnModifyFile = new DelegateCommand(OnModifyFileCommand, CanModifyFileCommand);
            OnDeleteFile = new DelegateCommand(OnDeleteFileCommand, CanDeleteFileCommand);
            OnDeleteFinalFile = new DelegateCommand(OnDeleteFinalFileCommand, CanDeleteFinalFileCommand);
            OnViewDeleteFile = new DelegateCommand(OnViewDeleteFileCommand, CanViewDeleteFileCommand);

            OnSave = new DelegateCommand(OnSaveCommand, CanSaveCommand);

            OnModifyProject = new DelegateCommand(OnModifyProjectCommand, CanModifyProjectCommand);
            OnViewProject = new DelegateCommand(OnViewProjectCommand, CanViewProjectCommand);
            OnAdd = new DelegateCommand(OnAddCommand, CanAddCommand);
            OnDeleteProject = new DelegateCommand(OnDeleteProjectCommand, CanDeleteProjectCommand);
            OnFreezeProject = new DelegateCommand(OnFreezeProjectCommand, CanFreezeProjectCommand);
            OnUnFreezeProject = new DelegateCommand(OnUnFreezeProjectCommand, CanUnFreezeProjectCommand);
            OnSetDeliveryTime = new DelegateCommand(OnSetDeliveryTimeCommand, CanSetDeliveryTimeCommand);
            OnSetContractNumber = new DelegateCommand(OnSetContractNumberCommand, CanSetContractNumberCommand);
            OnSetInvoiceCompletionTime = new DelegateCommand(OnSetInvoiceCompletionTimeCommand, CanSetInvoiceCompletionTimeCommand);


            OnAddProduct = new DelegateCommand(OnAddProductCommand, CanAddProductCommand);
            OnModifyProduct = new DelegateCommand(OnModifyProductCommand, CanModifyProduct);
            OnDeleteProduct = new DelegateCommand(OnDeleteProductCommand, CanDeleteProductCommand);
            OnProductSetOutPutNumber = new DelegateCommand(OnProductSetOutPutNumberCommand, CanProductSetOutPutNumberCommand);

            OnAddProjectResp = new DelegateCommand(OnAddProjectRespCommand, CanAddProjectRespCommand);
            OnModifyProjectResp = new DelegateCommand(OnModifyProjectRespCommand, CanModifyProjectRespCommand);
            OnLinkProjectResp = new DelegateCommand(OnLinkProjectRespCommand, CanLinkProjectRespCommand);

            OnModifyProductMan = new DelegateCommand(OnModifyProductManCommand, CanModifyProductManCommand);

            OnProductPart = new DelegateCommand(OnProductPartCommand, CanProductPartCommand);

            OnRefash = new DelegateCommand(OnRefashCommand);

            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);

            DoubleClickProduct = new DelegateCommand(DoubleClickProductCommand);

            DoubleClickProjectResponsible = new DelegateCommand(DoubleClickProjectResponsibleCommand);

            OnAddImportantPart = new DelegateCommand(OnAddImportantPartCommand, CanAddImportantPartCommand);
            OnImportImportantPart = new DelegateCommand(OnImportImportantPartCommand, CanImportImportantPartCommand);
            OnArivePart = new DelegateCommand(OnArivePartCommand, CanArivePartCommand);
            OnOutPart = new DelegateCommand(OnOutPartCommand, CanOnOutPartCommand);
            OnModifyImportantPart = new DelegateCommand(OnModifyImportantPartCommand, CanModifyImportantPartCommand);
            OnViewImportantPart = new DelegateCommand(OnViewImportantPartCommand, CanViewImportantPartCommand);
            OnDeleteImportantPart = new DelegateCommand(OnDeleteImportantPartCommand, CanDeleteImportantPartCommand);
            OnFinalDeleteImportantPart = new DelegateCommand(OnFinalDeleteImportantPartCommand, CanFinalDeleteImportantPartCommand);

            OnAddImportantPartRejester = new DelegateCommand(OnAddImportantPartRejesterCommand, CanAddImportantPartRejesterCommand);
            OnImportImportantPartRejester = new DelegateCommand(OnImportImportantPartRejesterCommand, CanImportImportantPartRejesterCommand);
            OnViewImportantPartRejester = new DelegateCommand(OnViewImportantPartRejesterCommand, CanViewImportantPartRejesterCommand);
            OnModifyImportantPartRejester = new DelegateCommand(OnModifyImportantPartRejesterCommand, CanModifyImportantPartRejesterCommand);
            OnDeleteImportantPartRejester = new DelegateCommand(OnDeleteImportantPartRejesterCommand, CanDeleteImportantPartRejesterCommand);


            OnExport = new DelegateCommand(OnExportCommand);
            OnPrint = new DelegateCommand(OnPrintCommand);
            OnImport = new DelegateCommand(OnImportCommand, CanImportCommand);
            OnImportProduct = new DelegateCommand(OnImportProductCommand, CanImportProductCommand);


            FilterList = new ObservableCollection<string>();
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";

            FilterType = new ObservableCollection<string>();
            FilterType.Add("包含");
            FilterType.Add("不包含");
            selectFiltersType = "包含";
        }

        public void LoadData()
        {
            IsBusy = true;
            SystemManageDomainContext = new SystemManageDomainContext();//.RejectChanges();
            ProductDomainContext = new ProductDomainContext();//.RejectChanges();
            ProductDomainContextForFile = new ProductDomainContext();//.RejectChanges();
            ProductDomainContext.PropertyChanged -= ProductDomainContext_PropertyChanged;
            ProductDomainContext.PropertyChanged += ProductDomainContext_PropertyChanged;
            selectProjectEntity = null;
            projectView = null;
            productView = null;
            projectResponsibleView = null;
            ProductEntityList.Clear();
            ProjectResponsibleEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                SystemManageDomainContext.Load<ProductManager.Web.Model.department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            DepartmemtList.Clear();
            DepartmentDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmemtList.Add(departmentEntity);
                DepartmentDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
            }
            DepartmentEntity departmentEntityZero = new DepartmentEntity();
            departmentEntityZero.DepartmentID = 0;
            departmentEntityZero.DepartmentName = "请选择部门";
            DepartmemtList.Add(departmentEntityZero);
            UpdateChanged("DepartmemtList");

            LoadOperation<ProductManager.Web.Model.project> loadOperationProject =
                ProductDomainContext.Load<ProductManager.Web.Model.project>(ProductDomainContext.GetProjectQuery());
            loadOperationProject.Completed += loadOperationProject_Completed;
        }

        private void loadOperationProject_Completed(object sender, EventArgs e)
        {
            ProjectEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.project project in loadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();
                ProjectEntityDictionary.Add(projectEntity.ManufactureNumber, projectEntity);
            }

            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                SystemManageDomainContext.Load<ProductManager.Web.Model.user>(SystemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            UserEntityDictionaryName.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                UserEntityDictionary.Add(userEntity.UserID, userEntity);
                UserEntityDictionaryName.Add(userEntity.CUserName, userEntity);
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

            LoadOperation<ProductManager.Web.Model.product_type> loadOperationProductType =
                SystemManageDomainContext.Load<ProductManager.Web.Model.product_type>(SystemManageDomainContext.GetProduct_typeQuery());
            loadOperationProductType.Completed += loadOperationProductType_Completed;

        }

        void loadOperationProductType_Completed(object sender, EventArgs e)
        {
            ProductTypeEntityDictionary.Clear();
            ProductTypeEntityList.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product_type product_type in loadOperation.Entities)
            {
                ProductTypeEntity productTypeEntity = new ProductTypeEntity();
                productTypeEntity.ProductType = product_type;
                productTypeEntity.Update();
                ProductTypeEntityDictionary.Add(productTypeEntity.ProductTypeID, productTypeEntity);
                ProductTypeEntityList.Add(productTypeEntity);
            }

            LoadOperation<ProductManager.Web.Model.product> loadOperationProduct =
               ProductDomainContext.Load<ProductManager.Web.Model.product>(ProductDomainContext.GetProductQuery());
            loadOperationProduct.Completed += loadOperationProductDictionary_Completed;

        }

        void loadOperationProductDictionary_Completed(object sender, EventArgs e)
        {
            ProductEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product product in loadOperation.Entities)
            {
                ProductEntity productEntity = new ProductEntity();
                productEntity.Product = product;
                productEntity.Update();
                ProductEntityDictionary.Add(productEntity.ProductID, productEntity);
            }


            LoadOperation<ProductManager.Web.Model.product_part_type> loadOperationProductPartType =
               SystemManageDomainContext.Load<ProductManager.Web.Model.product_part_type>(SystemManageDomainContext.GetProduct_part_typeQuery());
            loadOperationProductPartType.Completed += loadOperationPartTimeType_Completed;
        }

        void loadOperationPartTimeType_Completed(object sender, EventArgs e)
        {
            ProductPartTypeDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.product_part_type product_part_type in loadOperation.Entities)
            {
                ProductPartTypeEntity productPartTypeEntity = new ProductPartTypeEntity();
                productPartTypeEntity.ProductPartType = product_part_type;
                productPartTypeEntity.Update();
                ProductPartTypeDictionary.Add(productPartTypeEntity.ProductPartTypeID, productPartTypeEntity);
            }

            App app = Application.Current as App;
            LoadOperation<ProductManager.Web.Model.user_project> loadOperationUserProject =
               ProductDomainContext.Load<ProductManager.Web.Model.user_project>(ProductDomainContext.GetUserProjectQuery(app.UserInfo.UserID));
            loadOperationUserProject.Completed += LoadUserProjectComplete;
        }

        void LoadUserProjectComplete(object sender, EventArgs e)
        {
            UserProjectEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user_project user_project in loadOperation.Entities)
            {
                UserProjectEntity lUserProjectEntity = new UserProjectEntity();
                lUserProjectEntity.UserProject = user_project;
                lUserProjectEntity.Update();
                UserProjectEntity lUserProjectEntityTemp;
                if (!UserProjectEntityDictionary.TryGetValue(lUserProjectEntity.ManufactureNumber, out lUserProjectEntityTemp))
                {
                    UserProjectEntityDictionary.Add(lUserProjectEntity.ManufactureNumber, lUserProjectEntity);
                }

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

            this.productSource = new EntityList<ProductManager.Web.Model.product>(this.ProductDomainContext.products);
            this.productLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.product>(
                this.LoadProductEntities,
                this.LoadOperationProductCompleted
                );
            this.productView = new DomainCollectionView<ProductManager.Web.Model.product>(this.productLoader, this.productSource);

            this.projectResponsibleSource = new EntityList<ProductManager.Web.Model.project_responsible>(this.ProductDomainContext.project_responsibles);
            this.projectResponsibleLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible>(
                this.LoadProjectResponsibleEntities,
                this.LoadOperationProjectResponsibleCompleted);
            this.projectResponsibleView = new DomainCollectionView<ProductManager.Web.Model.project_responsible>(this.projectResponsibleLoader, this.projectResponsibleSource);

            this.importantPartSource = new EntityList<ProductManager.Web.Model.important_part>(this.ProductDomainContext.important_parts);
            this.importantPartLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.important_part>(
                this.LoadImportantPartEntities,
                this.LoadImportantPartCompleted);
            this.importantPartView = new DomainCollectionView<ProductManager.Web.Model.important_part>(this.importantPartLoader, this.importantPartSource);

            this.importantPartRejesterSource = new EntityList<ProductManager.Web.Model.important_part_rejester>(this.ProductDomainContext.important_part_rejesters);
            this.importantPartRejesterLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.important_part_rejester>(
                this.LoadImportantPartRejesterEntities,
                this.LoadImportantPartRejesterCompleted
                );
            this.importantPartRejesterView = new DomainCollectionView<ProductManager.Web.Model.important_part_rejester>(this.importantPartRejesterLoader, this.importantPartRejesterSource);
            UpdateChanged("CollectionProjectView");
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.important_part_rejester> LoadImportantPartRejesterEntities()
        {
            if (SelectProjectEntity != null)
            {
                IsBusy = true;
                EntityQuery<ProductManager.Web.Model.important_part_rejester> lQuery = this.ProductDomainContext.GetImportantPartRejesterQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.importantPartRejesterView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.important_part_rejester> lQuery = this.ProductDomainContext.GetImportantPartRejesterQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.importantPartRejesterView));
            }
        }

        private void LoadImportantPartRejesterCompleted(LoadOperation<ProductManager.Web.Model.important_part_rejester> aLoadOperation)
        {
            ImportantPartRejesterEntityList.Clear();
            foreach (ProductManager.Web.Model.important_part_rejester important_part_rejester in aLoadOperation.Entities)
            {
                ImportantPartRejesterEntity importantPartRejesterEntity = new ImportantPartRejesterEntity();
                importantPartRejesterEntity.ImportantPartRejester = important_part_rejester;
                importantPartRejesterEntity.UserEntityDictionary = UserEntityDictionary;
                importantPartRejesterEntity.Update();
                ImportantPartRejesterEntityList.Add(importantPartRejesterEntity);
            }
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.important_part> LoadImportantPartEntities()
        {
            if (SelectProjectEntity != null)
            {
                IsBusy = true;
                EntityQuery<ProductManager.Web.Model.important_part> lQuery = this.ProductDomainContext.GetImportant_partQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.importantPartView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.important_part> lQuery = this.ProductDomainContext.GetImportant_partQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.importantPartView));
            }
        }

        private void LoadImportantPartCompleted(LoadOperation<ProductManager.Web.Model.important_part> aLoadOperation)
        {
            ImportantPartEntityList.Clear();
            foreach (ProductManager.Web.Model.important_part important_part in aLoadOperation.Entities)
            {
                ImportantPartEntity importantPartEntity = new ImportantPartEntity();
                importantPartEntity.ImportantPart = important_part;
                importantPartEntity.UserEntityDictionary = UserEntityDictionary;
                importantPartEntity.Update();
                ImportantPartEntityList.Add(importantPartEntity);
            }
            IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project_responsible> LoadProjectResponsibleEntities()
        {
            if (SelectProjectEntity != null)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project_responsible> lQuery = this.ProductDomainContext.GetProject_responsibleQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectResponsibleView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project_responsible> lQuery = this.ProductDomainContext.GetProject_responsibleQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectResponsibleView));
            }
        }

        private void LoadOperationProjectResponsibleCompleted(LoadOperation<ProductManager.Web.Model.project_responsible> aLoadOperation)
        {
            ProjectResponsibleEntityList.Clear();
            foreach (ProductManager.Web.Model.project_responsible project_responsible in aLoadOperation.Entities)
            {
                ProjectResponsibleEntity projectResponsibleEntity = new ProjectResponsibleEntity();
                projectResponsibleEntity.ProjectResponsible = project_responsible;
                projectResponsibleEntity.Update();
                DepartmentEntity departmentEntity;
                if (DepartmentDictionary.TryGetValue(projectResponsibleEntity.DepartmentID, out departmentEntity))
                {
                    projectResponsibleEntity.DepartmentName = departmentEntity.DepartmentName;
                }
                UserEntity userEntity;
                if (projectResponsibleEntity.ResponsiblePersionName != null)
                {
                    if (UserEntityDictionaryName.TryGetValue(projectResponsibleEntity.ResponsiblePersionName, out userEntity))
                    {
                        projectResponsibleEntity.UserPhoneNumber = userEntity.UserPhoneNumber;
                    }
                }
                ProjectResponsibleEntityList.Add(projectResponsibleEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectResponsibleView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectResponsibleEntityList");
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
                        if (selectFiltersType == "包含")
                        {
                            lQuery = lQuery.Where(e => e.manufacture_number.ToLower().Contains(filterContent.ToLower()));
                        }
                        else
                        {
                            lQuery = lQuery.Where(e => !e.manufacture_number.ToLower().Contains(filterContent.ToLower()));
                        }
                    }
                }
                if (selectFilerList == "项目名称")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        if (selectFiltersType == "包含")
                        {
                            lQuery = lQuery.Where(e => e.project_name.ToLower().Contains(filterContent.ToLower()));
                        }
                        else
                        {
                            lQuery = lQuery.Where(e => !e.project_name.ToLower().Contains(filterContent.ToLower()));
                        }
                    }
                }
                if (selectFilerList == "备注")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        if (selectFiltersType == "包含")
                        {
                            lQuery = lQuery.Where(e => e.remark.ToLower().Contains(filterContent.ToLower()));
                        }
                        else
                        {
                            lQuery = lQuery.Where(e => !e.remark.ToLower().Contains(filterContent.ToLower()));
                        }
                    }
                }
                if (selectFilerList == "年份")
                {
                    if (!String.IsNullOrEmpty(filterContent))
                    {
                        try
                        {
                            int year = Convert.ToInt32(filterContent);
                            if (selectFiltersType == "包含")
                            {
                                lQuery = lQuery.Where(e => e.year_number == year);
                            }
                            else
                            {
                                lQuery = lQuery.Where(e => e.year_number != year);
                            }
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
                            if (selectFiltersType == "包含")
                            {
                                lQuery = lQuery.Where(e => e.record_date.Value.Year == remarkDate.Year && e.record_date.Value.Month == remarkDate.Month && e.record_date.Value.Day == remarkDate.Day);
                            }
                            else
                            {
                                lQuery = lQuery.Where(e => e.record_date.Value.Year != remarkDate.Year || e.record_date.Value.Month != remarkDate.Month || e.record_date.Value.Day != remarkDate.Day);
                            }
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
            UserProjectCount = 0;
            foreach (ProductManager.Web.Model.project project in aLoadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.ProjectEntityDictionary = ProjectEntityDictionary;
                projectEntity.Update();

                UserProjectEntity lUserProjectEntity;
                if (UserProjectEntityDictionary.TryGetValue(projectEntity.ManufactureNumber, out lUserProjectEntity))
                {
                    projectEntity.UserProjectEntity = lUserProjectEntity;
                    projectEntity.SetIsUserProject(true);
                }

                projectEntity.UserProjectEntityDictionary = UserProjectEntityDictionary;
                projectEntity.ProductManagersViewModel = this;
                projectEntity.ProductDomainContext = ProductDomainContext;
                if (IsUserProject && !projectEntity.IsUserProject)
                {
                    continue;
                }
                ProjectEntityList.Add(projectEntity);
                UserProjectCount++;
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectEntityList");
            UpdateChanged("RecorderCount");
            this.IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.product> LoadProductEntities()
        {
            if (SelectProjectEntity != null)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.product> lQuery = this.ProductDomainContext.GetProductQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.product> lQuery = this.ProductDomainContext.GetProductQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productView));
            }
        }

        private void LoadOperationProductCompleted(LoadOperation<ProductManager.Web.Model.product> aLoadOperation)
        {
            ProductEntityList.Clear();
            foreach (ProductManager.Web.Model.product product in aLoadOperation.Entities)
            {
                ProductEntity productEntity = new ProductEntity();
                productEntity.Product = product;
                productEntity.ProjectEntityDictionary = ProjectEntityDictionary;
                productEntity.ProductPartTypeEntityDictionary = ProductPartTypeDictionary;
                productEntity.Update();

                productEntity.ProductEntityDictionary = ProductEntityDictionary;
                ProductTypeEntity lProductTypeEntity;
                if (ProductTypeEntityDictionary.TryGetValue(productEntity.ProductTypeID, out lProductTypeEntity))
                {
                    productEntity.ProductTypeString = lProductTypeEntity.ProductTypeName;
                }

                ProductEntityList.Add(productEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.productView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProductEntityList");
            this.IsBusy = false;
        }

        private void OnRefashCommand()
        {
            selectProjectEntity = null;
            ProductEntityList.Clear();
            ProjectResponsibleEntityList.Clear();
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }

        private void OnDeleteProjectCommand()
        {
            if (SelectProjectEntity != null)
            {
                DeleteProjectWindow deleteProjectWindow = new DeleteProjectWindow(SelectProjectEntity);
                deleteProjectWindow.Closed += deleteProjectWindow_Closed;
                deleteProjectWindow.Show();
            }
        }

        void deleteProjectWindow_Closed(object sender, EventArgs e)
        {
            DeleteProjectWindow deleteProjectWindow = sender as DeleteProjectWindow;
            if (deleteProjectWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanDeleteProjectCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010100);
            }
            else
            {
                return false;
            }
        }

        private void OnFreezeProjectCommand()
        {
            if (SelectProjectEntity != null)
            {
                FreezeProjectWindow freezeProjectWindow = new FreezeProjectWindow(SelectProjectEntity);
                freezeProjectWindow.Closed += freezeProjectWindow_Closed;
                freezeProjectWindow.Show();
            }
        }

        void freezeProjectWindow_Closed(object sender, EventArgs e)
        {
            FreezeProjectWindow freezeProjectWindow = sender as FreezeProjectWindow;
            if (freezeProjectWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanFreezeProjectCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010100);
            }
            else
            {
                return false;
            }
        }

        private void OnUnFreezeProjectCommand()
        {
            if (SelectProjectEntity != null)
            {
                SelectProjectEntity.IsFreeze = false;
                SelectProjectEntity.DUpdate();
                SelectProjectEntity.RaisALL();
                OnSaveCommand();
            }
        }

        private bool CanUnFreezeProjectCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010100);
            }
            else
            {
                return false;
            }
        }

        private void OnSetDeliveryTimeCommand()
        {
            SetDeliveryTimeWindow lSetDeliveryTimeWindow = new SetDeliveryTimeWindow(SelectProjectEntity);
            lSetDeliveryTimeWindow.Closed += lSetDeliveryTimeWindow_Closed;
            lSetDeliveryTimeWindow.Show();
        }

        void lSetDeliveryTimeWindow_Closed(object sender, EventArgs e)
        {
            SetDeliveryTimeWindow lSetDeliveryTimeWindow = sender as SetDeliveryTimeWindow;
            if (lSetDeliveryTimeWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanSetDeliveryTimeCommand(Object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010900);
            }
            else
            {
                return false;
            }
        }

        private void OnSetContractNumberCommand()
        {
            SetContractNumberWindow lSetContractNumberWindow = new SetContractNumberWindow(SelectProjectEntity);
            lSetContractNumberWindow.Closed += lSetContractNumberWindow_Closed;
            lSetContractNumberWindow.Show();
        }

        void lSetContractNumberWindow_Closed(object sender, EventArgs e)
        {
            SetContractNumberWindow lSetContractNumberWindow = sender as SetContractNumberWindow;
            if(lSetContractNumberWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanSetContractNumberCommand(Object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2011000);
            }
            else
            {
                return false;
            }
        }

        private void OnSetInvoiceCompletionTimeCommand()
        {
            InvoiceCompletionTimeWindow lInvoiceCompletionTimeWindow = new InvoiceCompletionTimeWindow(SelectProjectEntity);
            lInvoiceCompletionTimeWindow.Closed += lInvoiceCompletionTimeWindow_Closed;
            lInvoiceCompletionTimeWindow.Show();
        }

        void lInvoiceCompletionTimeWindow_Closed(object sender, EventArgs e)
        {
            InvoiceCompletionTimeWindow lInvoiceCompletionTimeWindow = sender as InvoiceCompletionTimeWindow;
            if (lInvoiceCompletionTimeWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanSetInvoiceCompletionTimeCommand(Object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2011100);
            }
            else
            {
                return false;
            }
        }

        private void OnModifyProductManCommand()
        {
            ChangeProductManufactureNumberWindow changeProductManufactureNumberWindow = new ChangeProductManufactureNumberWindow(SelectProductEntity);
            changeProductManufactureNumberWindow.Closed += changeProductManufactureNumberWindow_Closed;
            changeProductManufactureNumberWindow.Show();
        }

        private void changeProductManufactureNumberWindow_Closed(object sender, EventArgs e)
        {
            ChangeProductManufactureNumberWindow changeProductManufactureNumberWindow = sender as ChangeProductManufactureNumberWindow;
            if (changeProductManufactureNumberWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyProductManCommand(object aObject)
        {
            if (SelectProductEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnProductPartCommand()
        {
            ProductPartTimeWindow productPartTimeWindow = new ProductPartTimeWindow(SelectProductEntity);
            productPartTimeWindow.Show();
        }

        private bool CanProductPartCommand(object aObject)
        {
            if (SelectProductEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoubleClickProjectCommand()
        {
            ShowExpander = true;
            /*if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete)
            {
                ProjectWindow projectWindow = new ProjectWindow(ProjectWindowType.Modify, SelectProjectEntity);
                projectWindow.Show();
            }*/
        }

        private void DoubleClickProductCommand()
        {
            if (SelectProductEntity != null)
            {
                ProductWindow productWindow = new ProductWindow(ProductWindowType.Modify, ProductTypeEntityList, SelectProductEntity);
                productWindow.Show();
            }
        }

        private void DoubleClickProjectResponsibleCommand()
        {
            if (SelectProjectResponsibleEntity != null)
            {
                ProjectResponsibleWindow projectResponsibleWindow = new ProjectResponsibleWindow(DepartmemtList, SelectProjectResponsibleEntity);
                projectResponsibleWindow.Show();
            }
        }

        void ProductDomainContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnSaveCommand()
        {
            SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
            subOperation.Completed += subOperation_Completed;
        }

        private bool CanSaveCommand(object aObject)
        {
            return ProductDomainContext.HasChanges;
        }

        void subOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("成功", "成功！");
                notifyWindow.Show();
                //LoadData();
            }

            (OnSave as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        private void OnViewProjectCommand()
        {
            ProjectWindow projectWindow = new ProjectWindow(ProjectWindowType.View, SelectProjectEntity);
            projectWindow.Closed += projectWindow_Closed;
            projectWindow.Show();
        }

        private bool CanViewProjectCommand(object aObject)
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

        private void OnAddCommand()
        {
            AddProjectEntity = new ProjectEntity();
            AddProjectEntity.RecordDate = DateTime.Now;
            AddProjectEntity.YearNumber = DateTime.Now.Year;
            AddProjectEntity.ProjectEntityDictionary = ProjectEntityDictionary;
            AddProjectEntity.Project = new ProductManager.Web.Model.project();
            ProjectWindow projectWindow = new ProjectWindow(ProjectWindowType.ADD, AddProjectEntity);
            projectWindow.Closed += AddProjectClosed;
            projectWindow.Show();
        }

        private bool CanAddCommand(object aObject)
        {
            App app = Application.Current as App;
            return app.UserInfo.GetUerRight(2010100);
        }

        private void OnModifyProjectCommand()
        {
            ProjectWindow projectWindow = new ProjectWindow(ProjectWindowType.Modify, SelectProjectEntity);
            projectWindow.Closed += projectWindow_Closed;
            projectWindow.Show();
        }

        void projectWindow_Closed(object sender, EventArgs e)
        {
            ProjectWindow projectWindow = sender as ProjectWindow;
            if (projectWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyProjectCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010100);
            }
            else
            {
                return false;
            }
        }

        private void OnAddProductCommand()
        {
            AddProductEntity = new ProductEntity();
            AddProductEntity.ProjectEntityDictionary = ProjectEntityDictionary;
            AddProductEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddProductEntity.ProductIDCreateData = DateTime.Now;
            AddProductEntity.ProductTypeID = 1;
            AddProductEntity.Product = new ProductManager.Web.Model.product();
            AddProductEntity.ProductEntityDictionary = ProductEntityDictionary;
            ProductWindow productWindow = new ProductWindow(ProductWindowType.ADD, ProductTypeEntityList, AddProductEntity);
            productWindow.Closed += AddProductClosed;
            productWindow.Show();
        }

        private void AddProductClosed(object sender, EventArgs e)
        {
            ProductWindow productWindow = sender as ProductWindow;
            if (productWindow.DialogResult == true)
            {
                ProductEntityList.Add(AddProductEntity);
                ProductDomainContext.products.Add(AddProductEntity.Product);
                ProductEntityDictionary.Add(AddProductEntity.ProductID, AddProductEntity);
                OnSaveCommand();
            }
        }

        private bool CanAddProductCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2011200);
            }
            else
            {
                return false;
            }
        }

        private void OnModifyProductCommand()
        {
            ProductWindow productWindow = new ProductWindow(ProductWindowType.Modify, ProductTypeEntityList, SelectProductEntity);
            productWindow.Closed += productWindow_Closed;
            productWindow.Show();
        }

        void productWindow_Closed(object sender, EventArgs e)
        {
            ProductWindow productWindow = sender as ProductWindow;
            if (productWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyProduct(object aObject)
        {
            if (SelectProductEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010700);
            }
            else
            {
                return false;
            }
        }

        private void OnDeleteProductCommand()
        {
            ConfirmWindow lConfirmWindow = new ConfirmWindow("删除产品", "是否删除产品：" + SelectProductEntity.ProductID);
            lConfirmWindow.Closed += lConfirmWindow_Closed;
            lConfirmWindow.Show();
        }

        void lConfirmWindow_Closed(object sender, EventArgs e)
        {
            ConfirmWindow confirmWindow = sender as ConfirmWindow;
            if (confirmWindow.DialogResult == true)
            {
                ProductDomainContext.products.Remove(SelectProductEntity.Product);
                SubmitOperation deleteProductOption = ProductDomainContext.SubmitChanges();
                deleteProductOption.Completed += deleteProductOption_Completed;
                //OnSaveCommand();
            }
        }

        void deleteProductOption_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("成功", "成功！");
                notifyWindow.Show();
                //LoadData();
                using (this.productView.DeferRefresh())
                {
                    this.productView.MoveToFirstPage();
                }
            }

        }

        private bool CanDeleteProductCommand(object aObject)
        {
            if (SelectProductEntity != null)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2011400);
            }
            else
            {
                return false;
            }
        }

        private void OnProductSetOutPutNumberCommand()
        {
            ProductWindow productWindow = new ProductWindow(ProductWindowType.SetOutPutNumber, ProductTypeEntityList, SelectProductEntity);
            productWindow.Closed += productWindow_Closed;
            productWindow.Show();
        }

        private bool CanProductSetOutPutNumberCommand(object aObject)
        {
            if (SelectProductEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010600);
            }
            else
            {
                return false;
            }
        }

        private void OnAddProjectRespCommand()
        {
            AddProjectResponsibleEntity = new ProjectResponsibleEntity();
            AddProjectResponsibleEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddProjectResponsibleEntity.ProjectResponsible = new ProductManager.Web.Model.project_responsible();
            ProjectResponsibleWindow projectResponsibleWindow = new ProjectResponsibleWindow(DepartmemtList, AddProjectResponsibleEntity);
            projectResponsibleWindow.Closed += AddProductResponsibleClosed;
            projectResponsibleWindow.Show();
        }

        private bool CanAddProjectRespCommand(object aObject)
        {
            App app = Application.Current as App;
            if (app.UserInfo.GetUerRight(2010500))
            {
                if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private void OnModifyProjectRespCommand()
        {
            ProjectResponsibleWindow projectResponsibleWindow = new ProjectResponsibleWindow(DepartmemtList, SelectProjectResponsibleEntity);
            projectResponsibleWindow.Closed += projectResponsibleWindow_Closed;
            projectResponsibleWindow.Show();
        }

        private void projectResponsibleWindow_Closed(object sender, EventArgs e)
        {
            ProjectResponsibleWindow projectResponsibleWindow = sender as ProjectResponsibleWindow;
            if (projectResponsibleWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanModifyProjectRespCommand(object aObject)
        {
            App app = Application.Current as App;
            if (app.UserInfo.GetUerRight(2010500))
            {
                if (SelectProjectResponsibleEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        private void OnLinkProjectRespCommand()
        {
            LinkResponsePersonWindow lLinkResponseWindow = new LinkResponsePersonWindow(ProductDomainContext, DepartmentDictionary, UserEntityDictionaryName);
            lLinkResponseWindow.Closed += lLinkResponseWindow_Closed;
            lLinkResponseWindow.Show();
        }

        void lLinkResponseWindow_Closed(object sender, EventArgs e)
        {
            LinkResponsePersonWindow lLinkResponseWindow = sender as LinkResponsePersonWindow;
            if (lLinkResponseWindow.DialogResult == true)
            {
                OnSaveCommand();
            }
        }

        private bool CanLinkProjectRespCommand(object aObject)
        {
            App app = Application.Current as App;
            if (app.UserInfo.GetUerRight(2010500))
            {
                if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void AddProductResponsibleClosed(object sender, EventArgs e)
        {
            ProjectResponsibleWindow projectResponsibleWindow = sender as ProjectResponsibleWindow;
            if (projectResponsibleWindow.DialogResult == true)
            {
                ProjectResponsibleEntityList.Add(AddProjectResponsibleEntity);
                ProductDomainContext.project_responsibles.Add(AddProjectResponsibleEntity.ProjectResponsible);
                OnSaveCommand();
            }
        }

        public void ConfirmLeave()
        {
            if (ProductDomainContext.HasChanges)
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
                ProductDomainContext.SubmitChanges();
            }
            else
            {
                ProductDomainContext.RejectChanges();
            }
        }

        void AddProjectClosed(object sender, EventArgs e)
        {
            ProjectWindow projectWindow = sender as ProjectWindow;
            if (projectWindow.DialogResult == true)
            {
                ProjectEntityList.Add(AddProjectEntity);
                ProjectEntityDictionary.Add(AddProjectEntity.ManufactureNumber, AddProjectEntity);
                ProductDomainContext.projects.Add(AddProjectEntity.Project);
                OnSaveCommand();
            }
        }

        private void OnLinkFileCommand()
        {
            //FilesDictionarys = new Dictionary<string, Dictionary<string, ProjectFilesEntity>>();

            //IsBusy = true;

            //ProductDomainContextForFile.
            //LoadOperation<ProductManager.Web.Model.project_files> loadOperationProjectFiles =
            //        ProductDomainContextForFile.Load<ProductManager.Web.Model.project_files>(ProductDomainContextForFile.GetProject_filesQuery());
            //loadOperationProjectFiles.Completed += loadOperationAllProjectFiles_Completed;
            //loadOperationProjectFiles.Completed += ShowLinkFileWindow;
//             LinkFileViewModel linkFileViewModel = new LinkFileViewModel(SelectProjectEntity.ManufactureNumber, 
//                                                                     ProjectEntityList, DepartmentDictionary,
//                                                                     UserEntityDictionary, FileTypeEntityDictionary);
//             LinkFileWindow linkFileWindow = new LinkFileWindow(linkFileViewModel);
//             linkFileWindow.Closed += linkFileWindow_Closed;
//             linkFileWindow.Show();

            LinkFileViewModel linkFileViewModel = new LinkFileViewModel(SelectProjectEntity.ManufactureNumber,
                                                                               ProjectEntityList, DepartmentDictionary,
                                                                               UserEntityDictionary, FileTypeEntityDictionary);
            LinkFileWindow linkFileWindow = new LinkFileWindow(linkFileViewModel);
            linkFileWindow.Closed += linkFileWindow_Closed;
            linkFileWindow.Show();
        }

        private Dictionary<String, Dictionary<String, ProjectFilesEntity>> FilesDictionarys;

        void loadOperationAllProjectFiles_Completed(object sender, EventArgs e)
        {
            ProjectFilesEntityList.Clear();
            LoadOperation<ProductManager.Web.Model.project_files> loadOperation = sender as LoadOperation<ProductManager.Web.Model.project_files>;
            foreach (ProductManager.Web.Model.project_files project_files in loadOperation.Entities)
            {
                ProjectFilesEntity projectFilesEntity = new ProjectFilesEntity();
                projectFilesEntity.ProjectFiles = project_files;
                projectFilesEntity.Update();

                Dictionary<String, ProjectFilesEntity> lTempDictionary;
                if (!FilesDictionarys.TryGetValue(projectFilesEntity.manufactureNumber, out lTempDictionary))
                {
                    lTempDictionary = new Dictionary<string, ProjectFilesEntity>();
                    FilesDictionarys.Add(projectFilesEntity.manufactureNumber, lTempDictionary);
                }

                if (lTempDictionary != null)
                {
                    ProjectFilesEntity lFileEntityTemp;
                    if (!lTempDictionary.TryGetValue(projectFilesEntity.FileName, out lFileEntityTemp))
                    {
                        lTempDictionary.Add(projectFilesEntity.FileName, projectFilesEntity);
                    }
                }

            }
            IsBusy = false;
        }

        void ShowLinkFileWindow(object sender, EventArgs e)
        {
//             LinkFileViewModel linkFileViewModel = new LinkFileViewModel(SelectProjectEntity.ManufactureNumber,
//                                                                                ProjectEntityList, DepartmentDictionary,
//                                                                                UserEntityDictionary, FileTypeEntityDictionary);
//             LinkFileWindow linkFileWindow = new LinkFileWindow(linkFileViewModel);
//             linkFileWindow.Closed += linkFileWindow_Closed;
//             linkFileWindow.Show();
        }

        bool IsDuplicate(String aManufactureNumber, ProjectFilesEntity aProjectFiles)
        {
            bool lRes = false;
            if (FilesDictionarys != null)
            {
                Dictionary<String, ProjectFilesEntity> lTempDictionary;
                if (FilesDictionarys.TryGetValue(aManufactureNumber, out lTempDictionary))
                {
                    ProjectFilesEntity lFileEntityTemp;
                    if (lTempDictionary.TryGetValue(aProjectFiles.FileName, out lFileEntityTemp))
                    {
                        lRes = true;
                    }
                }

            }
            return lRes;
        }

        void linkFileWindow_Closed(object sender, EventArgs e)
        {
            LinkFileWindow linkFileWindow = sender as LinkFileWindow;
            if (linkFileWindow.DialogResult == true)
            {
                App app = Application.Current as App;
                if (linkFileWindow.linkFileViewModel.IsLinkFile)
                {
                    string rManufactureNumber = String.IsNullOrEmpty(linkFileWindow.linkFileViewModel.SelectProjectFilesEntity.RManufactureNumber) ?
                                                    linkFileWindow.linkFileViewModel.SelectProjectFilesEntity.ManufactureNumber :
                                                    linkFileWindow.linkFileViewModel.SelectProjectFilesEntity.RManufactureNumber;
                    foreach (ProjectEntity projectEntity in linkFileWindow.linkFileViewModel.ProjectLinkEntityList)
                    {
                        foreach (ProjectFilesEntity lEntity in linkFileWindow.projectFilesDataGrid.SelectedItems)
                        {
                            if (rManufactureNumber == projectEntity.ManufactureNumber)
                            {
                                continue;
                            }

                            if (IsDuplicate(projectEntity.ManufactureNumber, lEntity))
                            {
                                continue;
                            }

                            AddProjectFilesEntity = new ProjectFilesEntity();
                            AddProjectFilesEntity.DepartmentID = app.UserInfo.DepartmentID;
                            AddProjectFilesEntity.FileDiscript = lEntity.FileDiscript;
                            AddProjectFilesEntity.FileName = lEntity.FileName;
                            AddProjectFilesEntity.FileType = lEntity.FileType;
                            AddProjectFilesEntity.ManufactureNumber = projectEntity.ManufactureNumber;
                            AddProjectFilesEntity.RManufactureNumber = rManufactureNumber;
                            AddProjectFilesEntity.UserID = app.UserInfo.UserID;
                            AddProjectFilesEntity.FileUpdateTime = lEntity.FileUpdateTime;
                            AddProjectFilesEntity.FileTypeID = lEntity.FileTypeID;
                            AddProjectFilesEntity.FileBytes = lEntity.FileBytes;

                            AddProjectFilesEntity.DepartmentName = lEntity.DepartmentName;
                            AddProjectFilesEntity.UserName = lEntity.UserName;
                            AddProjectFilesEntity.FileDeletePersionName = lEntity.FileDeletePersionName;
                            AddProjectFilesEntity.FileTypeName = lEntity.FileTypeName;

                            AddProjectFilesEntity.ProjectFiles = new ProductManager.Web.Model.project_files();
                            ProjectFilesEntityList.Add(AddProjectFilesEntity);
                            AddProjectFilesEntity.DUpdate();
                            ProductDomainContextForFile.project_files.Add(AddProjectFilesEntity.ProjectFiles);
                        }
                      
                        
                    }
                }
                else
                {
                    foreach (ProjectEntity projectEntity in linkFileWindow.linkFileViewModel.ProjectLinkEntityList)
                    {
                        foreach (ProjectFilesEntity entity in linkFileWindow.linkFileViewModel.ProjectFilesEntityList)
                        {
                            string rManufactureNumber = String.IsNullOrEmpty(entity.RManufactureNumber) ? entity.ManufactureNumber : entity.RManufactureNumber;
                            if (rManufactureNumber == projectEntity.ManufactureNumber)
                            {
                                continue;
                            }
                            if (IsDuplicate(projectEntity.ManufactureNumber, linkFileWindow.linkFileViewModel.SelectProjectFilesEntity))
                            {
                                continue;
                            }
                            AddProjectFilesEntity = new ProjectFilesEntity();
                            AddProjectFilesEntity.DepartmentID = app.UserInfo.DepartmentID;
                            AddProjectFilesEntity.FileDiscript = entity.FileDiscript;
                            AddProjectFilesEntity.FileName = entity.FileName;
                            AddProjectFilesEntity.FileType = entity.FileType;
                            AddProjectFilesEntity.ManufactureNumber = projectEntity.ManufactureNumber;
                            AddProjectFilesEntity.RManufactureNumber = rManufactureNumber;
                            AddProjectFilesEntity.UserID = app.UserInfo.UserID;
                            AddProjectFilesEntity.FileUpdateTime = entity.FileUpdateTime;
                            AddProjectFilesEntity.FileTypeID = entity.FileTypeID;
                            AddProjectFilesEntity.FileBytes = entity.FileBytes;

                            AddProjectFilesEntity.DepartmentName = entity.DepartmentName;
                            AddProjectFilesEntity.UserName = entity.UserName;
                            AddProjectFilesEntity.FileDeletePersionName = entity.FileDeletePersionName;
                            AddProjectFilesEntity.FileTypeName = entity.FileTypeName;

                            AddProjectFilesEntity.ProjectFiles = new ProductManager.Web.Model.project_files();
                            ProjectFilesEntityList.Add(AddProjectFilesEntity);
                            AddProjectFilesEntity.DUpdate();
                            ProductDomainContextForFile.project_files.Add(AddProjectFilesEntity.ProjectFiles);
                        }
                    }
                }
                IsBusy = true;
                SubmitOperation subOperation = ProductDomainContextForFile.SubmitChanges();
                subOperation.Completed += SubOperationCommpleted;
            }
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
                ProductDomainContextForFile.project_files.Add(AddProjectFilesEntity.ProjectFiles);
                IsBusy = true;
                SubmitOperation subOperation = ProductDomainContextForFile.SubmitChanges();
                subOperation.Completed += SubOperationCommpleted;
            }
        }

        private void SubOperationCommpleted(object sender, EventArgs e)
        {
            IsBusy = false;        
        }

        private bool CanUpdateFileCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010200);
            }
            else
            {
                return false;
            }
        }

        private void OnDeleteFileCommand()
        {
            DeleteFileWindow deleteFileWindow = new DeleteFileWindow(SelectProjectFilesEntity);
            deleteFileWindow.Closed += deleteFileWindow_Closed;
            deleteFileWindow.Show();
        }

        void deleteFileWindow_Closed(object sender, EventArgs e)
        {
            DeleteFileWindow deleteFileWindow = sender as DeleteFileWindow;
            if (deleteFileWindow.DialogResult == true)
            {
                SelectProjectFilesEntity.RaiseCommand();
                //(SelectProjectFilesEntity.OnDownload as DelegateCommand).RaiseCanExecuteChanged();
                (OnModifyFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                SubmitOperation submitOperation = ProductDomainContextForFile.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
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
                LoadProjectFiles();
            }
        }

        private bool CanDeleteFileCommand(object aObject)
        {
            if (selectProjectFilesEntity != null && !selectProjectFilesEntity.FileDelete && SelectProjectEntity != null && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010300);
            }
            else
            {
                return false;
            }
        }

        private void OnModifyFileCommand()
        {
            ModifyFileWindow modifyFileWindow = new ModifyFileWindow(FileTypeEntityList, selectProjectFilesEntity);
            modifyFileWindow.Closed += modifyFileWindow_Closed;
            modifyFileWindow.Show();
        }

        void modifyFileWindow_Closed(object sender, EventArgs e)
        {
            ModifyFileWindow modifyFileWindow = sender as ModifyFileWindow;
            if (modifyFileWindow.DialogResult == true)
            {
                SelectProjectFilesEntity.RaiseCommand();
                //(SelectProjectFilesEntity.OnDownload as DelegateCommand).RaiseCanExecuteChanged();
                (OnModifyFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                SubmitOperation submitOperation = ProductDomainContextForFile.SubmitChanges();
                submitOperation.Completed += submitModifyOperation_Completed;
            }
        }

        private void submitModifyOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "修改失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("删除成功", "修改成功！");
                notifyWindow.Show();
                //LoadData();
                LoadProjectFiles();
            }
        }

        private bool CanModifyFileCommand(Object aObject)
        {
            if (selectProjectFilesEntity != null && !selectProjectFilesEntity.FileDelete && SelectProjectEntity != null && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010200);
            }
            else
            {
                return false;
            }
        }

        private void OnDeleteFinalFileCommand()
        {
            ConfirmWindow lConfirmWindow = new ConfirmWindow("删除文件", "是否永久删除文件？");
            lConfirmWindow.Closed += ConfirmWindow_Closed;
            lConfirmWindow.Show();
        }

        void ConfirmWindow_Closed(object sender, EventArgs e)
        {
            ConfirmWindow confirmWindow = sender as ConfirmWindow;
            if (confirmWindow.DialogResult == true)
            {
                ProductDomainContextForFile.project_files.Remove(selectProjectFilesEntity.ProjectFiles);
                //SelectProjectFilesEntity.RaiseCommand();
                //(SelectProjectFilesEntity.OnDownload as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteFinalFile as DelegateCommand).RaiseCanExecuteChanged();
                SubmitOperation submitOperation = ProductDomainContextForFile.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        private bool CanDeleteFinalFileCommand(object aObject)
        {
            if (selectProjectFilesEntity != null && SelectProjectEntity != null && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2010800);
            }
            else
            {
                return false;
            }
        }

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

        private void LoadProjectFiles()
        {
            IsBusy = true;
            if (SelectProjectEntity == null)
            {
                ProjectFilesEntityList.Clear();
            }
            else
            {
                LoadOperation<ProductManager.Web.Model.project_files> loadOperationProjectFiles =
                    ProductDomainContextForFile.Load<ProductManager.Web.Model.project_files>(ProductDomainContextForFile.GetProject_filesByIDQuery(SelectProjectEntity.ManufactureNumber));
                loadOperationProjectFiles.Completed += loadOperationProjectFiles_Completed;
            }
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
            IsBusy = false;
        }

        public void HideExpander()
        {
            ShowExpander = false;
        }

        private void OnExportCommand()
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == true)
            {
                try
                {
                    Workbook workbook = new Workbook();
                    Worksheet worksheet = new Worksheet("生产令号汇总");

                    //Title
                    worksheet.Cells[0, 0] = new Cell("生产令号");
                    worksheet.Cells[0, 1] = new Cell("项目名称");
                    worksheet.Cells[0, 2] = new Cell("型号");
                    worksheet.Cells[0, 3] = new Cell("备注");
                    worksheet.Cells[0, 4] = new Cell("年份");
                    worksheet.Cells[0, 5] = new Cell("记录时间");
                    worksheet.Cells[0, 6] = new Cell("删除标记");
                    worksheet.Cells[0, 7] = new Cell("冻结标记");
                    worksheet.Cells[0, 8] = new Cell("发运完成时间");
                    worksheet.Cells[0, 9] = new Cell("项目合同号");
                    worksheet.Cells[0, 10] = new Cell("开票完成时间");


                    Int16 RowCount = 1;

                    foreach (ProjectEntity projectEntity in ProjectEntityList)
                    {
                        int columnCount = 0;

                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.ManufactureNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.ProjectName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.ModelNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.Remark);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.YearNumber.ToString());
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.RecordDateString);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.IsDeleteString);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.IsFreezeString);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.DeliveryTime.HasValue? projectEntity.DeliveryTime.Value.ToShortDateString():"");
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.ContractNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(projectEntity.InvoiceCompletionTime.HasValue? projectEntity.InvoiceCompletionTime.Value.ToShortDateString():"");
                        RowCount++;
                    }
                    workbook.Worksheets.Add(worksheet);

                    Stream sFile = sDialog.OpenFile();
                    workbook.Save(sFile);

                    Message.InfoMessage("导出成功");
                }
                catch (Exception outputE)
                {
                    string errorMessage = "导出文件失败：" + outputE.Message;
                    Message.ErrorMessage(errorMessage);
                }
            }
        }

        private void OnPrintCommand()
        {

            ReportParame lReportParame = new ReportParame();
            lReportParame.ReportTitle = "生产令号汇总表";

            lReportParame.Title = new List<String>();
            lReportParame.TitleWidth = new List<int>();

            lReportParame.Title.Add("生产令号");
            lReportParame.TitleWidth.Add(100);

            lReportParame.Title.Add("项目名称");
            lReportParame.TitleWidth.Add(330);

            lReportParame.Title.Add("型号");
            lReportParame.TitleWidth.Add(90);

            lReportParame.Title.Add("年份");
            lReportParame.TitleWidth.Add(80);

            lReportParame.Title.Add("记录时间");
            lReportParame.TitleWidth.Add(110);

            //lReportParame.Title.Add("删除标记");
            //lReportParame.TitleWidth.Add(70);

            //lReportParame.Title.Add("冻结标记");
            //lReportParame.TitleWidth.Add(70);

            //lReportParame.Title.Add("备注");
            //lReportParame.TitleWidth.Add(450);
            lReportParame.Values = new List<List<String>>();

            foreach (ProjectEntity projectEntity in ProjectEntityList)
            {
                List<String> lValue = new List<String>();

                // 生产令号
                lValue.Add(projectEntity.ManufactureNumber);

                // 项目名称
                lValue.Add(projectEntity.ProjectName);

                // 型号
                lValue.Add(projectEntity.ModelNumber);

                // 年份
                lValue.Add(projectEntity.YearNumber.ToString());

                // 记录时间
                lValue.Add(projectEntity.RecordDateString);

                // 删除标记
                //lValue.Add(projectEntity.IsDeleteString);

                // 冻结标记
                //lValue.Add(projectEntity.IsFreezeString);

                // 备注
                //lValue.Add(projectEntity.Remark);

                lReportParame.Values.Add(lValue);
            }

            App app = Application.Current as App;
            String lUser = app.UserInfo.UserName;

            PointWindow lPointWindow = new PointWindow(lReportParame, lUser);
            lPointWindow.Show();
        }

        private void OnImportCommand()
        {
            // private Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }
            ImportProjectWindow lImportProjectWindow = new ImportProjectWindow(ProductDomainContext, ProjectEntityDictionary);
            lImportProjectWindow.Closed += lImportProjectWindow_Closed;

            lImportProjectWindow.Show();
        }

        private void lImportProjectWindow_Closed(object sender, EventArgs e)
        {
            ImportProjectWindow lImportProjectWindow = sender as ImportProjectWindow;
            if (lImportProjectWindow.DialogResult == true)
            {
                SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
                subOperation.Completed += ImportProjectFinish;
            }
        }

        private bool CanImportCommand(Object aObject)
        {
            App app = Application.Current as App;
            return app.UserInfo.GetUerRight(2010100);
        }

        private void OnImportProductCommand()
        {
            ImportProductWindow lImportProductWindow = new ImportProductWindow(ProductDomainContext, ProjectEntityDictionary, ProductEntityDictionary, ProductTypeEntityDictionary);
            lImportProductWindow.Closed += lImportProductWindow_Closed;
            lImportProductWindow.Show();
        }

        void lImportProductWindow_Closed(object sender, EventArgs e)
        {
            ImportProductWindow lImportProductWindow = sender as ImportProductWindow;
            if (lImportProductWindow.DialogResult == true)
            {
                SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
                subOperation.Completed += ImportProjectFinish;
            }
        }

        private bool CanImportProductCommand(Object aObject)
        {
            App app = Application.Current as App;
            return app.UserInfo.GetUerRight(2011300);
        }

        private void ImportProjectFinish(object sender, EventArgs e)
        {
            ProductDomainContext.projects.Clear();

            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
            ProductDomainContext.PropertyChanged -= ImportProjectFinish;
        }

        private void OnAddImportantPartCommand()
        {
            AddImportantPartEntity = new ImportantPartEntity();
            AddImportantPartEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddImportantPartEntity.UserEntityDictionary = UserEntityDictionary;
            AddImportantPartEntity.ImportantPart = new ProductManager.Web.Model.important_part();
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.Add, AddImportantPartEntity);
            importantPartWindow.Closed += importantPartWindow_Closed;
            importantPartWindow.Show();
        }

        private bool CanAddImportantPartCommand(object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050500);
            }
            else
            {
                return false;
            }
        }

        private void OnImportImportantPartCommand()
        {
            ImportImportantPartWindow importImportantPartWindow = new ImportImportantPartWindow(ProjectEntityDictionary, ProductDomainContext, UserEntityDictionary, SelectProjectEntity);
            importImportantPartWindow.Closed += importImportantPartWindow_Closed;
            importImportantPartWindow.Show();
        }

        void importImportantPartWindow_Closed(object sender, EventArgs e)
        {
            ImportImportantPartWindow importImportantPartWindow = sender as ImportImportantPartWindow;
            if (importImportantPartWindow.DialogResult == true)
            {

                SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
                subOperation.Completed += ImportImportImportantPartFinish;

            }
        }

        private void ImportImportImportantPartFinish(object sender, EventArgs e)
        {
            using (this.importantPartView.DeferRefresh())
            {
                this.importantPartView.MoveToFirstPage();
            }
        }

        private bool CanImportImportantPartCommand(Object aObject)
        {
            //if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
           // {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050500);
           // }
           // else
           // {
           //     return false;
           // }
        }

        void importantPartWindow_Closed(object sender, EventArgs e)
        {
            ImportantPartWindow importantPartWindow = sender as ImportantPartWindow;
            if (importantPartWindow.DialogResult == true)
            {
                ImportantPartEntityList.Add(AddImportantPartEntity);
                ProductDomainContext.important_parts.Add(AddImportantPartEntity.ImportantPart);
                OnSaveCommand();
            }
        }

        private void OnArivePartCommand()
        {
            SelectImportantPartEntity.AriveTime = DateTime.Now;
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.ARIVE, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            //importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        void importantPartWindowModify_Closed(object sender, EventArgs e)
        {
            ImportantPartWindow importantPartWindow = sender as ImportantPartWindow;
            if (importantPartWindow.DialogResult == true)
            {
                (OnArivePart as DelegateCommand).RaiseCanExecuteChanged();
                (OnOutPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnModifyImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                OnSaveCommand();
            }
        }

        private bool CanArivePartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null /*&& !SelectImportantPartEntity.IsArive*/)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050500);
            }
            return false;
        }

        private void OnOutPartCommand()
        {
            //SelectImportantPartEntity.PartOutTime = DateTime.Now;
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.OUT, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        private bool CanOnOutPartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null /*&& SelectImportantPartEntity.IsArive && !SelectImportantPartEntity.IsOut*/)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050500);
            }
            return false;
        }

        private void OnModifyImportantPartCommand()
        {
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.MODIFY, SelectImportantPartEntity);
            importantPartWindow.Closed += importantPartWindowModify_Closed;
            importantPartWindow.Show();
        }

        private bool CanModifyImportantPartCommand(object aObject)
        {
            if (SelectProjectEntity == null)
            {
                return false;
            }
            if (SelectImportantPartEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze && !SelectImportantPartEntity.IsDelete)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050500);
            }
            return false;
        }

        private void OnViewImportantPartCommand()
        {
            ImportantPartWindow importantPartWindow = new ImportantPartWindow(ImportantPartWindowState.VIEW, SelectImportantPartEntity);
            importantPartWindow.Show();
        }

        private bool CanViewImportantPartCommand(object aObject)
        {
            if (SelectImportantPartEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnDeleteImportantPartCommand()
        {
            DeleteImportantPartWindow deleteImportantPartWindow = new DeleteImportantPartWindow(SelectImportantPartEntity);
            deleteImportantPartWindow.Closed += deleteImportantPartWindow_Closed;
            deleteImportantPartWindow.Show();
        }

        private void deleteImportantPartWindow_Closed(object sender, EventArgs e)
        {
            DeleteImportantPartWindow deleteImportantPartWindow = sender as DeleteImportantPartWindow;
            if (deleteImportantPartWindow.DialogResult == true)
            {
                (OnArivePart as DelegateCommand).RaiseCanExecuteChanged();
                (OnOutPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnModifyImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteImportantPart as DelegateCommand).RaiseCanExecuteChanged();
                OnSaveCommand();
            }
        }

        private bool CanDeleteImportantPartCommand(Object aObject)
        {
            if (SelectProjectEntity == null)
            {
                return false;
            }
            if (SelectImportantPartEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze && !SelectImportantPartEntity.IsDelete)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050700);
            }
            return false;
        }

        private void OnFinalDeleteImportantPartCommand()
        {
            ProductDomainContext.important_parts.Remove(SelectImportantPartEntity.ImportantPart);
            SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
            subOperation.Completed += OnFinalDeleteImportantPartCommand_Completed;
        }


        void OnFinalDeleteImportantPartCommand_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("成功", "成功！");
                notifyWindow.Show();
                using (importantPartView.DeferRefresh())
                {
                    importantPartView.MoveToFirstPage();
                }

                //LoadData();
            }
        }

        private bool CanFinalDeleteImportantPartCommand(Object aObject)
        {
            if (SelectProjectEntity == null)
            {
                return false;
            }
            if (SelectImportantPartEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050900);
            }
            return false;
        }


        private void OnAddImportantPartRejesterCommand()
        {
            AddImportantPartRejesterEntity = new ImportantPartRejesterEntity();
            AddImportantPartRejesterEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddImportantPartRejesterEntity.UserEntityDictionary = UserEntityDictionary;
            AddImportantPartRejesterEntity.ImportantPartRejester = new ProductManager.Web.Model.important_part_rejester();
            ImportantPartRejesterWindow importantPartRejesterWindow = new ImportantPartRejesterWindow(ImportantPartRejesterWindowState.Add, AddImportantPartRejesterEntity);
            importantPartRejesterWindow.Closed += importantPartRejesterWindow_Closed;
            importantPartRejesterWindow.Show();
        }

        private bool CanAddImportantPartRejesterCommand(Object aObject)
        {
            if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050600);
            }
            else
            {
                return false;
            }
        }

        private void OnImportImportantPartRejesterCommand()
        {
            ImportImportantPartRejesterWindow importImportantPartRejesterWindow = new ImportImportantPartRejesterWindow(ProjectEntityDictionary, ProductDomainContext, UserEntityDictionary, SelectProjectEntity);
            importImportantPartRejesterWindow.Closed += importImportantPartRejesterWindow_Closed;
            importImportantPartRejesterWindow.Show();
        }

        private void importImportantPartRejesterWindow_Closed(object sender, EventArgs e)
        {
            ImportImportantPartRejesterWindow importImportantPartRejesterWindow = sender as ImportImportantPartRejesterWindow;
            if (importImportantPartRejesterWindow.DialogResult == true)
            {
                SubmitOperation subOperation = ProductDomainContext.SubmitChanges();
                subOperation.Completed += ImportImportImportantPartRejesterFinish;
            }
        }

        private void ImportImportImportantPartRejesterFinish(object sender, EventArgs e)
        {
            using (this.importantPartRejesterView.DeferRefresh())
            {
                this.importantPartRejesterView.MoveToFirstPage();
            }
        }

        private bool CanImportImportantPartRejesterCommand(Object aObject)
        {
            //if (SelectProjectEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze)
            //{
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050600);
            //}
            //else
            //{
            //    return false;
            //}
        }

        private void OnViewImportantPartRejesterCommand()
        {
            ImportantPartRejesterWindow importantPartRejesterWindow = new ImportantPartRejesterWindow(ImportantPartRejesterWindowState.VIEW, SelectImportantPartRejesterEntity);
            importantPartRejesterWindow.Show();
        }

        private bool CanViewImportantPartRejesterCommand(Object aObject)
        {
            if (SelectImportantPartRejesterEntity != null)
            {
                return true;
            }
            return false;
        }

        private void OnModifyImportantPartRejesterCommand()
        {
            ImportantPartRejesterWindow importantPartRejesterWindow = new ImportantPartRejesterWindow(ImportantPartRejesterWindowState.MODIFY, SelectImportantPartRejesterEntity);
            importantPartRejesterWindow.Closed += importantPartRejesterWindowModify_Closed;
            importantPartRejesterWindow.Show();
        }

        private bool CanModifyImportantPartRejesterCommand(Object aObject)
        {
            if (SelectProjectEntity == null)
            {
                return false;
            }
            if (SelectImportantPartRejesterEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze && !SelectImportantPartRejesterEntity.IsDelete)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050600);
            }
            return false;
        }

        private void importantPartRejesterWindow_Closed(object sender, EventArgs e)
        {
            ImportantPartRejesterWindow importantPartRejesterWindow = sender as ImportantPartRejesterWindow;
            if (importantPartRejesterWindow.DialogResult == true)
            {
                ImportantPartRejesterEntityList.Add(AddImportantPartRejesterEntity);
                ProductDomainContext.important_part_rejesters.Add(AddImportantPartRejesterEntity.ImportantPartRejester);
                OnSaveCommand();
            }
        }

        private void importantPartRejesterWindowModify_Closed(object sender, EventArgs e)
        {
            ImportantPartRejesterWindow importantPartRejesterWindow = sender as ImportantPartRejesterWindow;
            if (importantPartRejesterWindow.DialogResult == true)
            {
                (OnModifyImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                OnSaveCommand();
            }
        }

        private void OnDeleteImportantPartRejesterCommand()
        {
            DeleteImportantPartRejisterWindow deleteImportantPartRejisterWindow = new DeleteImportantPartRejisterWindow(SelectImportantPartRejesterEntity);
            deleteImportantPartRejisterWindow.Closed += deleteImportantPartRejesterWindow_Closed;
            deleteImportantPartRejisterWindow.Show();
        }

        private void deleteImportantPartRejesterWindow_Closed(object sender, EventArgs e)
        {
            DeleteImportantPartRejisterWindow deleteImportantPartRejisterWindow = sender as DeleteImportantPartRejisterWindow;
            if (deleteImportantPartRejisterWindow.DialogResult == true)
            {
                (OnModifyImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                (OnViewImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                (OnDeleteImportantPartRejester as DelegateCommand).RaiseCanExecuteChanged();
                OnSaveCommand();
            }
        }

        private bool CanDeleteImportantPartRejesterCommand(Object aObject)
        {
            if (SelectProjectEntity == null)
            {
                return false;
            }
            if (SelectImportantPartRejesterEntity != null && !SelectProjectEntity.IsDelete && !SelectProjectEntity.IsFreeze && !SelectImportantPartRejesterEntity.IsDelete)
            {
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(2050800);
            }
            return false;
        }

    }
}
