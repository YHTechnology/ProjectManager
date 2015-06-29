using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using ProductManager.Web.Service;
using System.Collections.ObjectModel;
using ProductManager.ViewData.Entity;
using System.Collections.Generic;
using ProductManager.Controls;
using ProductManager.Views.PlanManager;
using System.IO;
using Lite.ExcelLibrary.SpreadSheet;
using ProductManager.Web.Model;
using System.Linq;
using Microsoft.Windows.Data.DomainServices;
using System.ComponentModel;
using ProductManager.FileUploader;

namespace ProductManager.ViewModel.PlanManager
{
    [Export("PlanEdit")]
    public class PlanEditViewModel : NotifyPropertyChanged
    {
        private PlanManagerDomainContext planManagerDomainContext = new PlanManagerDomainContext();
        private ProductDomainContext ProductDomainContext = new ProductDomainContext();

        public ObservableCollection<ProjectEntity> ProjectList { get; set; }

        private DomainCollectionView<project> projectView;
        private DomainCollectionViewLoader<project> projectLoader;
        private EntityList<project> prjectSource;

        private DomainCollectionView<plan> planView;
        private DomainCollectionViewLoader<plan> planLoader;
        private EntityList<plan> panSource;

        private Dictionary<string, int> departmentNameIdDictionary;
        private Dictionary<int, string> departmentIdNameDictionary;

        private Dictionary<string, UserProjectEntity> UserProjectEntityDictionary { get; set; }
        private ObservableCollection<string> importErrorList { get; set; }

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

        public void LoadData()
        {
            IsBusy = true;
            ProductDomainContext.user_projects.Clear();
            planManagerDomainContext.plans.Clear();

            this.panSource = new EntityList<plan>(this.planManagerDomainContext.plans);
            this.planLoader = new DomainCollectionViewLoader<plan>(
                this.LoadPlanEntities,
                this.LoadOperationPlanCompleted);
            this.planView = new DomainCollectionView<plan>(this.planLoader, this.panSource);

            App app = Application.Current as App;
            LoadOperation<user_project> loadOperationUserProject =
               ProductDomainContext.Load<user_project>(ProductDomainContext.GetUserProjectQuery(app.UserInfo.UserID));
            loadOperationUserProject.Completed += LoadUserProjectComplete;
        }

        void LoadUserProjectComplete(object sender, EventArgs e)
        {
            UserProjectEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (user_project user_project in loadOperation.Entities)
            {
                UserProjectEntity lUserProjectEntity = new UserProjectEntity();
                lUserProjectEntity.UserProject = user_project;
                lUserProjectEntity.Update();
                UserProjectEntityDictionary.Add(lUserProjectEntity.ManufactureNumber, lUserProjectEntity);
            }

            planManagerDomainContext.plan_extras.Clear();
            LoadOperation<plan_extra> loadOperationPlanExtra =
                planManagerDomainContext.Load<plan_extra>(planManagerDomainContext.GetPlan_extraQuery());
            loadOperationPlanExtra.Completed += loadOperationPlanExtra_Completed;
        }

        private LoadOperation<plan> LoadPlanEntities()
        {
            this.IsBusy = true;
            EntityQuery<plan> lQuery = this.planManagerDomainContext.GetPlanQuery();
            if (SelectProjectEntity != null)
            {
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
            }
            return this.planManagerDomainContext.Load(lQuery.SortAndPageBy(this.planView));
        }

        private void LoadOperationPlanCompleted(LoadOperation<plan> aLoadOperation)
        {
            IsBusy = false;
        }

        void loadOperationPlan_Completed(object sender, EventArgs e)
        {
           
        }

