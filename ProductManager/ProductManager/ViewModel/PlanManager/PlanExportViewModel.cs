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
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;

using ProductManager.Web.Service;
using ProductManager.ViewData.Entity;
using ProductManager.Views.PlanManager;
using ProductManager.Controls;
using ProductManager.Web.Model;

namespace ProductManager.ViewModel.PlanManager
{
    [Export("PlanExport")]
    public class PlanExportViewModel : NotifyPropertyChanged
    {
        private PlanManagerDomainContext planManagerDomainContext = new PlanManagerDomainContext();

        public ObservableCollection<ProjectEntity> ProjectList { get; set; }

        private Dictionary<int, string> departmentIdNameDictionary { get; set; }

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

        public void LoadData()
        {
            IsBusy = true;
            planManagerDomainContext.plans.Clear();
            LoadOperation<ProductManager.Web.Model.plan> loadOperationPlan =
                planManagerDomainContext.Load<ProductManager.Web.Model.plan>(planManagerDomainContext.GetPlanQuery());
            loadOperationPlan.Completed += loadOperationPlan_Completed;
        }

        void loadOperationPlan_Completed(object sender, EventArgs e)
        {
            planManagerDomainContext.plan_extras.Clear();
            LoadOperation<ProductManager.Web.Model.plan_extra> loadOperationPlanExtra =
                planManagerDomainContext.Load<ProductManager.Web.Model.plan_extra>(planManagerDomainContext.GetPlan_extraQuery());
            loadOperationPlanExtra.Completed += loadOperationPlanExtra_Completed;
        }

        void loadOperationPlanExtra_Completed(object sender, EventArgs e)
        {
            planManagerDomainContext.departments.Clear();
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                planManagerDomainContext.Load<ProductManager.Web.Model.department>(planManagerDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            departmentIdNameDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                departmentIdNameDictionary.Add(departmentEntity.DepartmentID, departmentEntity.DepartmentName);
            }

            IsBusy = true;

            planManagerDomainContext.projects.Clear();
            LoadOperation<ProductManager.Web.Model.project> loadOperationProject =
                planManagerDomainContext.Load<project>(planManagerDomainContext.GetProjectQuery());
            loadOperationProject.Completed += loadOperationProject_Completed;
        }

        void loadOperationProject_Completed(object sender, EventArgs e)
        {
            ProjectList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.project project in loadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();
                ProjectList.Add(projectEntity);
            }

            UpdateChanged("ProjectList");
            IsBusy = false;
        }

