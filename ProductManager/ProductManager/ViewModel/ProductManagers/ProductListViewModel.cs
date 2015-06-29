using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq.Expressions;
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
    [Export("ProductList")]
    public class ProductListViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext;

        private DomainCollectionView<ProductManager.Web.Model.product> productView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.product> productLoader;
        private EntityList<ProductManager.Web.Model.product> productSource;

        private Dictionary<String, ProjectEntity> ProjectEntityDictionary;

        public ObservableCollection<ProductEntity> ProductEntityList { get; set; }

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
        public String ProductIDSearch { get; set; }
        public String ProductNameSearch { get; set; }
        public String ProductOutNumberSearch { get; set; }

        public ICommand OnSearch { get; private set; }
        public ICommand OnExport { get; private set; }
        public ICommand OnPrint { get; private set; }

        public ProductListViewModel()
        {
            ProjectEntityDictionary = new Dictionary<String, ProjectEntity>();
            ProductEntityList = new ObservableCollection<ProductEntity>();

            OnSearch = new DelegateCommand(OnSearchCommand);
            OnExport = new DelegateCommand(OnExportCommand);
            OnPrint = new DelegateCommand(OnPrintCommand);
        }

        public void LoadData()
        {
            IsBusy = true;

            ProductDomainContext = new ProductDomainContext();

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

            this.productSource = new EntityList<ProductManager.Web.Model.product>(this.ProductDomainContext.products);
            this.productLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.product>(
                this.LoadProductEntities,
                this.LoadOperationProductCompleted);
            this.productView = new DomainCollectionView<ProductManager.Web.Model.product>(this.productLoader, this.productSource);
            using (this.productView.DeferRefresh())
            {
                this.productView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.product> LoadProductEntities()
        {
            this.IsBusy = true;


            EntityQuery<ProductManager.Web.Model.product> lQuery = this.ProductDomainContext.GetProductQuery();

            {
                if (ManufactureNumberSearch != null && ManufactureNumberSearch.Length > 0)
                {
                    lQuery = lQuery.Where(t => t.manufacture_number.Contains(ManufactureNumberSearch));
                }
                if (ProductNameSearch != null && ProductNameSearch.Length > 0)
                {
                    lQuery = lQuery.Where(t => t.product_name.Contains(ProductNameSearch));
                }
                if (ProductIDSearch != null && ProductIDSearch.Length > 0)
                {
                    lQuery = lQuery.Where(t => t.product_id.Contains(ProductIDSearch));
                }
            }

            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productView));
        }

        private void LoadOperationProductCompleted(LoadOperation<ProductManager.Web.Model.product> aLoadOperation)
        {
            ProductEntityList.Clear();
            foreach (ProductManager.Web.Model.product product in aLoadOperation.Entities)
            {
                ProductEntity productEntity = new ProductEntity();
                productEntity.Product = product;
                productEntity.Update();

                ProjectEntity lProjectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(productEntity.ManufactureNumber, out lProjectEntityTemp))
                {
                    productEntity.ProjectName = lProjectEntityTemp.ProjectName;
                }

                if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                {
                    if (!productEntity.ProjectName.Contains(ProjectNameSearch))
                    {
                        continue;
                    }
                }

                ProductEntityList.Add(productEntity);
            }
            IsBusy = false;
        }

        private void OnSearchCommand()
        {
            ShowExpander = false;
            using (this.productView.DeferRefresh())
            {
                this.productView.MoveToFirstPage();
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
                    Worksheet worksheet = new Worksheet("产品汇总");

                    //Title
                    worksheet.Cells[0, 0] = new Cell("生产令号");
                    worksheet.Cells[0, 1] = new Cell("项目名称");
                    worksheet.Cells[0, 2] = new Cell("产品序号");
                    worksheet.Cells[0, 3] = new Cell("产品名称");
                    worksheet.Cells[0, 4] = new Cell("出厂编号");

                    Int16 RowCount = 1;

                    foreach (ProductEntity productEntity in ProductEntityList)
                    {
                        int columnCount = 0;

                        if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                        {
                            if (!productEntity.ProjectName.Contains(ProjectNameSearch))
                            {
                                continue;
                            }
                        }

                        worksheet.Cells[RowCount, columnCount++] = new Cell(productEntity.ManufactureNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(productEntity.ProjectName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(productEntity.ProductID);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(productEntity.ProductName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(productEntity.ProductOutputNumber);

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
            lReportParame.ReportTitle = "产品汇总表";

            lReportParame.Title = new List<String>();
            lReportParame.TitleWidth = new List<int>();

            lReportParame.Title.Add("生产令号");
            lReportParame.TitleWidth.Add(100);

            lReportParame.Title.Add("项目名称");
            lReportParame.TitleWidth.Add(330);

            lReportParame.Title.Add("产品序号");
            lReportParame.TitleWidth.Add(100);

            lReportParame.Title.Add("产品名称");
            lReportParame.TitleWidth.Add(80);

            lReportParame.Title.Add("出厂编号");
            lReportParame.TitleWidth.Add(110);


            lReportParame.Values = new List<List<String>>();

            foreach (ProductEntity productEntity in ProductEntityList)
            {
                if (ProjectNameSearch != null && ProjectNameSearch.Length > 0)
                {
                    if (!productEntity.ProjectName.Contains(ProjectNameSearch))
                    {
                        continue;
                    }
                }
                List<String> lValue = new List<String>();

                // 生产令号
                lValue.Add(productEntity.ManufactureNumber);

                // 项目名称
                lValue.Add(productEntity.ProjectName);

                // 产品序号
                lValue.Add(productEntity.ProductID);

                // 产品名称
                lValue.Add(productEntity.ProductName);

                // 出厂编号
                lValue.Add(productEntity.ProductOutputNumber);

                lReportParame.Values.Add(lValue);
            }

            App app = Application.Current as App;
            String lUser = app.UserInfo.UserName;

            PointWindow lPointWindow = new PointWindow(lReportParame, lUser);
            lPointWindow.Show();
        }
    }
}
