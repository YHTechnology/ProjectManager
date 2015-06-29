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
    [Export("ResponsiblePersonList")]
    public class ResponsiblePersonListViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext = new ProductDomainContext();

        public ObservableCollection<ResponsiblePersonEntity> ResponsiblePersonEntityList { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> prjectSource;

        private DomainCollectionView<ProductManager.Web.Model.project_responsible> projectResponsibleView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible> projectResponsibleLoader;
        private EntityList<ProductManager.Web.Model.project_responsible> projectResponsibleSource;

        private Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        public bool IsBusy { get; set; }

        private bool isFilter = false;
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
        public String SelectList { get; set; }
        public bool IsNotOutput { get; set; }
        public String FilterContent { get; set; }

        public ICommand OnRefash { get; set; }
        public ICommand OnExport { get; set; }

        public ResponsiblePersonListViewModel()
        {
            ResponsiblePersonEntityList = new ObservableCollection<ResponsiblePersonEntity>();
            ProjectEntityDictionary = new Dictionary<String, ProjectEntity>();
            FilterList = new ObservableCollection<String>();

            IsBusy = false;
            IsFilter = false;
            OnRefash = new DelegateCommand(OnReflashCommand);
            OnExport = new DelegateCommand(OnExportCommand);

            FilterList.Add("项目负责人");
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
        }

        public void LoadData()
        {
            prjectSource = new EntityList<Web.Model.project>(ProductDomainContext.projects);
            projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                LoadProjectEntities,
                LoadOperationProjectCompleted
                );
            projectView = new DomainCollectionView<ProductManager.Web.Model.project>(projectLoader, prjectSource);

            projectResponsibleSource = new EntityList<Web.Model.project_responsible>(ProductDomainContext.project_responsibles);
            projectResponsibleLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project_responsible>(
                LoadProjectResponsibleEntities,
                LoadProjectResponsibleCompleted
                );
            projectResponsibleView = new DomainCollectionView<ProductManager.Web.Model.project_responsible>(projectResponsibleLoader, projectResponsibleSource);

            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            EntityQuery<ProductManager.Web.Model.project> lQuery;
            if (IsFilter && SelectList == "项目负责人")
            {
                lQuery = this.ProductDomainContext.GetProjectByRespPersonNameQuery(FilterContent);
            }
            else
            {
                lQuery = this.ProductDomainContext.GetProjectByRespPersonNameQuery("");
            }

            if (IsFilter)
            {
                if (SelectList == "生产令号")
                {
                    lQuery = lQuery.Where(c => c.manufacture_number.ToLower().Contains(FilterContent.ToLower()));
                }

                if (SelectList == "项目名称")
                {
                    lQuery = lQuery.Where(c => c.project_name.ToLower().Contains(FilterContent.ToLower()));
                }
            }


            if (IsNotOutput)
            {
                lQuery = lQuery.Where(c => !c.delivery_time.HasValue);
            }
            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project> aLoadOperation)
        {
            ResponsiblePersonEntityList.Clear();
            ProjectEntityDictionary.Clear();

            foreach (ProductManager.Web.Model.project project in aLoadOperation.Entities)
            {
                ProjectEntity lProjectEntity = new ProjectEntity();
                lProjectEntity.Project = project;
                lProjectEntity.Update();
                ProjectEntityDictionary.Add(lProjectEntity.ManufactureNumber, lProjectEntity);
            }

            using (projectResponsibleView.DeferRefresh())
            {
                projectResponsibleView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.project_responsible> LoadProjectResponsibleEntities()
        {
            EntityQuery<ProductManager.Web.Model.project_responsible> lQuery = this.ProductDomainContext.GetProject_responsibleQuery();
            if (IsFilter && SelectList == "项目负责人")
            {
                lQuery = lQuery.Where(c => c.responsible_persionName.ToLower().Contains(FilterContent.ToLower()));
            }
            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectResponsibleView));
        }

        private void LoadProjectResponsibleCompleted(LoadOperation<ProductManager.Web.Model.project_responsible> aLoadOperation)
        {
            foreach (ProductManager.Web.Model.project_responsible project_responsible in aLoadOperation.Entities)
            {
                ResponsiblePersonEntity lResponsiblePersionEntity = new ResponsiblePersonEntity();
                lResponsiblePersionEntity.ResponsiblePerson = project_responsible.responsible_persionName;
                lResponsiblePersionEntity.ManufactureNumber = project_responsible.manufacture_number;
                lResponsiblePersionEntity.ProjectNote = project_responsible.descript;
                ProjectEntity lProjectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(lResponsiblePersionEntity.ManufactureNumber, out lProjectEntityTemp))
                {
                    lResponsiblePersionEntity.ProjectName = lProjectEntityTemp.ProjectName;
                    lResponsiblePersionEntity.RecoderDateTime = lProjectEntityTemp.RecordDate;
                    lResponsiblePersionEntity.OutputDateTime = lProjectEntityTemp.DeliveryTime;
                    //lResponsiblePersionEntity.ProjectNote = lProjectEntityTemp.Remark;
                    ResponsiblePersonEntityList.Add(lResponsiblePersionEntity);
                }
            }
        }

        private void OnReflashCommand()
        {
            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
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
                    Worksheet worksheet = new Worksheet("负责人汇总");

                    //Title
                    worksheet.Cells[0, 0] = new Cell("项目负责人");
                    worksheet.Cells[0, 1] = new Cell("生产令号");
                    worksheet.Cells[0, 2] = new Cell("项目名称");
                    worksheet.Cells[0, 3] = new Cell("项目备注");
                    worksheet.Cells[0, 4] = new Cell("记录时间");
                    worksheet.Cells[0, 5] = new Cell("发运完成时间");

                    Int16 RowCount = 1;

                    foreach (ResponsiblePersonEntity responsiblePersonEntity in ResponsiblePersonEntityList)
                    {
                        int columnCount = 0;

                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.ResponsiblePerson);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.ManufactureNumber);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.ProjectName);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.ProjectNote);
                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.RecoderDateTime.HasValue ? responsiblePersonEntity.RecoderDateTime.Value.ToShortDateString() : "");
                        worksheet.Cells[RowCount, columnCount++] = new Cell(responsiblePersonEntity.OutputDateTime.HasValue ? responsiblePersonEntity.OutputDateTime.Value.ToShortDateString() : "");
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
    }
}
