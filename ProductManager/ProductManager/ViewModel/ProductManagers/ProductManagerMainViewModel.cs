using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("ProductManagerMain")]
    public class ProductManagerMainViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext;

        private SystemManageDomainContext SystemManageDomainContext;

        private DomainCollectionView<ProductManager.Web.Model.important_part> importantPartView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.important_part> importantPartLoader;
        private EntityList<ProductManager.Web.Model.important_part> importantPartSource;

        public ObservableCollection<ImportantPartEntity> ImportantPartEntityList { get; set; }

        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }
        private Dictionary<int, UserEntity> UserEntityDictionary { get; set; }
        private Dictionary<String, UserEntity> UserEntityDictionaryName { get; set; }

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

        public String ManufactureNumberSearch { get; set; }
        public String ProjectNameSearch { get; set; }
        public String ImportantPartNameSearch { get; set; }
        public String ImportantPartManufacturers { get; set; }
        public Nullable<DateTime> AriveTimeStart { get; set; }
        public Nullable<DateTime> AriveTimeEnd { get; set; }
        public Nullable<DateTime> PartOutTimeStart { get; set; }
        public Nullable<DateTime> PartOutTimeEnd { get; set; }
        public String PartOutNumberSearch { get; set; }
        public String PartManuNumber { get; set; }

        public ICommand OnSearch { get; private set; }
        public ICommand OnExport { get; private set; }
        public ICommand OnPrint { get; private set; }

        public ProductManagerMainViewModel()
        {
            ImportantPartEntityList = new ObservableCollection<ImportantPartEntity>();
            ProjectEntityDictionary = new Dictionary<String, ProjectEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            UserEntityDictionaryName = new Dictionary<String, UserEntity>();

            OnSearch = new DelegateCommand(OnSearchCommand);
            OnExport = new DelegateCommand(OnExportCommand);
            OnPrint = new DelegateCommand(OnPrintCommand);
        }

        public void LoadData()
        {
            IsBusy = true;
            ProductDomainContext = new ProductDomainContext();
            SystemManageDomainContext = new SystemManageDomainContext();

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

            this.importantPartSource = new EntityList<ProductManager.Web.Model.important_part>(this.ProductDomainContext.important_parts);
            this.importantPartLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.important_part>(
                this.LoadImportantPartEntities,
                this.LoadImportantPartCompleted);
            this.importantPartView = new DomainCollectionView<ProductManager.Web.Model.important_part>(this.importantPartLoader, this.importantPartSource);

            using (this.importantPartView.DeferRefresh())
            {
                this.importantPartView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.important_part> LoadImportantPartEntities()
        {
            EntityQuery<ProductManager.Web.Model.important_part> lQuery = this.ProductDomainContext.GetImportant_partQuery();
            
            if (ManufactureNumberSearch != null && ManufactureNumberSearch.Length > 0)
            {
                lQuery = lQuery.Where(t => t.manufacture_number.ToLower().Contains(ManufactureNumberSearch.ToLower()));
            }

            if (ImportantPartNameSearch != null && ImportantPartNameSearch.Length > 0)
            {
                lQuery = lQuery.Where(t => t.important_part_name.ToLower().Contains(ImportantPartNameSearch.ToLower()));
            }

            if (ImportantPartManufacturers != null && ImportantPartManufacturers.Length > 0)
            {
                lQuery = lQuery.Where(t => t.important_part_manufacturers.ToLower().Contains(ImportantPartManufacturers.ToLower()));
            }

            if (AriveTimeStart.HasValue && AriveTimeEnd.HasValue )
            {
                lQuery = lQuery.Where(t => t.arive_time >= AriveTimeStart.Value);
                lQuery = lQuery.Where(t => t.arive_time <= AriveTimeEnd.Value);
            }

            /*if (PartOutTimeStart.HasValue && PartOutTimeEnd.HasValue)
            {
                lQuery = lQuery.Where(t => t.part_out_time >= PartOutTimeStart.Value);
                lQuery = lQuery.Where(t => t.part_out_time <= PartOutTimeEnd.Value);
            }
            */
            /*if (PartOutNumberSearch != null && PartOutNumberSearch.Length > 0)
            {
                lQuery = lQuery.Where(t => t.part_out_number.Contains(PartOutNumberSearch));
            }

            if (PartManuNumber != null && PartManuNumber.Length > 0)
            {
                lQuery = lQuery.Where(t => t.part_manu_number.Contains(PartManuNumber));
            }
            */
            lQuery = lQuery.OrderByDescending(e => e.arive_time);
            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.importantPartView));
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

                ProjectEntity projectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(importantPartEntity.ManufactureNumber, out projectEntityTemp))
                {
                    importantPartEntity.ProjectName = projectEntityTemp.ProjectName;
                }

                if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                {
                    if (!importantPartEntity.ProjectName.ToLower().Contains(ProjectNameSearch.ToLower()))
                    {
                        continue;
                    }
                }

                ImportantPartEntityList.Add(importantPartEntity);
            }
            IsBusy = false;
        }
        private void OnSearchCommand()
        {
            ShowExpander = false;
            using (this.importantPartView.DeferRefresh())
            {
                this.importantPartView.MoveToFirstPage();
            }
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
                    Worksheet worksheet = new Worksheet("外购部件汇总");

                    //Title
                    worksheet.Cells[0, 0] = new Cell("生产令号");
                    worksheet.Cells[0, 1] = new Cell("项目名称");
                    worksheet.Cells[0, 2] = new Cell("主要部件名称");
                    worksheet.Cells[0, 3] = new Cell("厂家");
                    worksheet.Cells[0, 4] = new Cell("到货时间");

                    Int16 RowCount = 1;

                    foreach (ImportantPartEntity importantPartEntity in ImportantPartEntityList)
                    {
                        int columnCount = 0;

                        if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                        {
                            if (!importantPartEntity.ProjectName.Contains(ProjectNameSearch))
                            {
                                continue;
                            }
                        }

                        worksheet.Cells[RowCount, columnCount++] = new Cell(importantPartEntity.ManufactureNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(importantPartEntity.ProjectName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(importantPartEntity.ImportantPartName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(importantPartEntity.ImportantPartManufacturers);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(importantPartEntity.AriveTime.HasValue?importantPartEntity.AriveTime.Value.ToShortDateString():"");

                        RowCount++;
                    }
                    workbook.Worksheets.Add(worksheet);

                    Stream sFile = sDialog.OpenFile();
                    workbook.Save(sFile);

                    ProductManager.Controls.Message.InfoMessage("导出成功");
                }
                catch (Exception outputE)
                {
                    string errorMessage = "导出文件失败：" + outputE.Message;
                    ProductManager.Controls.Message.ErrorMessage(errorMessage);
                }
            }
        }

        private void OnPrintCommand()
        {
            ReportParame lReportParame = new ReportParame();
            lReportParame.ReportTitle = "外购部件汇总表";

            lReportParame.Title = new List<String>();
            lReportParame.TitleWidth = new List<int>();

            lReportParame.Title.Add("生产令号");
            lReportParame.TitleWidth.Add(150);

            lReportParame.Title.Add("项目名称");
            lReportParame.TitleWidth.Add(350);

            lReportParame.Title.Add("主要部件名称");
            lReportParame.TitleWidth.Add(150);

            lReportParame.Title.Add("厂家");
            lReportParame.TitleWidth.Add(150);

            lReportParame.Title.Add("到货时间");
            lReportParame.TitleWidth.Add(150);
// 
//             lReportParame.Title.Add("出厂日期");
//             lReportParame.TitleWidth.Add(100);
// 
//             lReportParame.Title.Add("出厂编号");
//             lReportParame.TitleWidth.Add(100);
// 
//             lReportParame.Title.Add("厂家序列号");
//             lReportParame.TitleWidth.Add(100);

            lReportParame.Values = new List<List<String>>();

            foreach (ImportantPartEntity importantPartEntity in ImportantPartEntityList)
            {
                if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                {
                    if (!importantPartEntity.ProjectName.Contains(ProjectNameSearch))
                    {
                        continue;
                    }
                }
                List<String> lValue = new List<String>();

                lValue.Add(importantPartEntity.ManufactureNumber);
                lValue.Add(importantPartEntity.ProjectName);
                lValue.Add(importantPartEntity.ImportantPartName);
                lValue.Add(importantPartEntity.ImportantPartManufacturers);
                lValue.Add(importantPartEntity.AriveTime.HasValue ? importantPartEntity.AriveTime.Value.ToShortDateString() : "");
//                 lValue.Add(importantPartEntity.PartOutTimeString);
//                 lValue.Add(importantPartEntity.PartOutNumber);
//                 lValue.Add(importantPartEntity.PartManuNumber);
                lReportParame.Values.Add(lValue);
            }

            App app = Application.Current as App;
            String lUser = app.UserInfo.UserName;

            PointWindow lPointWindow = new PointWindow(lReportParame, lUser);
            lPointWindow.Show();
        }
    }
}
