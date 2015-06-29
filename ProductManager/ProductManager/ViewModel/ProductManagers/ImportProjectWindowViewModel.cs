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
    public class ImportProjectWindowViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductContext;
        private ChildWindow childWindow;

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

        private String importFileName;
        public String ImportFileName
        {
            get
            {
                return importFileName;
            }
            set
            {
                if (importFileName != value)
                {
                    importFileName = value;
                    UpdateChanged("ImportFileName");
                }
            }
        }

        public ObservableCollection<ProjectEntity> ProjectImportEntityList { get; set; }
        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        public ICommand OnImport { get; private set; }
        public ICommand OnDownloadTemp { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnOK { get; private set; }

        private SaveFileDialog saveFileDialog;

        public ImportProjectWindowViewModel(ChildWindow aChildWindow, ProductDomainContext aProductDemainContext, Dictionary<String, ProjectEntity> aProjectEntityDictionary)
        {
            childWindow = aChildWindow;
            ProductContext = aProductDemainContext;
            ProjectEntityDictionary = aProjectEntityDictionary;
            ProjectImportEntityList = new ObservableCollection<ProjectEntity>();

            OnImport = new DelegateCommand(OnImportCommand);
            OnDownloadTemp = new DelegateCommand(OnDownloadTempCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            OnOK = new DelegateCommand(OnOKCommand);
        }


        private void OnImportCommand()
        {
            ProjectImportEntityList.Clear();

            OpenFileDialog lOpenFile = new OpenFileDialog();

            lOpenFile.Filter = "Excel (*.xls)|*.xls";

            Dictionary<String, int> lHeaderDictionary = new Dictionary<String, int>();

            if (lOpenFile.ShowDialog() == true)
            {
                ProjectImportEntityList.Clear();

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

                        ProjectEntity lProjectEntity = new ProjectEntity();
                        lProjectEntity.Project = new ProductManager.Web.Model.project();
                        // 生产令号
                        int lManufactureNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("生产令号", out lManufactureNumberColumn)
                                && -1 != lManufactureNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lManufactureNumberColumn)))
                            {
                                lProjectEntity.ManufactureNumber = cell.StringValue;
                            }
                        }

                        // 项目名称
                        int lProjectNameColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("项目名称", out lProjectNameColumn)
                                && -1 != lProjectNameColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lProjectNameColumn)))
                            {
                                lProjectEntity.ProjectName = cell.StringValue;
                            }
                        }

                        //型号
                        int lModelNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("型号", out lModelNumberColumn)
                                && -1 != lModelNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lModelNumberColumn)))
                            {
                                lProjectEntity.ModelNumber = cell.StringValue;
                            }
                        }

                        //备注
                        int lRemarkColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("备注", out lRemarkColumn)
                                && -1 != lRemarkColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lRemarkColumn)))
                            {
                                lProjectEntity.Remark = cell.StringValue;
                            }
                        }

                        //年份
                        int lYearNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("年份", out lYearNumberColumn)
                                && -1 != lYearNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lYearNumberColumn)))
                            {
                                lProjectEntity.YearNumber = Convert.ToInt32(cell.StringValue);
                            }
                        }

                        lProjectEntity.RecordDate = DateTime.Now;
                        lProjectEntity.DUpdate();
                        
                        if (lProjectEntity.ManufactureNumber.Length == 0)
                        {
                            Message.ErrorMessage("生产令号不能为空 在第 " + rowPair.Key + " 行！");
                            break;
                        }

                        ProjectEntity lProjectEntityTemp;
                        if (ProjectEntityDictionary.TryGetValue(lProjectEntity.ManufactureNumber, out lProjectEntityTemp))
                        {
                            Message.ErrorMessage("生产令号" + lProjectEntity.ManufactureNumber + "重复 在第 " + rowPair.Key + " 行！");
                            break;
                        }

                        ProjectImportEntityList.Add(lProjectEntity);

                        //ProjectEntityDictionary.Add(lProjectEntity.ManufactureNumber, lProjectEntity);
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
            String FileUrl = CustomUri.GetAbsoluteUrl("ProductmanagerFileTemp/生产令号导入模版.xls");
            
            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { "生产令号导入模版.xls" });
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
            foreach (ProjectEntity lProjectEntity in ProjectImportEntityList)
            {
                ProjectEntityDictionary.Add(lProjectEntity.ManufactureNumber, lProjectEntity);
                ProductContext.projects.Add(lProjectEntity.Project);
            }
            childWindow.DialogResult = true;
        }
    }
}