        private ProjectEntity selectProjectEntity = null;
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
                    (OnExportPlan as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public PlanExportViewModel()
        {
            ProjectList = new ObservableCollection<ProjectEntity>();

            departmentIdNameDictionary = new Dictionary<int, string>();

            OnExportPlan = new DelegateCommand(OnExportPlanCommand, CanExportPlan);
        }

        public ICommand OnExportPlan { get; private set; }

        private void OnExportPlanCommand()
        {
            IsBusy = true;

            if (null == SelectProjectEntity.PlanVersionID
                || null == SelectProjectEntity.ManufactureNumber
                || string.Empty == SelectProjectEntity.PlanVersionID
                || string.Empty == SelectProjectEntity.ManufactureNumber)
            {
                Message.ErrorMessage("生产令号或版本号无效");
            }
            else
            {
                IEnumerable<string> sheetNames = from c in planManagerDomainContext.plans
                                                 where c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                    && c.version_id == SelectProjectEntity.PlanVersionID
                                                 select c.sheet_name;
                if (sheetNames.Count() > 0)
                {
                    ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                    ObservableCollection<string> differentSheets = new ObservableCollection<string>();
                    foreach (string value in sheetNames)
                    {
                        if (null == value || differentSheets.Contains(value))
                        {
                            continue;
                        }
                        differentSheets.Add(value);
                        IEnumerable<plan> selectedPlans = from c in planManagerDomainContext.plans
                                                          where c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                            && c.version_id == SelectProjectEntity.PlanVersionID
                                                            && c.sheet_name == value
                                                          select c;
                        ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
                        bool hasOrderDate = false;
                        foreach (plan item in selectedPlans)
                        {
                            PlanEntity planEntity = new PlanEntity();
                            planEntity.Plan = item;
                            if (item.order_date.HasValue)
                            {
                                hasOrderDate = true;
                            }
                            string getDepartmentName = string.Empty;
                            planEntity.Update();
                            planEntity.ProjectName = SelectProjectEntity.ProjectName;
                            if (departmentIdNameDictionary.TryGetValue(planEntity.DepartmentId, out getDepartmentName))
                            {
                                planEntity.DepartmentName = getDepartmentName;
                            }
                            planList.Add(planEntity);
                        }
                        if (planList.Count > 0)
                        {
                            int modelIndex = -1;
                            if ("采购节点" == planList[0].SheetName)
                            {
                                if (hasOrderDate)
                                {
                                    modelIndex = 1;
                                }
                                else
                                {
                                    modelIndex = 2;
                                }
                            }
                            else
                            {
                                modelIndex = 0;
                            }                            

                            PlanListViewModel planListViewModel = new PlanListViewModel(value, planList,
                                                                        modelIndex, departmentIdNameDictionary);
                            planListViewModel.IsReadOnly = true;
                            planListViewModelList.Add(planListViewModel);
                        }
                    }
                    if (planListViewModelList.Count > 0)
                    {
                        PlanExtraEntity planExtraEntity = null;
                        IEnumerable<plan_extra> plan_extras = from c in planManagerDomainContext.plan_extras
                                                              where c.version_id == SelectProjectEntity.PlanVersionID
                                                              && c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                              select c;
                        if (0 != plan_extras.Count())
                        {
                            planExtraEntity = new PlanExtraEntity();
                            planExtraEntity.PlanExtra = plan_extras.First<plan_extra>();
                            planExtraEntity.Update();
                            planExtraEntity.PlanExtra = null;
                        }
                        PlanListEditWindow planListWindow = new PlanListEditWindow("计划导出", "导出", SelectProjectEntity.PlanVersionID,
                                                                                planListViewModelList, planExtraEntity);
                        planListWindow.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
                        planListWindow.Closed += new EventHandler(PlanListWindow_Closed);
                        planListWindow.Show();
                    }
                }
                else
                {
                    string errorMessage = "无相关数据(生产令号：" +
                                            SelectProjectEntity.ManufactureNumber.TrimEnd() +
                                            ",版本号" +
                                            SelectProjectEntity.PlanVersionID
                                            + ")";
                    Message.ErrorMessage(errorMessage);
                }
            }

            IsBusy = false;
        }

        private bool CanExportPlan(object aObject)
        {
            return null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && null != SelectProjectEntity.ManufactureNumber
                && string.Empty != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.ManufactureNumber;
        }

        void PlanListWindow_Closed(object sender, EventArgs e)
        {
            PlanListEditWindow planListWindow = sender as PlanListEditWindow;
            if (planListWindow.DialogResult == true)
            {
                SaveFileDialog sDialog = new SaveFileDialog();
                sDialog.Filter = "Excel Files(*.xls)|*.xls";

                if (sDialog.ShowDialog() == true)
                {
                    try
                    {
                        string versionId = "文件编号：";
                        if (null != planListWindow.planExtraEntity && null != planListWindow.planExtraEntity.FileId)
                        {
                            versionId += planListWindow.planExtraEntity.FileId;
                        }
                        versionId += " 计划版本：";
                        versionId += SelectProjectEntity.PlanVersionID;
                        string projectNameKey = SelectProjectEntity.ProjectName + "        ";
                        string manufactureNumber = "生产令号：" + SelectProjectEntity.ManufactureNumber;

                        Workbook workbook = new Workbook();
                        ColumnModel columnModel = new ColumnModel();
                        foreach (PlanListViewModel item in planListWindow.planListViewModelList)
                        {
                            string projectNameName = projectNameKey + item.Title;

                            Worksheet worksheet = new Worksheet(item.Title);

                            Int16 RowCount = 0;
                            worksheet.Cells[RowCount++, 0] = new Cell(versionId);
                            worksheet.Cells[RowCount++, 0] = new Cell(projectNameName);
                            worksheet.Cells[RowCount++, 0] = new Cell(manufactureNumber);

                            int columnCount = 0;
                            foreach (string itemColumn in columnModel.List[item.ColumnModelIndex])
                            {
                                worksheet.Cells[RowCount, columnCount++] = new Cell(itemColumn);
                            }
                            ++RowCount;

                            foreach (PlanEntity planEntity in item.PlanList)
                            {
                                columnCount = 0;
                                string value = Convert.ToString(planEntity.SequenceId);
                                worksheet.Cells[RowCount, columnCount++] = new Cell(value);

                                worksheet.Cells[RowCount, columnCount++] = new Cell(planEntity.ComponentName);
                                worksheet.Cells[RowCount, columnCount++] = new Cell(planEntity.TaskDescription);

                                value = Convert.ToString(planEntity.Weight);
                                worksheet.Cells[RowCount, columnCount++] = new Cell(value);

                                if (planEntity.Score.HasValue)
                                {
                                    value = Convert.ToString(planEntity.Score.Value);
                                    worksheet.Cells[RowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (1 == item.ColumnModelIndex)
                                {
                                    if (planEntity.OrderDate.HasValue)
                                    {
                                        value = Convert.ToString(planEntity.OrderDate.Value);
                                        worksheet.Cells[RowCount, columnCount] = new Cell(value);
                                    }
                                    ++columnCount;
                                }

                                value = Convert.ToString(planEntity.TargetDate);
                                worksheet.Cells[RowCount, columnCount++] = new Cell(value);

                                if (planEntity.TargetDateAdjustment1.HasValue)
                                {
                                    value = Convert.ToString(planEntity.TargetDateAdjustment1.Value);
                                    worksheet.Cells[RowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (planEntity.TargetDateAdjustment2.HasValue)
                                {
                                    value = Convert.ToString(planEntity.TargetDateAdjustment2.Value);
                                    worksheet.Cells[RowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (planEntity.AccomplishDate.HasValue)
                                {
                                    value = Convert.ToString(planEntity.AccomplishDate.Value);
                                    worksheet.Cells[RowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (null != planEntity.DepartmentName && string.Empty != planEntity.DepartmentName)
                                {
                                    worksheet.Cells[RowCount, columnCount] = new Cell(planEntity.DepartmentName);
                                }
                                ++columnCount;

                                if (null != planEntity.Remark && string.Empty != planEntity.Remark)
                                {
                                    worksheet.Cells[RowCount, columnCount] = new Cell(planEntity.Remark);
                                }
                                ++columnCount;

                                ++RowCount;
                            }

                            workbook.Worksheets.Add(worksheet);
                        }
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
            (OnExportPlan as DelegateCommand).RaiseCanExecuteChanged();
        }
    }
}
