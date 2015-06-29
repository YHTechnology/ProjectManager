using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Lite.ExcelLibrary.SpreadSheet;
using ProductManager.Controls;
using ProductManager.FileUploader;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    public class ImportProductWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;
        private ProductDomainContext ProductContext;

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

        public ObservableCollection<ProductEntity> ProductEntityList { get; set; }
        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }
        public Dictionary<String, ProductEntity> ProductEntityDictionary { get; set; }
        public Dictionary<int, ProductTypeEntity> ProductTypeEntityDictionary { get; set; }
        public Dictionary<String, int> ProductTypeIDDictionary { get; set; }

        public Dictionary<String, ProductEntity> CurrentProductEntityDicationary;

        SaveFileDialog saveFileDialog;

        public ICommand OnImport { get; private set; }
        public ICommand OnDownloadTemp { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnOK { get; private set; }

        public ImportProductWindowViewModel(ChildWindow aChildWindow
                                , ProductDomainContext aProductDomainContext
                                , Dictionary<String, ProjectEntity> aProjectEntityDictionary
                                , Dictionary<String, ProductEntity> aProductEntityDictionary
                                , Dictionary<int, ProductTypeEntity> aProductTypeEntityDictionary)
        {
            childWindow = aChildWindow;
            ProductContext = aProductDomainContext;

            ProductEntityList = new ObservableCollection<ProductEntity>();

            ProjectEntityDictionary = aProjectEntityDictionary;
            ProductEntityDictionary = aProductEntityDictionary;
            ProductTypeEntityDictionary = aProductTypeEntityDictionary;

            CurrentProductEntityDicationary = new Dictionary<string, ProductEntity>();

            OnImport = new DelegateCommand(OnImportCommand);
            OnDownloadTemp = new DelegateCommand(OnDownloadTempCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            OnOK = new DelegateCommand(OnOKCommand);

            ProductTypeIDDictionary = new Dictionary<string, int>();
            foreach (KeyValuePair<int, ProductTypeEntity> keyValuePairProductType in ProductTypeEntityDictionary)
            {
                ProductTypeIDDictionary.Add(keyValuePairProductType.Value.ProductTypeName, keyValuePairProductType.Value.ProductTypeID);
            }
        }

        private void OnImportCommand()
        {
            OpenFileDialog lOpenFile = new OpenFileDialog();
            lOpenFile.Filter = "Excel (*.xls)|*.xls";
            Dictionary<String, int> lHeaderDictionary = new Dictionary<String, int>();

            if (lOpenFile.ShowDialog() == true)
            {
                ProductEntityList.Clear();

                try
                {
                    FileStream fs = lOpenFile.File.OpenRead();
                    Workbook book = Workbook.Open(fs);

                    foreach (KeyValuePair<int, Row> rowPair in book.Worksheets[0].Cells.Rows)
                    {
                        if (rowPair.Key == 0)
                        {
                            try
                            {
                                foreach (KeyValuePair<int, Cell> cellPair in rowPair.Value)
                                {
                                    lHeaderDictionary.Add(cellPair.Value.StringValue, cellPair.Key);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Message.ErrorMessage("表头重复或超出范围！");
                                break;
                            }
                            continue;
                        }

                        ProductEntity lProductEntity = new ProductEntity();
                        lProductEntity.Product = new ProductManager.Web.Model.product();
                        lProductEntity.ProductEntityDictionary = ProductEntityDictionary;
                        lProductEntity.ProjectEntityDictionary = ProjectEntityDictionary;
                        //lProductEntity.ProductPartTypeEntityDictionary = ProductTypeEntityDictionary;

                        int lManufactureNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("生产令号", out lManufactureNumberColumn)
                                && -1 != lManufactureNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lManufactureNumberColumn)))
                            {
                                lProductEntity.ManufactureNumber = cell.StringValue;
                                ProjectEntity projectEntityTemp;
                                if (!ProjectEntityDictionary.TryGetValue(lProductEntity.ManufactureNumber, out projectEntityTemp))
                                {
                                    NotifyWindow lNotifyWindow = new NotifyWindow("错误","第 " + rowPair.Key.ToString() + "行 系统中没有生产令号：" + lProductEntity.ManufactureNumber);
                                    lNotifyWindow.Show();
                                    return;
                                }
                            }
                            else
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("错误", "第 " + rowPair.Key.ToString() + "行 系统中没有生产令号：" + lProductEntity.ManufactureNumber);
                                lNotifyWindow.Show();
                                return;
                            }
                        }

                        int lProjectNameColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("序列号", out lProjectNameColumn)
                                && -1 != lProjectNameColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lProjectNameColumn)))
                            {
                                lProductEntity.ProductID = cell.StringValue;

                                ProductEntity lProductEntityTemp;
                                if (ProductEntityDictionary.TryGetValue(lProductEntity.ProductID, out lProductEntityTemp))
                                {
                                    NotifyWindow lNotifyWindow = new NotifyWindow("错误","第 " + rowPair.Key.ToString() + "行 序列号重复：" + lProductEntity.ProductID);
                                    lNotifyWindow.Show();
                                    return;
                                }
                                if (CurrentProductEntityDicationary.TryGetValue(lProductEntity.ProductID, out lProductEntityTemp))
                                {
                                    NotifyWindow lNotifyWindow = new NotifyWindow("错误", "第 " + rowPair.Key.ToString() + "行 序列号重复：" + lProductEntity.ProductID);
                                    lNotifyWindow.Show();
                                    return;
                                }
                            }
                        }

                        int lModelNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("产品类型", out lModelNumberColumn)
                                && -1 != lModelNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lModelNumberColumn)))
                            {
                                lProductEntity.ProductTypeString = cell.StringValue;

                                int lId;
                                if (ProductTypeIDDictionary.TryGetValue(lProductEntity.ProductTypeString, out lId))
                                {
                                    lProductEntity.ProductTypeID = lId;
                                }
                                else
                                {
                                    NotifyWindow lNotifyWindow = new NotifyWindow("错误", "第 " + rowPair.Key.ToString() + "行 系统中无该产品类型：" + lProductEntity.ProductTypeString);
                                    lNotifyWindow.Show();
                                    return;
                                }
                            }
                        }

                        int lRemarkColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("名称", out lRemarkColumn)
                                && -1 != lRemarkColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lRemarkColumn)))
                            {
                                lProductEntity.ProductName = cell.StringValue;
                            }
                        }

                        int lInputTimeColume = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("编制日期", out lInputTimeColume)
                                && -1 != lInputTimeColume
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lInputTimeColume)))
                            {
                                lProductEntity.ProductIDCreateData = cell.DateTimeValue;
                            }
                        }

                        int lOutputNumberColume = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("出厂编号", out lOutputNumberColume)
                                && -1 != lOutputNumberColume
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lOutputNumberColume)))
                            {
                                lProductEntity.ProductOutputNumber = cell.StringValue;
                            }
                        }

                        int lNote1 = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("备注1", out lNote1)
                                && -1 != lNote1
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lNote1)))
                            {
                                lProductEntity.ProductDescript1 = cell.StringValue;
                            }
                        }

                        int lNote2 = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("备注1", out lNote2)
                                && -1 != lNote2
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lNote2)))
                            {
                                lProductEntity.ProductDescript2 = cell.StringValue;
                            }
                        }

                        lProductEntity.DUpdate();
                        ProductEntityList.Add(lProductEntity);
                        CurrentProductEntityDicationary.Add(lProductEntity.ProductID, lProductEntity);
                        //ProductContext.projects.Add(lProjectEntity.Project);
                    }

                }
                catch (System.Exception ex)
                {
                    Message.ErrorMessage(ex.Message);
                }
            }
        }

        private void OnDownloadTempCommand()
        {
            String FileUrl = CustomUri.GetAbsoluteUrl("ProductmanagerFileTemp/产品导入模版.xls");

            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { "产品导入模版.xls" });
                bool? dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult != true) return;
                WebClient client = new WebClient();
                Uri uri = new Uri(FileUrl, UriKind.RelativeOrAbsolute);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
                client.OpenReadAsync(uri);
            }
            catch (System.Exception ex)
            {
                NotifyWindow notifyWindow = new NotifyWindow("下载错误", ex.Message);
                notifyWindow.Show();
            }
        }

        void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream sf = (Stream)saveFileDialog.OpenFile())
                {
                    e.Result.CopyTo(sf);
                    sf.Flush();
                    sf.Close();
                    NotifyWindow notifyWindow = new NotifyWindow("下载完成", "下载完成！");
                    notifyWindow.Show();
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("下载错误", e.Error.Message);
                notifyWindow.Show();
            }
        }

        private void OnCancelCommand()
        {
            childWindow.DialogResult = false;
        }

        private void OnOKCommand()
        {
            foreach (ProductEntity productEntity in ProductEntityList)
            {
                ProductContext.products.Add(productEntity.Product);
                ProductEntityDictionary.Add(productEntity.ProductID, productEntity);
            }

            childWindow.DialogResult = true;
        }
    }
}
