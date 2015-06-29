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
    public class ImportImportantPartWindowViewModel : NotifyPropertyChanged
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

        public ObservableCollection<ImportantPartEntity> ImportantPartEntityList { get; set; }
        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }
        public Dictionary<int, UserEntity> DictionaryUser { get; set; }
        public ProjectEntity ProjectEntity { get; set; }

        SaveFileDialog saveFileDialog;

        public ICommand OnImport { get; private set; }
        public ICommand OnDownloadTemp { get; private set; }
        public ICommand OnCancel { get; private set; }
        public ICommand OnOK { get; private set; }
        
        public ImportImportantPartWindowViewModel(ChildWindow aChildWindow, Dictionary<String, ProjectEntity> aProjectDictionary, ProductDomainContext aProductDomainContext, Dictionary<int, UserEntity> aDictionaryUser, ProjectEntity aProjectEntity)
        {
            childWindow = aChildWindow;
            ProjectEntityDictionary = aProjectDictionary;
            ProductContext = aProductDomainContext;
            DictionaryUser = aDictionaryUser;
            ProjectEntity = aProjectEntity;

            ImportantPartEntityList = new ObservableCollection<ImportantPartEntity>();

            OnImport = new DelegateCommand(OnImportCommand);
            OnDownloadTemp = new DelegateCommand(OnDownloadTempCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            OnOK = new DelegateCommand(OnOKCommand);

        }

        private void OnImportCommand()
        {
            OpenFileDialog lOpenFile = new OpenFileDialog();
            lOpenFile.Filter = "Excel (*.xls)|*.xls";
            Dictionary<String, int> lHeaderDictionary = new Dictionary<String, int>();

            if (lOpenFile.ShowDialog() == true)
            {
                ImportantPartEntityList.Clear();

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

                        ImportantPartEntity lImportantPartEntity = new ImportantPartEntity();
                        lImportantPartEntity.ImportantPart = new ProductManager.Web.Model.important_part();
                        //lImportantPartEntity.ManufactureNumber = ProjectEntity.ManufactureNumber;


                        int lManufactureNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("生产令号", out lManufactureNumberColumn)
                                && -1 != lManufactureNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lManufactureNumberColumn)))
                            {
                                lImportantPartEntity.ManufactureNumber = cell.StringValue;

                                ProjectEntity projectEntityTemp;
                                if (!ProjectEntityDictionary.TryGetValue(lImportantPartEntity.ManufactureNumber, out projectEntityTemp))
                                {
                                    NotifyWindow lNotifyWindow = new NotifyWindow("错误", "第 " + rowPair.Key.ToString() + "行 系统中没有生产令号：" + lImportantPartEntity.ManufactureNumber);
                                    lNotifyWindow.Show();
                                    return;
                                }
                            }
                            else
                            {
                                NotifyWindow lNotifyWindow = new NotifyWindow("错误", "第 " + rowPair.Key.ToString() + "行 系统中没有生产令号：" + lImportantPartEntity.ManufactureNumber);
                                lNotifyWindow.Show();
                                return;
                            }
                        }

                        int lImportPartNameColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("主要部件名称", out lImportPartNameColumn)
                                && -1 != lManufactureNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lImportPartNameColumn)))
                            {
                                lImportantPartEntity.ImportantPartName = cell.StringValue;
                            }
                        }

                        int lProjectNameColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("厂家", out lProjectNameColumn)
                                && -1 != lProjectNameColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lProjectNameColumn)))
                            {
                                lImportantPartEntity.ImportantPartManufacturers = cell.StringValue;
                            }
                        }

                        int lModelNumberColumn = -1;
                        {
                            Cell cell = Cell.EmptyCell;
                            if (lHeaderDictionary.TryGetValue("到货时间", out lModelNumberColumn)
                                && -1 != lModelNumberColumn
                                && Cell.EmptyCell != (cell = rowPair.Value.GetCell(lModelNumberColumn)))
                            {
                                lImportantPartEntity.AriveTime = cell.DateTimeValue;
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
                                lImportantPartEntity.Note = cell.StringValue;
                            }
                        }

                        App app = Application.Current as App;
                        lImportantPartEntity.AriveUserId = app.UserInfo.UserID;
                        lImportantPartEntity.AriveInputTime = DateTime.Now;
                        lImportantPartEntity.UserEntityDictionary = DictionaryUser;
                        lImportantPartEntity.DUpdate();

                        ImportantPartEntityList.Add(lImportantPartEntity);

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
            String FileUrl = CustomUri.GetAbsoluteUrl("ProductmanagerFileTemp/重要部件到货信息导入模版.xls");

            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { "重要部件到货信息导入模版.xls" });
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
            foreach (ImportantPartEntity importantPartEntity in ImportantPartEntityList)
            {
                ProductContext.important_parts.Add(importantPartEntity.ImportantPart);
            }

            childWindow.DialogResult = true;
        }
    }
}
