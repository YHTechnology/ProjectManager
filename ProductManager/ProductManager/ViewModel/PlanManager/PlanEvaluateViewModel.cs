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
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;

using ProductManager.Web.Service;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Model;
using ProductManager.Controls;
using ProductManager.Views.PlanManager;
using Microsoft.Windows.Data.DomainServices;
using System.ComponentModel;

namespace ProductManager.ViewModel.PlanManager
{
    [Export("PlanEvaluate")]
    public class PlanEvaluateViewModel : NotifyPropertyChanged
    {
        private PlanManagerDomainContext planManagerDomainContext = new PlanManagerDomainContext();

        public ObservableCollection<ProjectEntity> ProjectList { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> prjectSource;

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

        public ICollectionView CollectionProjectView
        {
            get { return this.projectView; }
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
                    /*if (isFilter)
                    {
                        selectProjectEntity = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }*/
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
                    /*if (isFilter)
                    {
                        selectProjectEntity = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }*/
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
                    /*if (isFilter)
                    {
                        selectProjectEntity = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }*/
                }
            }
        }

        public void LoadData()
        {
            IsBusy = true;
            planManagerDomainContext.plans.Clear();

            LoadOperation<ProductManager.Web.Model.plan> loadOperationPlan =
                planManagerDomainContext.Load<plan>(planManagerDomainContext.GetPlanQuery());
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

            planManagerDomainContext.projects.Clear();

            this.prjectSource = new EntityList<ProductManager.Web.Model.project>(this.planManagerDomainContext.projects);
            this.projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                this.LoadProjectEntities,
                this.LoadOperationProjectCompleted);
            this.projectView = new DomainCollectionView<ProductManager.Web.Model.project>(this.projectLoader, this.prjectSource);
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
            //LoadOperation<ProductManager.Web.Model.project> loadOperationProject =
            //    planManagerDomainContext.Load<project>(planManagerDomainContext.GetProjectQuery());
            //loadOperationProject.Completed += loadOperationProject_Completed;
        }

//         void loadOperationProject_Completed(object sender, EventArgs e)
//         {
//             ProjectList.Clear();
//             LoadOperation loadOperation = sender as LoadOperation;
//             foreach (ProductManager.Web.Model.project project in loadOperation.Entities)
//             {
//                 ProjectEntity projectEntity = new ProjectEntity();
//                 projectEntity.Project = project;
//                 projectEntity.Update();
//                 ProjectList.Add(projectEntity);
//             }
// 
//             UpdateChanged("ProjectList");
//             IsBusy = false;
//         }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            if (!isFilter)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.planManagerDomainContext.GetProjectQuery();
                return this.planManagerDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.planManagerDomainContext.GetProjectQuery();
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
                return this.planManagerDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project> aLoadOperation)
        {
            ProjectList.Clear();
            foreach (ProductManager.Web.Model.project project in aLoadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();
                ProjectList.Add(projectEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectList");
            this.IsBusy = false;
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
                    (OnEvaluateSingle as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }


        public ICommand OnRefash { get; private set; }

        private void OnRefashCommand()
        {
            selectProjectEntity = null;
            //ProductEntityList.Clear();
            //ProjectResponsibleEntityList.Clear();
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }

        public PlanEvaluateViewModel() 
        {
            ProjectList = new ObservableCollection<ProjectEntity>();
            departmentIdNameDictionary = new Dictionary<int, string>();

            OnEvaluateSingle = new DelegateCommand(OnEvaluateSingleCommand, CanEvaluateSingle);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);

            OnRefash = new DelegateCommand(OnRefashCommand);

            FilterList = new ObservableCollection<string>();
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";
        
        }

        public ICommand OnEvaluateSingle { get; private set; }

        public ICommand DoubleClickProject { get; private set; }

        private void DoubleClickProjectCommand()
        {
            OnEvaluateSingleCommand();
        }

        private void OnEvaluateSingleCommand()
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
                    Dictionary<string, int> accomplishRateDictionary = new Dictionary<string, int>();
                    foreach (string value in sheetNames)
                    {
                        if (null == value || accomplishRateDictionary.ContainsKey(value))
                        {
                            continue;
                        }
                        IEnumerable<plan> selectedPlans = from c in planManagerDomainContext.plans
                                                          where c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                            && c.version_id == SelectProjectEntity.PlanVersionID
                                                            && c.sheet_name == value
                                                          select c;
                        decimal totalValue = 0;
                        decimal accomplishValue = 0;
                        ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
                        bool hasOrderDate = false;
                        foreach (plan item in selectedPlans)
                        {
                            if (item.weight.HasValue)
                            {
                                totalValue += item.weight.Value;
                                if (item.accomplish_date.HasValue && item.score.HasValue)
                                {
                                    accomplishValue += item.score.Value;
                                }
                            }

                            PlanEntity planEntity = new PlanEntity();
                            planEntity.Plan = item;
                            if (item.order_date.HasValue)
                            {
                                hasOrderDate = true;
                            }
                            string getDepartmentName = string.Empty;
                            planEntity.Update();
                            planEntity.Plan = null;
                            planEntity.ProjectName = SelectProjectEntity.ProjectName;
                            if (departmentIdNameDictionary.TryGetValue(planEntity.DepartmentId, out getDepartmentName))
                            {
                                planEntity.DepartmentName = getDepartmentName;
                            }
                            planList.Add(planEntity);
                        }

                        decimal resultValue = (0 == totalValue) ? 0 : accomplishValue / totalValue;
                        int resultInt = Convert.ToInt16(Convert.ToDouble(resultValue) * 100);
                        accomplishRateDictionary.Add(value, resultInt);

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
                            planListViewModel.IsReadOnly = false;
                            planListViewModelList.Add(planListViewModel);
                        }
                    }
                    if (accomplishRateDictionary.Count > 0)
                    {
                        string title = "计划考核(";
                        title += SelectProjectEntity.ProjectName;
                        title += " ";
                        title += SelectProjectEntity.ManufactureNumber;
                        title += ")";

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

                        PlanListEvaluateWindow planEvaluateResultWindow = new PlanListEvaluateWindow(title, planListViewModelList,
                                                                                accomplishRateDictionary, planExtraEntity);
                        planEvaluateResultWindow.Closed += new EventHandler(PlanListWindow_Closed);
                        planEvaluateResultWindow.Show();
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

        private bool CanEvaluateSingle(object aObject)
        {
            return null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && null != SelectProjectEntity.ManufactureNumber
                && string.Empty != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.ManufactureNumber;
        }

        void PlanListWindow_Closed(object sender, EventArgs e)
        {
            PlanListEvaluateWindow planListWindow = sender as PlanListEvaluateWindow;
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

                            foreach (PlanEntity planEntity in item.FilterPlanList)
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
            (OnEvaluateSingle as DelegateCommand).RaiseCanExecuteChanged();
        }
    }
}