        void loadOperationPlanExtra_Completed(object sender, EventArgs e)
        {
            planManagerDomainContext.departments.Clear();
            LoadOperation<department> loadOperationDepartment =
                planManagerDomainContext.Load<department>(planManagerDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            departmentNameIdDictionary.Clear();
            departmentIdNameDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (department department in loadOperation.Entities)
            {
                if (!departmentNameIdDictionary.Keys.Contains(department.department_name))
                {
                    departmentNameIdDictionary.Add(department.department_name, department.department_id);
                }
                departmentIdNameDictionary.Add(department.department_id, department.department_name);
            }

            planManagerDomainContext.projects.Clear();

            this.prjectSource = new EntityList<project>(this.planManagerDomainContext.projects);
            this.projectLoader = new DomainCollectionViewLoader<project>(
                this.LoadProjectEntities,
                this.LoadOperationProjectCompleted);
            this.projectView = new DomainCollectionView<project>(this.projectLoader, this.prjectSource);
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }


        private LoadOperation<project> LoadProjectEntities()
        {
            if (!isFilter)
            {
                this.IsBusy = true;
                EntityQuery<project> lQuery = this.planManagerDomainContext.GetProjectQuery();
                return this.planManagerDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<project> lQuery = this.planManagerDomainContext.GetProjectQuery();
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

        private void LoadOperationProjectCompleted(LoadOperation<project> aLoadOperation)
        {
            ProjectList.Clear();
            foreach (project project in aLoadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();

                if (!String.IsNullOrEmpty(projectEntity.PlanVersionID))
                {
                    IEnumerable<plan_extra> plan_extras = from c in planManagerDomainContext.plan_extras
                                                          where c.version_id == projectEntity.PlanVersionID
                                                            && c.manufacture_number == projectEntity.ManufactureNumber
                                                          select c;
                    if (0 != plan_extras.Count())
                    {
                        plan_extra planExtra = plan_extras.First<plan_extra>();
                        projectEntity.CompileUserName = planExtra.compile_user_name;
                        projectEntity.CompileDate = planExtra.compile_date;
                    }

                    IEnumerable<string> planVersions = from c in planManagerDomainContext.plans
                                                       where c.manufacture_number == projectEntity.ManufactureNumber
                                                       select c.version_id;
                    if (0 != planVersions.Count())
                    {
                        projectEntity.PlanVersionDictionary = new Dictionary<string, string>();
                        foreach (string item in planVersions)
                        {
                            if (projectEntity.PlanVersionDictionary.ContainsKey(item))
                            {
                                continue;
                            }
                            projectEntity.PlanVersionDictionary.Add(item, item);
                        }
                        projectEntity.HasHistory = true;
                    }
                }

                UserProjectEntity lUserProjectEntity;
                if (UserProjectEntityDictionary.TryGetValue(projectEntity.ManufactureNumber, out lUserProjectEntity))
                {
                    projectEntity.UserProjectEntity = lUserProjectEntity;
                    projectEntity.SetIsUserProject(true);
                }

                projectEntity.UserProjectEntityDictionary = UserProjectEntityDictionary;
                if (IsUserProject && !projectEntity.IsUserProject)
                {
                    continue;
                }

                ProjectList.Add(projectEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectList");
            this.IsBusy = false;
        }
        /*
        void loadOperationProject_Completed(object sender, EventArgs e)
        {
            ProjectList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (project project in loadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();
                ProjectList.Add(projectEntity);
            }
         
            UpdateChanged("ProjectList");
            IsBusy = false;
        }
        */
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
                    (OnFirstImport as DelegateCommand).RaiseCanExecuteChanged();
                    (OnSmallChange as DelegateCommand).RaiseCanExecuteChanged();
                    (OnImportAndChange as DelegateCommand).RaiseCanExecuteChanged();

                    using(planView.DeferRefresh())
                    {
                        this.projectView.MoveToFirstPage();
                    }
                }
            }
        }

        public PlanEditViewModel() 
        {
            ProjectList = new ObservableCollection<ProjectEntity>();
            departmentNameIdDictionary = new Dictionary<string, int>();
            departmentIdNameDictionary = new Dictionary<int, string>();
            UserProjectEntityDictionary = new Dictionary<string, UserProjectEntity>();
            importErrorList = new ObservableCollection<string>();

            OnFirstImport = new DelegateCommand(OnFirstImportCommand, CanFirstImport);
            OnSmallChange = new DelegateCommand(OnSmallChangeCommand, CanChange);
            OnImportAndChange = new DelegateCommand(OnImportAndChangeCommand, CanChange);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);
            OnRefash = new DelegateCommand(OnRefashCommand);
            OnDownloadTemp = new DelegateCommand(OnDownloadTempCommand);

            FilterList = new ObservableCollection<string>();
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";
        }

        private int ExcelSheetValidation(Worksheet aSheet, 
                                        ref int aFirstValueRow, 
                                        ref string aManufactureName, 
                                        ref string aVersionId,
                                        ref string aFileId,
                                        ref string aRequirement, 
                                        ref Dictionary<string, int> aMatchedColumnDictionary)
        {
            aFirstValueRow = -1;
            ColumnModel columnModel = new ColumnModel();
            foreach (KeyValuePair<int, Row> rowPair in aSheet.Cells.Rows)
            {
                try
                {
                    if (rowPair.Value.LastColIndex < columnModel.MinSize - 1)
                    {
                        Cell cell = rowPair.Value.GetCell(rowPair.Value.FirstColIndex);
                        if (Cell.EmptyCell != cell)
                        {
                            string firstValue = cell.StringValue;
                            string manufactureNumberKey = "生产令号：";
                            string versionIdKey1 = "计划版本：";
                            string versionIdKey2 = "计划版本:";   
                            string versionIdKey = string.Empty;
                            int pos = -1;
                            if (-1 != (pos = firstValue.IndexOf(versionIdKey1)))
                            {
                                versionIdKey = versionIdKey1;
                            }
                            else if( -1 != (pos = firstValue.IndexOf(versionIdKey2)))
                            {
                                versionIdKey = versionIdKey2;
                            }

                            if (versionIdKey.Count() > 0)
                            {
                                int versionPos = pos + versionIdKey.Length;
                                aVersionId = firstValue.Substring(versionPos);
                                aVersionId = aVersionId.Trim();
                                string fileIdKey = "文件编号：";
                                int fileIdStartPos = firstValue.IndexOf(fileIdKey);
                                if (-1 != fileIdStartPos)
                                {
                                    fileIdStartPos += fileIdKey.Length;
                                    aFileId = firstValue.Substring(fileIdStartPos, versionPos - fileIdStartPos - versionIdKey.Count());
                                    aFileId = aFileId.Trim();
                                }
                            }
                            else if (-1 != (pos = firstValue.IndexOf(manufactureNumberKey)) && pos < firstValue.Length)
                            {
                                pos += manufactureNumberKey.Length;
                                const char space = ' ';
                                while (space == firstValue[pos])
                                {
                                    ++pos;
                                }

                                int end = firstValue.IndexOf(space, pos);
                                aManufactureName = firstValue.Substring(pos, end - pos);
                                aManufactureName = aManufactureName.Trim();
                                aRequirement = firstValue.Substring(end);
                                aRequirement = aRequirement.Trim();
                            }
                        }
                        continue;
                    }
                }
                catch(Exception e)
                {
                    //empty line
                    continue;
                }
                foreach (ObservableCollection<string> item in columnModel.List)
                {
                    int matchCount = 0;
                    aMatchedColumnDictionary.Clear();
                    for (int column = rowPair.Value.FirstColIndex; column <= rowPair.Value.LastColIndex; ++column)
                    {
                        Cell cell = rowPair.Value.GetCell(column);
                        if (Cell.EmptyCell != cell && item.Contains(cell.StringValue))
                        {
                            aMatchedColumnDictionary.Add(cell.StringValue, column);
                            ++matchCount;
                        }
                    }
                    if (item.Count == matchCount)
                    {
                        aFirstValueRow = rowPair.Key + 1;
                        return columnModel.List.IndexOf(item);
                    }
                }  
            }

            return -1;
        }

        private bool CellToPlanExtraEntity(Row aRow, string aManufactureName, 
                                        string aFileId, string aRequirement, ref PlanExtraEntity aPlanExtraEntity)
        {
            bool returnValue = false;
            try
            {
                Cell cell = aRow.GetCell(aRow.FirstColIndex);
                if (Cell.EmptyCell != cell)
                {
                    returnValue = true;
                    string firstValue = cell.StringValue;
                    int pos = -1;
                    string compilationBasisKey = "编制依据：";
                    string reasonAdjustment1Key = "第一次调整原因：";
                    string reasonAdjustment2Key = "第二次调整原因：";
                    string compileUser = "编制：";
                    string date = "日期：";
                    if (-1 != (pos = firstValue.IndexOf(compilationBasisKey)))
                    {
                        if (null == aPlanExtraEntity)
                        {
                            aPlanExtraEntity = new PlanExtraEntity();
                            aPlanExtraEntity.ManufactureNumber = aManufactureName;
                            aPlanExtraEntity.FileId = aFileId;
                            aPlanExtraEntity.Requirement = aRequirement;
                        }
                        pos += compilationBasisKey.Count();
                        aPlanExtraEntity.CompilationBasis = firstValue.Substring(pos);
                        aPlanExtraEntity.CompilationBasis = aPlanExtraEntity.CompilationBasis.Trim();
                    }
                    else if (-1 != (pos = firstValue.IndexOf(reasonAdjustment1Key)) && null != aPlanExtraEntity)
                    {
                        pos += reasonAdjustment1Key.Count();
                        aPlanExtraEntity.ReasonAdjustment1 = firstValue.Substring(pos);
                        aPlanExtraEntity.ReasonAdjustment1 = aPlanExtraEntity.ReasonAdjustment1.Trim();
                    }
                    else if (-1 != (pos = firstValue.IndexOf(reasonAdjustment2Key)) && null != aPlanExtraEntity)
                    {
                        pos += reasonAdjustment2Key.Count();
                        aPlanExtraEntity.ReasonAdjustment2 = firstValue.Substring(pos);
                        aPlanExtraEntity.ReasonAdjustment2 = aPlanExtraEntity.ReasonAdjustment2.Trim();
                    }
                    else if (-1 != (pos = firstValue.IndexOf(compileUser)) && null != aPlanExtraEntity)
                    {
                        int startRow = aRow.FirstColIndex + 1;
                        Cell compieUserCell = aRow.GetCell(startRow);
                        if (Cell.EmptyCell != compieUserCell && null != compieUserCell.StringValue)
                        {
                            aPlanExtraEntity.CompileUserName = compieUserCell.StringValue.Trim();
                        }
                        string examineUser = "审核：";
                        string approveUser = "批准：";
                        for (int column = startRow + 1; column <= aRow.LastColIndex; ++column)
                        {
                            Cell elseCell = aRow.GetCell(column);
                            if (Cell.EmptyCell != elseCell && null != elseCell.StringValue && string.Empty != elseCell.StringValue)
                            {
                                int elsePos = -1;
                                if (-1 != ( elsePos = elseCell.StringValue.IndexOf(examineUser)))
                                {
                                    ++column;
                                    elseCell = aRow.GetCell(column);
                                    if (Cell.EmptyCell != elseCell && null != elseCell.StringValue)
                                    {
                                        aPlanExtraEntity.ExamineUserName = elseCell.StringValue.Trim();
                                    }
                                }
                                else if (-1 != (elsePos = elseCell.StringValue.IndexOf(approveUser)))
                                {
                                    ++column;
                                    elseCell = aRow.GetCell(column);
                                    if (Cell.EmptyCell != elseCell && null != elseCell.StringValue)
                                    {
                                        aPlanExtraEntity.ApproveUserName = elseCell.StringValue.Trim();
                                    }
                                }
                            }
                        }
                    }
                    else if (-1 != (pos = firstValue.IndexOf(date)) && null != aPlanExtraEntity)
                    {
                        int startRow = aRow.FirstColIndex + 1;
                        Cell compieDateCell = aRow.GetCell(startRow);
                        if (Cell.EmptyCell != compieDateCell && null != compieDateCell.StringValue && string.Empty != compieDateCell.StringValue)
                        {
                            aPlanExtraEntity.CompileDate = compieDateCell.DateTimeValue;
                        }

                        bool secondSet = false;
                        bool thirdSet = false;
                        for (int column = startRow + 1; column <= aRow.LastColIndex; ++column)
                        {
                            Cell elseCell = aRow.GetCell(column);
                            if (Cell.EmptyCell != elseCell && null != elseCell.StringValue && string.Empty != elseCell.StringValue)
                            {
                                int elsePos = -1;
                                if (-1 != (elsePos = elseCell.StringValue.IndexOf(date)))
                                {
                                    ++column;
                                    elseCell = aRow.GetCell(column);
                                    if (Cell.EmptyCell != elseCell && null != elseCell.StringValue && string.Empty != elseCell.StringValue)
                                    {
                                        if (!secondSet)
                                        {
                                            aPlanExtraEntity.ExamineDate = elseCell.DateTimeValue;
                                            secondSet = true;
                                        }
                                        else if (!thirdSet)
                                        {
                                            aPlanExtraEntity.ApproveDate = elseCell.DateTimeValue;
                                            thirdSet = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }
            catch (Exception e)
            {
               
            }
            return returnValue;
        }

        private PlanEntity ExcelRowToPlanEntity(string aSheetName, int aRowNumber, Row aExcelRow, Dictionary<string, int> aMatchedColumnDictionary,
                            ref string aLastComponentName)
        {
            do
            {
                PlanEntity planEntity = new PlanEntity();
                int getValue = 0;
                {
                    Cell cell = Cell.EmptyCell;
                    if (!aMatchedColumnDictionary.TryGetValue("序号", out getValue)
                        || -1 == getValue 
                        || Cell.EmptyCell == (cell = aExcelRow.GetCell(getValue)))
                    {
                        string lError = aSheetName + " " + Convert.ToString(aRowNumber + 1) + "行 无效序号" ;
                        importErrorList.Add(lError);
                        break;
                    }
                    planEntity.SequenceId = Convert.ToInt32(cell.StringValue);
                }

                int taskDescriptionColumn = -1;
                {
                    Cell cell = Cell.EmptyCell;
                    if (aMatchedColumnDictionary.TryGetValue("任务描述", out taskDescriptionColumn)
                        && -1 != taskDescriptionColumn
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(taskDescriptionColumn)))
                    {
                        planEntity.TaskDescription = cell.StringValue;
                    }
                }

                {
                    Cell cell1 = Cell.EmptyCell;
                    Cell cell2 = Cell.EmptyCell;
                    if ((!aMatchedColumnDictionary.TryGetValue("部件", out getValue)
                         || -1 == getValue 
                         || (Cell.EmptyCell == (cell1 = aExcelRow.GetCell(getValue))
                             & Cell.EmptyCell == (cell2 = aExcelRow.GetCell(getValue + 1))))
                        && string.Empty == aLastComponentName)
                    {
                        string lError = aSheetName + " " + Convert.ToString(aRowNumber + 1) + "行 无效部件";
                        importErrorList.Add(lError);
                        break;
                    }

                    if (Cell.EmptyCell != cell1)
                    {
                        planEntity.ComponentName = cell1.StringValue;
                    }

                    if (getValue + 1 != taskDescriptionColumn && Cell.EmptyCell != cell2)
                    {
                        planEntity.ComponentName += cell2.StringValue;
                    }

                    if (null == planEntity.ComponentName)
                    {
                        planEntity.ComponentName = string.Empty;
                    }

                    if (string.Empty == planEntity.ComponentName)
                    {
                        planEntity.ComponentName = aLastComponentName;
                    }
                    else
                    {
                        aLastComponentName = planEntity.ComponentName;
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if (!aMatchedColumnDictionary.TryGetValue("权重(分值)", out getValue)
                        || -1 == getValue 
                        || Cell.EmptyCell == (cell = aExcelRow.GetCell(getValue)))
                    {
                        string lError = aSheetName + " " + Convert.ToString(aRowNumber + 1) + "行 无效权重";
                        importErrorList.Add(lError);
                        break;
                    }
                    planEntity.Weight = Convert.ToDecimal(cell.StringValue);
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if (aMatchedColumnDictionary.TryGetValue("实际得分", out getValue)
                        && -1 != getValue 
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                        && String.Empty != cell.StringValue)
                    {
                        try
                        {
                            planEntity.Score = Convert.ToDecimal(cell.StringValue);
                        }
                        catch(FormatException e )
                        {
                            //ignore exception
                        }
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if (!aMatchedColumnDictionary.TryGetValue("计划完成时间", out getValue)
                        || -1 == getValue
                        || Cell.EmptyCell == (cell = aExcelRow.GetCell(getValue)))
                    {
                        if (!aMatchedColumnDictionary.TryGetValue("计划到货时间", out getValue)
                            || -1 == getValue
                            || Cell.EmptyCell == (cell = aExcelRow.GetCell(getValue)))
                        {
                            string lError = aSheetName + " " + Convert.ToString(aRowNumber + 1) + "行 无效计划时间";
                            importErrorList.Add(lError);
                            break;
                        }
                    }
                    try
                    {
                        planEntity.TargetDate = cell.DateTimeValue;
                    }
                    catch (FormatException e)
                    {
                        //ignore exception
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if ((aMatchedColumnDictionary.TryGetValue("计划第一次调整", out getValue)
                         || aMatchedColumnDictionary.TryGetValue("第一次调整", out getValue))
                        && -1 != getValue
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                        && String.Empty != cell.StringValue)
                    {
                        try
                        {
                            planEntity.TargetDateAdjustment1 = cell.DateTimeValue;
                        }
                        catch (FormatException e)
                        {
                            //ignore exception
                        }
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if ((aMatchedColumnDictionary.TryGetValue("计划第二次调整", out getValue)
                         || aMatchedColumnDictionary.TryGetValue("第二次调整", out getValue))
                        && -1 != getValue
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                        && String.Empty != cell.StringValue)
                    {
                        try
                        {
                            planEntity.TargetDateAdjustment2 = cell.DateTimeValue;
                        }
                        catch (FormatException e)
                        {
                            //ignore exception
                        }
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if ((aMatchedColumnDictionary.TryGetValue("实际完成时间", out getValue)
                            && -1 != getValue
                            && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                            && String.Empty != cell.StringValue)
                        || (aMatchedColumnDictionary.TryGetValue("实际到货时间", out getValue)
                            && -1 != getValue
                            && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                            && String.Empty != cell.StringValue))
                    {
                        try
                        {
                            planEntity.AccomplishDate = cell.DateTimeValue;
                        }
                        catch (FormatException e)
                        {
                            //ignore exception
                        }
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    int getDepartmentId = -1;
                    if (aMatchedColumnDictionary.TryGetValue("责任单位", out getValue)
                        && -1 != getValue
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                        && String.Empty != cell.StringValue
                        && departmentNameIdDictionary.TryGetValue(cell.StringValue, out getDepartmentId))
                    {
                        planEntity.DepartmentId = getDepartmentId;
                        planEntity.DepartmentName = cell.StringValue;
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if (aMatchedColumnDictionary.TryGetValue("备注", out getValue)
                        && -1 != getValue
                        && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue)))
                    {
                        planEntity.Remark = cell.StringValue;
                    }
                }

                {
                    Cell cell = Cell.EmptyCell;
                    if ((aMatchedColumnDictionary.TryGetValue("计划下订单时间", out getValue)
                            && -1 != getValue
                            && Cell.EmptyCell != (cell = aExcelRow.GetCell(getValue))
                            && String.Empty != cell.StringValue))
                    {
                        try
                        {
                            planEntity.OrderDate = cell.DateTimeValue;
                        }
                        catch (FormatException e)
                        {
                            //ignore exception
                        }
                    }
                }

                return planEntity;
            } while (false);
            return null;
        }

        public ICommand OnFirstImport { get; private set; }

        public ICommand OnSmallChange { get; private set; }

        public ICommand OnImportAndChange { get; private set; }

        public ICommand DoubleClickProject { get; private set; }

        public ICommand OnRefash { get; private set; }

        public ICommand OnDownloadTemp { get; private set; }

        private void OnRefashCommand()
        {
            selectProjectEntity = null;
            //ProductEntityList.Clear();
            //ProjectResponsibleEntityList.Clear();
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
            (OnFirstImport as DelegateCommand).RaiseCanExecuteChanged();
            (OnSmallChange as DelegateCommand).RaiseCanExecuteChanged();
            (OnImportAndChange as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void DoubleClickProjectCommand()
        {
            if (null != SelectProjectEntity)
            {
                if (String.IsNullOrEmpty(SelectProjectEntity.PlanVersionID))
                {
                    OnFirstImportCommand();
                }
                else
                {
                    OnSmallChangeCommand();
                }
            }
        }

        private bool CanFirstImport(object aObject)
        {
            return null != SelectProjectEntity 
                && (null == SelectProjectEntity.PlanVersionID 
                    || string.Empty == SelectProjectEntity.PlanVersionID);
        }

        private bool CanChange(object aObject)
        {
            return null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.PlanVersionID;
        }

        private void OnFirstImportCommand()
        {
            IsBusy = true;
            OpenFileDialog oFile = new OpenFileDialog();
            // .xls filter specified to select only .xls file.
            oFile.Filter = "Excel (*.xls)|*.xls";

            if (oFile.ShowDialog() == true)
            {
                ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                string getVersionId = string.Empty;
                string errorMessage = string.Empty;
                string getFileId = string.Empty;
                string lastVersionId = string.Empty;
                string getRequirement = string.Empty;
                PlanExtraEntity planExtraEntity = null;
                importErrorList.Clear();

                try
                {
                    FileStream fs = oFile.File.OpenRead();
                    Workbook book = Workbook.Open(fs);
                    for (int loop = 0; loop < book.Worksheets.Count; ++loop)
                    {
                        if ("设计完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "设计节点";
                        }
                        else if ("采购完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "采购节点";
                        }
                        else if ("生产完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "生产节点";
                        }
                        int firstValueRow = -1;
                        string getManufactureName = string.Empty;
                        Dictionary<string, int> matchedColumnDictionary = new Dictionary<string, int>();
                        int modelIndex = -1;
                        if (-1 == (modelIndex = ExcelSheetValidation(book.Worksheets[loop], ref firstValueRow, ref getManufactureName,
                                                ref getVersionId, ref getFileId, ref getRequirement, ref matchedColumnDictionary)))
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "格式与模板不匹配;\r\n";
                            continue;
                        }

                        if (string.Empty == getVersionId)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "解析版本号失败;\r\n";
                            continue;
                        }

                        if (string.Empty != lastVersionId && getVersionId != lastVersionId)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "版本号与上页不一致;\r\n";
                            continue;
                        }
                        lastVersionId = getVersionId;

                        if (SelectProjectEntity.ManufactureNumber.TrimEnd() != getManufactureName)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "生产令号不正确;\r\n";
                            continue;
                        }

                        var lResult = from c in planManagerDomainContext.plans 
                                      where c.version_id == getVersionId 
                                      && c.manufacture_number == getManufactureName
                                      select c;
                        if (0 != lResult.Count())
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "版本号重复;\r\n";
                            continue;
                        }

                        ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
                        string lastComponentName = string.Empty;
                        ObservableCollection<int> sequenceIdList = new ObservableCollection<int>();
                        foreach (KeyValuePair<int, Row> rowPair in book.Worksheets[loop].Cells.Rows)
                        {
                            try
                            {
                                if ( rowPair.Key < firstValueRow
                                    || CellToPlanExtraEntity(rowPair.Value, getManufactureName, getFileId, getRequirement, ref planExtraEntity)
                                    || rowPair.Value.LastColIndex < 1)
                                {
                                    //不允许穿插                                                                
                                    continue;
                                }
                            }
                            catch (Exception e)
                            {
                                //empty line
                                continue;
                            }

                            PlanEntity planEntity = ExcelRowToPlanEntity(book.Worksheets[loop].Name, rowPair.Key, rowPair.Value, matchedColumnDictionary, ref lastComponentName);
                            if (null != planEntity)
                            {
                                while (sequenceIdList.Contains(planEntity.SequenceId))
                                {
                                    planEntity.SequenceId++;
                                }
                                sequenceIdList.Add(planEntity.SequenceId);

                                planEntity.ProjectName = SelectProjectEntity.ProjectName;
                                planEntity.VersionId = getVersionId; 
                                planEntity.ManufactureNumber = getManufactureName;
                                planEntity.SheetName = book.Worksheets[loop].Name;
                                planList.Add(planEntity);
                            }
                        }

                        if(planList.Count > 0)
                        {
                            PlanListViewModel planListViewModel = new PlanListViewModel(book.Worksheets[loop].Name, planList,
                                                                      modelIndex, departmentIdNameDictionary);
                            planListViewModel.IsReadOnly = false;
                            planListViewModelList.Add(planListViewModel);
                        }
                    }

                    if (string.Empty == errorMessage && 0 == planListViewModelList.Count)
                    {
                        errorMessage = "无效文件 " + oFile.File.Name + "\r\n";
                    }
                    else
                    {
                        foreach (string value in importErrorList)
                        {
                            errorMessage += (value + "\r\n");
                        }
                        importErrorList.Clear();
                    }                    
                }
                catch (Exception e)
                {
                    errorMessage = e.Message + "\r\n";
                }

                if (planListViewModelList.Count > 0)
                {
                    string title = "初次入库(";
                    title += SelectProjectEntity.ProjectName;
                    title += " ";
                    title += SelectProjectEntity.ManufactureNumber;
                    title += " ";
                    title += getVersionId;
                    title += ")";
                    if (null != planExtraEntity)
                    {
                        planExtraEntity.VersionId = getVersionId;
                    }
                    PlanListEditWindow planListWindow = new PlanListEditWindow(title, "入库", getVersionId, planListViewModelList, planExtraEntity);
                    planListWindow.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
                    planListWindow.Closed += new EventHandler(PlanListWindow_Closed);
                    planListWindow.Show();
 
                    if (string.Empty != errorMessage)
                    {
                        Message.InfoMessage(errorMessage);
                    }
                }
                else
                {
                    Message.ErrorMessage(errorMessage);
                }
            }
            
            IsBusy = false;
        }

        void PlanListWindow_Closed(object sender, EventArgs e)
        {
            PlanListEditWindow planListWindow = sender as PlanListEditWindow;
            if (planListWindow.DialogResult == true)
            {
                int isAccomplished = 1;
                bool newPlans = false;
                int maxVersion = 0;
                for (int pos = 0; pos < planListWindow.planListViewModelList.Count; ++pos)
                {
                    PlanListViewModel planListViewModel = planListWindow.planListViewModelList[pos];
                    for (int planpos = 0; planpos < planListViewModel.PlanList.Count; ++planpos)
                    {
                        PlanEntity planEntity = planListViewModel.PlanList[planpos];
                        newPlans = (newPlans | planEntity.IsNew());

                        if (maxVersion < 1 && planEntity.TargetDateAdjustment1.HasValue)
                        {
                            maxVersion = 1;
                        }
                        else if (maxVersion < 2 && planEntity.TargetDateAdjustment2.HasValue)
                        {
                            maxVersion = 2;
                        }
                    }
                }
              
                string lNewVersionId = planListWindow.VersionId;
                if (newPlans)
                {
                    string titleString = planListWindow.Title as string;
                    if (-1 != titleString.IndexOf("少量修改"))
                    {
                        newPlans = false;
                        int lLeftPos = lNewVersionId.IndexOf('.') + 1;
                        string lDecimal = lNewVersionId.Substring(lLeftPos);
                        int lNumber = Convert.ToInt32(lDecimal);
                        if (lNumber < maxVersion)
                        {
                            newPlans = true;
                            if (1 == maxVersion)
                            {
                                lNewVersionId = lNewVersionId.Substring(0, lLeftPos) + Convert.ToString(maxVersion);
                            }
                            else
                            {
                                lNewVersionId = lNewVersionId.Substring(0, lLeftPos) + Convert.ToString(maxVersion);
                            }
                        }
                    }
                    planListWindow.VersionId = lNewVersionId;
                }

                for(int pos = 0; pos < planListWindow.planListViewModelList.Count; ++pos)
                {
                    PlanListViewModel planListViewModel = planListWindow.planListViewModelList[pos];                  
                    for(int planpos = 0; planpos < planListViewModel.PlanList.Count; ++planpos)
                    {
                        PlanEntity planEntity = planListViewModel.PlanList[planpos];
                        if (null == planEntity.AccomplishDate)
                        {
                            isAccomplished = 0;
                        }

                        planEntity.VersionId = planListWindow.VersionId;
                        if (newPlans)
                        {
                            planEntity.Plan = null;
                        }
                        bool lTempNew = (null == planEntity.Plan);
                        planEntity.DUpdate();
                        if (newPlans || lTempNew)
                        {
                            planManagerDomainContext.plans.Add(planEntity.Plan);
                        }
                    }

                    for (int deletePos = 0; deletePos < planListViewModel.DeletedPlanList.Count; ++deletePos)
                    {
                        PlanEntity planEntity = planListViewModel.DeletedPlanList[deletePos];
                        if (null != planEntity.Plan)
                        {
                            planManagerDomainContext.plans.Remove(planEntity.Plan);
                        }
                    }
                }

                App app = Application.Current as App;
                if(null != planListWindow.planExtraEntity)
                {
                    planListWindow.planExtraEntity.VersionId = planListWindow.VersionId;
                    planListWindow.planExtraEntity.CompileUserName = app.UserInfo.UserName;
                    planListWindow.planExtraEntity.CompileDate = DateTime.Now;
                    if (newPlans)
                    {
                        planListWindow.planExtraEntity.PlanExtra = null;
                    }
                    planListWindow.planExtraEntity.DUpdate();
                    if (newPlans)
                    {
                        planManagerDomainContext.plan_extras.Add(planListWindow.planExtraEntity.PlanExtra);
                    }
                }

                SelectProjectEntity.CompileUserName = app.UserInfo.UserName;
                SelectProjectEntity.CompileDate = DateTime.Now;
                SelectProjectEntity.AccomplishMark = isAccomplished;
                SelectProjectEntity.PlanVersionID = planListWindow.VersionId;
                SelectProjectEntity.PlanUpdateDate = DateTime.Now;
                SelectProjectEntity.DUpdate();
                
                SubmitOperation submitOperation = planManagerDomainContext.SubmitChanges();
                submitOperation.Completed += SubmitOperation_Completed;
            }
            (OnFirstImport as DelegateCommand).RaiseCanExecuteChanged();
            (OnSmallChange as DelegateCommand).RaiseCanExecuteChanged();
            (OnImportAndChange as DelegateCommand).RaiseCanExecuteChanged();
        }

        void SubmitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                Message.ErrorMessage("失败!");
            }
            else
            {
                Message.InfoMessage("成功!");
                OnRefashCommand();
            }
        }

        private bool GetNewVersionId(string aManufactureNumber, ref string aNewVersionID)
        {
            bool getNewVersionId = false;
            string lastVersionId = aNewVersionID;
            int pos = lastVersionId.IndexOf('.');
            if (-1 != pos && ++pos < lastVersionId.Length)
            {
                int currentSubVersion = Convert.ToInt32(lastVersionId.Substring(pos));
                while (!getNewVersionId && currentSubVersion < ushort.MaxValue)
                {
                    ++currentSubVersion;
                    lastVersionId = lastVersionId.Substring(0, pos) + Convert.ToString(currentSubVersion);
                    IEnumerable<string> versionIds = from c in planManagerDomainContext.plans
                                                     where c.version_id == lastVersionId
                                                     && c.manufacture_number == aManufactureNumber
                                                     select c.version_id;

                    if (0 == versionIds.Count())
                    {
                        getNewVersionId = true;
                        aNewVersionID = lastVersionId;
                        break;
                    }
                }
            }
            return getNewVersionId;
        }

        private void OnSmallChangeCommand()
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
                string newVersionId = SelectProjectEntity.PlanVersionID;
//                 if (!GetNewVersionId(SelectProjectEntity.ManufactureNumber, ref newVersionId))
//                 {
//                     Message.ErrorMessage("生成新版本号失败");
//                     IsBusy = false;
//                     return;
//                 }

                IEnumerable<string> sheetNames = from c in planManagerDomainContext.plans 
                                                 where c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                    && c.version_id == SelectProjectEntity.PlanVersionID 
                                                select c.sheet_name;
                if (sheetNames.Count() > 0)
                {
                    ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                    ObservableCollection<string> differentSheets = new ObservableCollection<string>();
                    foreach (string originalValue in sheetNames)
                    {
                        if (null == originalValue)
                        {
                            continue;
                        }
                        string value = originalValue;

                        if ("设计完成节点" == value)
                        {
                            value = "设计节点";
                        }
                        else if ("采购完成节点" == value)
                        {
                            value = "采购节点";
                        }
                        else if ("生产完成节点" == value)
                        {
                            value = "生产节点";
                        }

                        if (differentSheets.Contains(value))
                        {
                            continue;
                        }
                        differentSheets.Add(value);
                        IEnumerable<plan> selectedPlans = from c in planManagerDomainContext.plans
                                                  where c.manufacture_number == SelectProjectEntity.ManufactureNumber.TrimEnd()
                                                    && c.version_id == SelectProjectEntity.PlanVersionID
                                                    && c.sheet_name == originalValue orderby c.sequence_id
                                                  select c;
                        ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
                        bool hasOrderDate = false;
                        for (int pos = 0; pos < selectedPlans.Count<plan>(); ++pos)
                        {
                            plan item = selectedPlans.ElementAt(pos); ;
                            PlanEntity planEntity = new PlanEntity();
                            planEntity.Plan = item;
                            if (item.order_date.HasValue)
                            {
                                hasOrderDate = true;
                            }
                            string getDepartmentName = string.Empty;
                            planEntity.Update();
 //                           planEntity.Plan = null;
 //                           planEntity.VersionId = newVersionId;
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
                            planListViewModel.IsChanged = false;
                            planListViewModel.IsReadOnly = false;
                            planListViewModelList.Add(planListViewModel);
                        }
                    }
                    if (planListViewModelList.Count > 0)
                    {
                        string title = "少量修改(";
                        title += SelectProjectEntity.ProjectName;
                        title += " ";
                        title += SelectProjectEntity.ManufactureNumber.TrimEnd();
                        title += " ";
                        title += newVersionId;
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
//                             planExtraEntity.VersionId = newVersionId;
//                             planExtraEntity.PlanExtra = null;
                        }
                        PlanListEditWindow planListWindow = new PlanListEditWindow(title, "修改", newVersionId, planListViewModelList, planExtraEntity);
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
                                            + ")" + "\r\n"; ;
                    Message.ErrorMessage(errorMessage);
                }
            }

            IsBusy = false;
        }

        private void OnImportAndChangeCommand()
        {
            IsBusy = true;
//             string newVersionId = SelectProjectEntity.PlanVersionID;
//             if (!GetNewVersionId(SelectProjectEntity.ManufactureNumber, ref newVersionId))
//             {
//                 Message.ErrorMessage("生成新版本号失败");
//                 IsBusy = false;
//                 return;
//             }

            OpenFileDialog oFile = new OpenFileDialog();
            // .xls filter specified to select only .xls file.
            oFile.Filter = "Excel (*.xls)|*.xls";

            if (oFile.ShowDialog() == true)
            {
                ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                string excelVersionId = string.Empty;
                string errorMessage = string.Empty;
                string lastVersionId = string.Empty;
                string getFileId = string.Empty;
                string getRequirement = string.Empty;
                PlanExtraEntity planExtraEntity = null;
                importErrorList.Clear();
                
                try
                {
                    FileStream fs = oFile.File.OpenRead();
                    Workbook book = Workbook.Open(fs);
                    for (int loop = 0; loop < book.Worksheets.Count; ++loop)
                    {
                        if ("设计完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "设计节点";
                        }
                        else if ("采购完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "采购节点";
                        }
                        else if ("生产完成节点" == book.Worksheets[loop].Name)
                        {
                            book.Worksheets[loop].Name = "生产节点";
                        }
                        int firstValueRow = -1;
                        string getManufactureName = string.Empty;
                        int modelIndex = -1;
                        Dictionary<string, int> matchedColumnDictionary = new Dictionary<string, int>();
                        if (-1 == (modelIndex = ExcelSheetValidation(book.Worksheets[loop], ref firstValueRow,  ref getManufactureName,
                                                                    ref excelVersionId, ref getFileId, ref getRequirement, ref matchedColumnDictionary)))
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "格式与模板不匹配;\r\n";
                            continue;
                        }

                        if (string.Empty == excelVersionId)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "解析版本号失败;\r\n";
                            continue;
                        }

                        if (string.Empty != lastVersionId && excelVersionId != lastVersionId)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "版本号与上页不一致;\r\n";
                            continue;
                        }

                        if (SelectProjectEntity.ManufactureNumber.TrimEnd() != getManufactureName)
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "生产令号不正确;\r\n";
                            continue;
                        }

                        var lResult = from c in planManagerDomainContext.plans 
                                      where c.version_id == excelVersionId
                                      && c.manufacture_number == getManufactureName
                                      select c;
                        if (0 != lResult.Count())
                        {
                            errorMessage += book.Worksheets[loop].Name;
                            errorMessage += "版本号重复;\r\n";
                            continue;
                        }
                        lastVersionId = excelVersionId;

                        ObservableCollection<int> sequenceIdList = new ObservableCollection<int>();
                        ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
                        string lastComponentName = string.Empty;
                        foreach (KeyValuePair<int, Row> rowPair in book.Worksheets[loop].Cells.Rows)
                        {
                            try
                            {
                                if (rowPair.Key < firstValueRow
                                    || CellToPlanExtraEntity(rowPair.Value, getManufactureName, getFileId, getRequirement, ref planExtraEntity)
                                    || rowPair.Value.LastColIndex < 1)
                                {
                                    //不允许穿插                                                                
                                    continue;
                                }
                            }
                            catch (Exception e)
                            {
                                //empty line
                                continue;
                            }

                            PlanEntity planEntity = ExcelRowToPlanEntity(book.Worksheets[loop].Name, rowPair.Key, rowPair.Value, matchedColumnDictionary, ref lastComponentName);
                            if (null != planEntity)
                            {
                                while (sequenceIdList.Contains(planEntity.SequenceId))
                                {
                                    planEntity.SequenceId++;
                                }
                                sequenceIdList.Add(planEntity.SequenceId);

                                planEntity.VersionId = excelVersionId;
                                planEntity.ManufactureNumber = getManufactureName;
                                planEntity.SheetName = book.Worksheets[loop].Name;
                                planList.Add(planEntity);
                            }
                        }

                        if (planList.Count > 0)
                        {
                            PlanListViewModel planListViewModel = new PlanListViewModel(book.Worksheets[loop].Name, planList,
                                                                      modelIndex, departmentIdNameDictionary);
                            planListViewModel.IsReadOnly = false;
                            planListViewModelList.Add(planListViewModel);
                        }
                    }

                    if (string.Empty == errorMessage && 0 == planListViewModelList.Count)
                    {
                        errorMessage = "无效文件 " + oFile.File.Name + "\r\n";
                    }
                    else
                    {
                        foreach (string value in importErrorList)
                        {
                            errorMessage += (value + "\r\n");
                        }
                        importErrorList.Clear();
                    }       
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                if (planListViewModelList.Count > 0)
                {                   
                    string title = "大量修改(";
                    title += SelectProjectEntity.ProjectName;
                    title += " ";
                    title += SelectProjectEntity.ManufactureNumber;
                    title += " ";
                    title += excelVersionId;
                    title += ")";
                    if (null != planExtraEntity)
                    {
                        planExtraEntity.VersionId = excelVersionId;
                    }
                    PlanListEditWindow planListWindow = new PlanListEditWindow(title, "修改", excelVersionId, planListViewModelList, planExtraEntity);
                    planListWindow.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
                    planListWindow.Closed += new EventHandler(PlanListWindow_Closed);
                    planListWindow.Show();

                    if (string.Empty != errorMessage)
                    {
                        Message.InfoMessage(errorMessage);
                    }
                }
                else
                {
                    Message.ErrorMessage(errorMessage);
                }
            }
            IsBusy = false;
        }

        private SaveFileDialog saveFileDialog;

        private void OnDownloadTempCommand()
        {
            String FileUrl = CustomUri.GetAbsoluteUrl("ProductmanagerFileTemp/计划模版.xls");

            try
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "All Files|*.*";
                saveFileDialog.GetType().GetMethod("set_DefaultFileName").Invoke(saveFileDialog, new object[] { "计划模版.xls" });
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
    }
}
