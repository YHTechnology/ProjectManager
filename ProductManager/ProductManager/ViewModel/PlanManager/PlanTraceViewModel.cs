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
using Microsoft.Windows.Data.DomainServices;
using System.ComponentModel;

namespace ProductManager.ViewModel.PlanManager
{
    [Export("PlanTrace")]
    public class PlanTraceViewModel : NotifyPropertyChanged
    {
        private enum ViewType : uint
        {
            ViewTypeUnknown = 0,
            ViewTypeSingle = 1,
            ViewTypeMulti = 2,
            ViewTypeAll = 3,
            ViewTypeHistory = 4
        }

        private PlanManagerDomainContext planManagerDomainContext = new PlanManagerDomainContext();

        private ProductDomainContext ProductDomainContext = new ProductDomainContext();

        public ObservableCollection<ProjectEntity> ProjectList { get; set; }

        private ObservableCollection<ProjectEntity> selectedProjectList = null;
        public ObservableCollection<ProjectEntity> SelectedProjectList 
        {
            get
            {
                return selectedProjectList;
            } 
            set
            {
                if (selectedProjectList != value)
                {
                    selectedProjectList = value;
                    (OnViewSinglePlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnSetRemind as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewSingleHistory as DelegateCommand).RaiseCanExecuteChanged();   
                }
            }
        }

        private DomainCollectionView<project> projectView;
        private DomainCollectionViewLoader<project> projectLoader;
        private EntityList<project> prjectSource;

        private DomainCollectionView<plan> planView;
        private DomainCollectionViewLoader<plan> planLoader;
        private EntityList<plan> planSource;

        private Dictionary<int, string> departmentIdNameDictionary { get; set; }
        private ViewType currentViewType = ViewType.ViewTypeUnknown;

        private Dictionary<string, UserProjectEntity> UserProjectEntityDictionary { get; set; }
        private Dictionary<String, UserRemindEntity> UserRemindEntityDicationary { get; set; }

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
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        selectedProjectList = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }
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
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        selectedProjectList = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }
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
                    if (isFilter)
                    {
                        selectProjectEntity = null;
                        selectedProjectList = null;
                        //ProductEntityList.Clear();
                        //ProjectResponsibleEntityList.Clear();
                        using (this.CollectionProjectView.DeferRefresh())
                        {
                            this.projectView.MoveToFirstPage();
                        }
                    }
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
            planManagerDomainContext = new PlanManagerDomainContext();
            ProductDomainContext = new ProductDomainContext();

            planManagerDomainContext.plan_extras.Clear();

            App app = Application.Current as App;
            LoadOperation<user_remind> loadOperationUserRemind =
               planManagerDomainContext.Load<user_remind>(planManagerDomainContext.GetUserRemindQuery(app.UserInfo.UserID));
            loadOperationUserRemind.Completed += LoadUseRemindComplete;
        }


        void LoadUseRemindComplete(object sender, EventArgs e)
        {
            UserRemindEntityDicationary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (user_remind user_remind in loadOperation.Entities)
            {
                UserRemindEntity lUserRemindEntity = new UserRemindEntity();
                lUserRemindEntity.UserRemind = user_remind;
                lUserRemindEntity.Update();
                UserRemindEntityDicationary.Add(lUserRemindEntity.ManufactureNumber, lUserRemindEntity);
            }


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

        void loadOperationPlanExtra_Completed(object sender, EventArgs e)
        {
            planManagerDomainContext.departments.Clear();
            LoadOperation<department> loadOperationDepartment =
                planManagerDomainContext.Load<department>(planManagerDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            departmentIdNameDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                departmentIdNameDictionary.Add(departmentEntity.DepartmentID, departmentEntity.DepartmentName);
            }
            IsBusy = true;

            planManagerDomainContext.projects.Clear();
            //LoadOperation<project> loadOperationProject =
            //    planManagerDomainContext.Load<project>(planManagerDomainContext.GetProjectQuery());
            //loadOperationProject.Completed += loadOperationProject_Completed;
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

//         void loadOperationProject_Completed(object sender, EventArgs e)
//         {
//             ProjectList.Clear();
//             LoadOperation loadOperation = sender as LoadOperation;
//             foreach (project project in loadOperation.Entities)
//             {
//                 ProjectEntity projectEntity = new ProjectEntity();
//                 projectEntity.Project = project;
//                 projectEntity.Update();
//                 ProjectList.Add(projectEntity);
//             }
// 
//             UpdateChanged("ProjectList");
//             (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
//             IsBusy = false;
//         }

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

//                     IEnumerable<string> planVersions = from c in planManagerDomainContext.plans
//                                                           where c.manufacture_number == projectEntity.ManufactureNumber
//                                                           select c.version_id;
                    if (!string.IsNullOrEmpty(project.plan_version_id))
                    {
//                         projectEntity.PlanVersionDictionary = new Dictionary<string, string>();
//                         foreach (string item in planVersions)
//                         {
//                             if (projectEntity.PlanVersionDictionary.ContainsKey(item))
//                             {
//                                 continue;
//                             }
//                             projectEntity.PlanVersionDictionary.Add(item, item);
//                         }
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

                UserRemindEntity lUserRemindEntity;
                if (UserRemindEntityDicationary.TryGetValue(projectEntity.ManufactureNumber, out lUserRemindEntity))
                {
                    projectEntity.UserRemindEntity = lUserRemindEntity;
                }

                projectEntity.PlanManagerDomainContext = planManagerDomainContext;

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
                    (OnViewSinglePlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnSetRemind as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewSingleHistory as DelegateCommand).RaiseCanExecuteChanged();           
                }
            }
        }

        public ICommand OnRefash { get; private set; }

        private void OnRefashCommand()
        {
            selectProjectEntity = null;
            selectedProjectList = null;
            //ProductEntityList.Clear();
            //ProjectResponsibleEntityList.Clear();
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }

        public PlanTraceViewModel() 
        {
            ProjectList = new ObservableCollection<ProjectEntity>();

            departmentIdNameDictionary = new Dictionary<int, string>();
            UserProjectEntityDictionary = new Dictionary<string, UserProjectEntity>();
            UserRemindEntityDicationary = new Dictionary<String, UserRemindEntity>();

            OnViewSinglePlan = new DelegateCommand(OnViewSinglePlanCommand, CanViewPlan);
            OnSetRemind = new DelegateCommand(OnSetRemindCommand, CanSetRemind);
            OnViewAllPlan = new DelegateCommand(OnViewAllPlanCommand, CanViewAllPlan);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);
            OnRefash = new DelegateCommand(OnRefashCommand);
            OnViewSingleHistory = new DelegateCommand(OnViewSingleHistoryCommand, CanViewHistory);

            FilterList = new ObservableCollection<string>();
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";
        }

        public ICommand OnViewSinglePlan { get; private set; }

        public ICommand OnSetRemind { get; private set; }

        public ICommand OnViewAllPlan { get; private set; }

        public ICommand DoubleClickProject { get; private set; }

        public ICommand OnViewSingleHistory { get; private set; }

        private void DoubleClickProjectCommand()
        {
            OnViewSinglePlanCommand();
        }

        private LoadOperation<plan> LoadPlanEntities()
        {
            EntityQuery<plan> lQuery = planManagerDomainContext.GetPlanQuery();            
            switch (currentViewType)
            {
                case ViewType.ViewTypeSingle:
                    if (null == SelectedProjectList || SelectedProjectList.Count <= 1)
                    {
                        lQuery = lQuery.Where(e => e.version_id == SelectProjectEntity.PlanVersionID
                                            && e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                    }
                    break;
                case ViewType.ViewTypeMulti:
                    break;
                case ViewType.ViewTypeAll:
                    break;
                case ViewType.ViewTypeHistory:
                    if (null == SelectedProjectList || SelectedProjectList.Count <= 1)
                    {
                        lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                    }
                    break;
                default:
                    break;
            }

            return planManagerDomainContext.Load(lQuery.SortAndPageBy(projectView));
        }

        private void ViewSingle()
        {
            Dictionary<string, ObservableCollection<PlanEntity>> planListDictionary = new Dictionary<string, ObservableCollection<PlanEntity>>();
            Dictionary<string, int> moduleIndexDictionary = new Dictionary<string, int>();
            if (GetPorjectPlanList(SelectProjectEntity, SelectProjectEntity.PlanVersionID, true, ref planListDictionary, ref moduleIndexDictionary))
            {
                if (planListDictionary.Count > 0)
                {
                    ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                    foreach (KeyValuePair<string, ObservableCollection<PlanEntity>> kv in planListDictionary)
                    {
                        PlanListViewModel planListViewModel = new PlanListViewModel(kv.Key,
                                                                    planListDictionary[kv.Key],
                                                                    moduleIndexDictionary[kv.Key],
                                                                    departmentIdNameDictionary);
                        planListViewModel.IsReadOnly = true;
                        planListViewModelList.Add(planListViewModel);
                    }

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

                    string title = "跟踪计划" + SelectProjectEntity.ProjectName + " "
                                    + SelectProjectEntity.ManufactureNumber + " "
                                    + SelectProjectEntity.PlanVersionID + " "
                                    + "(红色（超过计划时间）；品红（接近计划时间）；绿色（按时完成）；紫色（超时完成）；灰色（无状态）)";

                    PlanListTraceWindow planListWindow = new PlanListTraceWindow(title, planListViewModelList,
                                                                        planExtraEntity, this);
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

        private void ViewMulti()
        {
            Dictionary<string, ObservableCollection<PlanEntity>> planListDictionary = new Dictionary<string, ObservableCollection<PlanEntity>>();
            Dictionary<string, int> moduleIndexDictionary = new Dictionary<string, int>();

            foreach (ProjectEntity item in SelectedProjectList)
            {
                if (null == item.PlanVersionID
                || null == item.ManufactureNumber
                || string.Empty == item.PlanVersionID
                || string.Empty == item.ManufactureNumber)
                {
                    continue;
                }

                GetPorjectPlanList(item, item.PlanVersionID, true, ref planListDictionary, ref moduleIndexDictionary);
            }

            if (planListDictionary.Count > 0)
            {
                ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                foreach (KeyValuePair<string, ObservableCollection<PlanEntity>> kv in planListDictionary)
                {
                    PlanListViewModel planListViewModel = new PlanListViewModel(kv.Key,
                                                                planListDictionary[kv.Key],
                                                                moduleIndexDictionary[kv.Key],
                                                                departmentIdNameDictionary);
                    planListViewModel.IsReadOnly = true;
                    planListViewModelList.Add(planListViewModel);
                }

                PlanListTraceWindow planListWindow = new PlanListTraceWindow("跟踪计划(说明：红色（超过计划时间）；品红（接近计划时间）；绿色（按时完成）；紫色（超时完成）；灰色（无状态）)",
                    planListViewModelList, null, this);
                planListWindow.Closed += new EventHandler(PlanListWindow_Closed);
                planListWindow.Show();
            }
        }

        private void ViewAll()
        {
            Dictionary<string, ObservableCollection<PlanEntity>> planListDictionary = new Dictionary<string, ObservableCollection<PlanEntity>>();
            Dictionary<string, int> moduleIndexDictionary = new Dictionary<string, int>();

            foreach (ProjectEntity item in ProjectList)
            {
                if (null == item.PlanVersionID
                || null == item.ManufactureNumber
                || string.Empty == item.PlanVersionID
                || string.Empty == item.ManufactureNumber)
                {
                    continue;
                }

                GetPorjectPlanList(item, item.PlanVersionID, true, ref planListDictionary, ref moduleIndexDictionary);
            }

            if (planListDictionary.Count > 0)
            {
                ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                foreach (KeyValuePair<string, ObservableCollection<PlanEntity>> kv in planListDictionary)
                {
                    PlanListViewModel planListViewModel = new PlanListViewModel(kv.Key,
                                                                planListDictionary[kv.Key],
                                                                moduleIndexDictionary[kv.Key],
                                                                departmentIdNameDictionary);
                    planListViewModel.IsReadOnly = true;
                    planListViewModelList.Add(planListViewModel);
                }

                PlanListTraceWindow planListWindow = new PlanListTraceWindow("跟踪计划(说明：红色（超过计划时间）；品红（接近计划时间）；绿色（按时完成）；紫色（超时完成）；灰色（无状态）)",
                    planListViewModelList, null, this);
                planListWindow.Closed += new EventHandler(PlanListWindow_Closed);
                planListWindow.Show();
            }
        }

        private void ViewHistory()
        {            
            Dictionary<string, ObservableCollection<PlanEntity>> planListDictionary = new Dictionary<string, ObservableCollection<PlanEntity>>();
            Dictionary<string, int> moduleIndexDictionary = new Dictionary<string, int>();
            if (GetPorjectPlanList(SelectProjectEntity, SelectProjectEntity.PlanVersionID, false, ref planListDictionary, ref moduleIndexDictionary))
            {
                if (planListDictionary.Count > 0)
                {
                    ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                    foreach (KeyValuePair<string, ObservableCollection<PlanEntity>> kv in planListDictionary)
                    {
                        PlanListViewModel planListViewModel = new PlanListViewModel(kv.Key,
                                                                    planListDictionary[kv.Key],
                                                                    moduleIndexDictionary[kv.Key],
                                                                    departmentIdNameDictionary);
                        planListViewModel.IsReadOnly = true;
                        planListViewModelList.Add(planListViewModel);
                    }

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

                    string title = "历史计划" + SelectProjectEntity.ProjectName + " "
                                    + SelectProjectEntity.ManufactureNumber.TrimEnd() + " "
                                    + SelectProjectEntity.PlanVersionID + " "
                                    + "(红色（超过计划时间）；品红（接近计划时间）；绿色（按时完成）；紫色（超时完成）；灰色（无状态）)";


                    PlanListHistoryWindow planListWindow = new PlanListHistoryWindow(title, planListViewModelList,
                                                                        planExtraEntity, this, 
                                                                        SelectProjectEntity,
                                                                        SelectProjectEntity.PlanVersionID);
                    planListWindow.Closed += new EventHandler(HistoryListWindow_Closed);
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

        private void LoadOperationPlanCompleted(LoadOperation<plan> aLoadOperation)
        {
            switch (currentViewType)
            {
            case ViewType.ViewTypeSingle:
                ViewSingle();
                break;
            case ViewType.ViewTypeMulti:
                ViewMulti();
                break;
           case ViewType.ViewTypeAll:
                ViewAll();
                break;
           case ViewType.ViewTypeHistory:
                {
                    SelectProjectEntity.PlanVersionDictionary = new Dictionary<string, string>();
                    foreach (plan item in aLoadOperation.Entities)
                    {
                        if (SelectProjectEntity.PlanVersionDictionary.ContainsKey(item.version_id))
                        {
                            continue;
                        }
                        SelectProjectEntity.PlanVersionDictionary.Add(item.version_id, item.version_id);
                    }
                    ViewHistory();
                }
                break;
            default:
                break;
            }

            IsBusy = false;
        }

        private void OnViewSinglePlanCommand()
        {
            IsBusy = true;

            if (null == SelectedProjectList || SelectedProjectList.Count <= 1)
            {
                if (null == SelectProjectEntity.PlanVersionID
                                || null == SelectProjectEntity.ManufactureNumber
                                || string.Empty == SelectProjectEntity.PlanVersionID
                                || string.Empty == SelectProjectEntity.ManufactureNumber)
                {
                    Message.ErrorMessage("生产令号或版本号无效");
                    IsBusy = false;
                    return;
                }

                currentViewType = ViewType.ViewTypeSingle;
            }
            else
            {
                currentViewType = ViewType.ViewTypeMulti;
            }

            planSource = new EntityList<plan>(planManagerDomainContext.plans);
            planLoader = new DomainCollectionViewLoader<plan>(
                LoadPlanEntities,
                LoadOperationPlanCompleted);
            planView = new DomainCollectionView<plan>(planLoader, planSource);
            using (planView.DeferRefresh())
            {
                planView.MoveToFirstPage();
            }        
        }

        private void OnSetRemindCommand()
        {
            IsBusy = true;
            PlanSetRemindDayWindow planRemindWindow = new PlanSetRemindDayWindow(SelectProjectEntity);
            planRemindWindow.Closed += new EventHandler(PlanRemindWindow_Closed);
            planRemindWindow.Show();
            IsBusy = false;
        }

        void PlanRemindWindow_Closed(object sender, EventArgs e)
        {
            PlanSetRemindDayWindow planRemindWindow = sender as PlanSetRemindDayWindow;
            if (planRemindWindow.DialogResult == true)
            {
                SelectProjectEntity.DUpdate();
                planManagerDomainContext.SubmitChanges();
            }
        }

        private void OnViewAllPlanCommand()
        {
            IsBusy = true;
            currentViewType = ViewType.ViewTypeAll;

            planSource = new EntityList<plan>(planManagerDomainContext.plans);
            planLoader = new DomainCollectionViewLoader<plan>(
                LoadPlanEntities,
                LoadOperationPlanCompleted);
            planView = new DomainCollectionView<plan>(planLoader, planSource);
            using (planView.DeferRefresh())
            {
                planView.MoveToFirstPage();
            }        
        }

        private bool CanViewHistory(object aObject)
        {
            bool value = null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && null != SelectProjectEntity.ManufactureNumber
                && string.Empty != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.ManufactureNumber;

            if (value && null != SelectedProjectList && SelectedProjectList.Count > 1)
            {
                value = false;
            }

            return value;
        }

        private bool CanViewPlan(object aObject)
        {
            bool value = null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && null != SelectProjectEntity.ManufactureNumber
                && string.Empty != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.ManufactureNumber;

            if (!value && null != SelectedProjectList)
            {
                foreach (ProjectEntity item in SelectedProjectList)
                {
                    if (null != item.PlanVersionID && string.Empty != item.PlanVersionID)
                    {
                        value = true;
                        break;
                    }
                }
            }

            return value;
        }

        private bool CanSetRemind(object aObject)
        {
            return null != SelectProjectEntity && (null == SelectedProjectList || SelectedProjectList.Count <= 1); 
        }

        private void SaveChanges()
        {
            SubmitOperation submitOperation = planManagerDomainContext.SubmitChanges();
            submitOperation.Completed += SubmitOperation_Completed;
        }

        void SubmitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (!submitOperation.HasError)
            {
                LoadData();
            }
        }        

        private bool GetPorjectPlanList(ProjectEntity item, string aPlanVersionId, bool aIsSetable,
            ref Dictionary<string, ObservableCollection<PlanEntity>> aPlanListDictionary, 
            ref Dictionary<string, int> aModuleIndexDictionary)
        {
            bool rturnValue = false;
            IEnumerable<string> sheetNames = from c in planManagerDomainContext.plans
                                             where c.manufacture_number == item.ManufactureNumber.TrimEnd()
                                                && c.version_id == aPlanVersionId
                                             select c.sheet_name;
            if (sheetNames.Count() > 0)
            {
                ObservableCollection<string> differentSheets = new ObservableCollection<string>();
                foreach(string originalValue in sheetNames)
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
                                                      where c.manufacture_number == item.ManufactureNumber.TrimEnd()
                                                        && c.version_id == aPlanVersionId
                                                        && c.sheet_name == originalValue
                                                      select c;
                    if (selectedPlans.Count() > 0)
                    {
                        if(!aModuleIndexDictionary.Keys.Contains(value))
                        {
                            aModuleIndexDictionary.Add(value, 0);
                        }

                        if (!aPlanListDictionary.Keys.Contains(value))
                        {
                            aPlanListDictionary.Add(value, new ObservableCollection<PlanEntity>());
                        }

                        if (selectedPlans.First().order_date.HasValue)
                        {
                            aModuleIndexDictionary[value] = 1;
                        }

                        ObservableCollection<PlanEntity> planList = aPlanListDictionary[value];    
                        foreach (plan planItem in selectedPlans)
                        {
                            PlanEntity planEntity = new PlanEntity();
                            planEntity.Plan = planItem;
                            string getDepartmentName = string.Empty;
                            planEntity.Update();
                            planEntity.ProjectName = item.ProjectName;
                            planEntity.PlanRemindDay = ("设计节点" == planItem.sheet_name) ? item.RemindDayDesign :
                                (("采购节点" == planItem.sheet_name) ? item.RemindDayPurchase : item.RemindDayProduce);
                            planEntity.IsSetable = aIsSetable;
                            if (departmentIdNameDictionary.TryGetValue(planEntity.DepartmentId, out getDepartmentName))
                            {
                                planEntity.DepartmentName = getDepartmentName;
                            }
                            planList.Add(planEntity);         
                        }
                    }
                }

                rturnValue = true;
            }
            return rturnValue;
        }

        private bool CanViewAllPlan(object aObject)
        {
            bool returnValue = false;

            foreach (ProjectEntity item in ProjectList)
            {
                if (null != item.PlanVersionID && string.Empty != item.PlanVersionID)
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        private void OnViewSingleHistoryCommand()
        {
            IsBusy = true;
            if (!SelectProjectEntity.HasHistory)
            {
                Message.ErrorMessage("无历史计划版本");
                IsBusy = false;
                return;
            }
            
            currentViewType = ViewType.ViewTypeHistory;

            planSource = new EntityList<plan>(planManagerDomainContext.plans);
            planLoader = new DomainCollectionViewLoader<plan>(
                LoadPlanEntities,
                LoadOperationPlanCompleted);
            planView = new DomainCollectionView<plan>(planLoader, planSource);
            using (planView.DeferRefresh())
            {
                planView.MoveToFirstPage();
            }        
        }

        void HistoryListWindow_Closed(object sender, EventArgs e)
        {
            PlanListHistoryWindow planListWindow = sender as PlanListHistoryWindow;
            if (planListWindow.DialogResult == true)
            {
                if (null == planListWindow.projectEntity || String.IsNullOrEmpty(planListWindow.SelectVersionId))
                {
                    Message.ErrorMessage("无效计划版本");
                }
                else
                {
                    Dictionary<string, ObservableCollection<PlanEntity>> planListDictionary = new Dictionary<string, ObservableCollection<PlanEntity>>();
                    Dictionary<string, int> moduleIndexDictionary = new Dictionary<string, int>();
                    if (GetPorjectPlanList(planListWindow.projectEntity, planListWindow.SelectVersionId, false, ref planListDictionary, ref moduleIndexDictionary))
                    {
                        if (planListDictionary.Count > 0)
                        {
                            ObservableCollection<PlanListViewModel> planListViewModelList = new ObservableCollection<PlanListViewModel>();
                            foreach (KeyValuePair<string, ObservableCollection<PlanEntity>> kv in planListDictionary)
                            {
                                PlanListViewModel planListViewModel = new PlanListViewModel(kv.Key,
                                                                            planListDictionary[kv.Key],
                                                                            moduleIndexDictionary[kv.Key],
                                                                            departmentIdNameDictionary);
                                planListViewModel.IsReadOnly = true;
                                planListViewModelList.Add(planListViewModel);
                            }

                            PlanExtraEntity planExtraEntity = null;
                            IEnumerable<plan_extra> plan_extras = from c in planManagerDomainContext.plan_extras
                                                                  where c.version_id == planListWindow.SelectVersionId
                                                                  && c.manufacture_number == planListWindow.projectEntity.ManufactureNumber.TrimEnd()
                                                                  select c;
                            if (0 != plan_extras.Count())
                            {
                                planExtraEntity = new PlanExtraEntity();
                                planExtraEntity.PlanExtra = plan_extras.First<plan_extra>();
                                planExtraEntity.Update();
                                planExtraEntity.PlanExtra = null;
                            }

                            string title = "历史计划" + planListWindow.projectEntity.ProjectName + " "
                                            + planListWindow.projectEntity.ManufactureNumber.TrimEnd() + " "
                                            + planListWindow.SelectVersionId + " "
                                            + "(红色（超过计划时间）；品红（接近计划时间）；绿色（按时完成）；紫色（超时完成）；灰色（无状态）)";


                            PlanListHistoryWindow planListHistoryWindow = new PlanListHistoryWindow(title, planListViewModelList,
                                                                                planExtraEntity, this,
                                                                                planListWindow.projectEntity,
                                                                                planListWindow.SelectVersionId);
                            planListHistoryWindow.Closed += new EventHandler(HistoryListWindow_Closed);
                            planListHistoryWindow.Show();
                        }
                    }
                    else
                    {
                        string errorMessage = "无相关数据(生产令号：" +
                                                planListWindow.projectEntity.ManufactureNumber.TrimEnd() +
                                                ",版本号" +
                                                planListWindow.SelectVersionId
                                                + ")";
                        Message.ErrorMessage(errorMessage);
                    }
                }
            }
        }

        void PlanListWindow_Closed(object sender, EventArgs e)
        {
            PlanListTraceWindow planListWindow = sender as PlanListTraceWindow;
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
                        if (null != planListWindow.planExtraEntity && null != planListWindow.planExtraEntity.VersionId)
                        {
                            versionId += planListWindow.planExtraEntity.VersionId;
                        }

                        string projectNameKey = null == planListWindow.planExtraEntity ? "所有项目" :
                                                           null != SelectProjectEntity ? SelectProjectEntity.ProjectName : planListWindow.planListViewModelList[0].PlanList[0].ProjectName;
                        projectNameKey += "        ";
                        string manufactureNumber = "生产令号：";
                        if (null != planListWindow.planExtraEntity && null != planListWindow.planExtraEntity.ManufactureNumber)
                        {
                            manufactureNumber += planListWindow.planExtraEntity.ManufactureNumber;
                        }

                        ColumnModel columnModel = new ColumnModel();
                        Dictionary<string, Worksheet> sheetDictionary = new Dictionary<string, Worksheet>();
                        Dictionary<string, int> rowCountDictionary = new Dictionary<string, int>();
                        Worksheet currentSheet = null;
                        foreach(PlanListViewModel model in planListWindow.planListViewModelList)
                        {
                            foreach (PlanEntity planEntity in model.FilterPlanList)
                            {
                                if ("设计完成节点" == planEntity.SheetName)
                                {
                                    planEntity.SheetName = "设计节点";
                                }
                                else if ("采购完成节点" == planEntity.SheetName)
                                {
                                    planEntity.SheetName = "采购节点";
                                }
                                else if ("生产完成节点" == planEntity.SheetName)
                                {
                                    planEntity.SheetName = "生产节点";
                                }
                                if (!sheetDictionary.Keys.Contains(planEntity.SheetName))
                                {
                                    currentSheet = new Worksheet(planEntity.SheetName);
                                    string projectNameName = projectNameKey + planEntity.SheetName;

                                    Int16 rowHeaderCount = 0;
                                    currentSheet.Cells[rowHeaderCount++, 0] = new Cell(versionId);
                                    currentSheet.Cells[rowHeaderCount++, 0] = new Cell(projectNameName);
                                    currentSheet.Cells[rowHeaderCount++, 0] = new Cell(manufactureNumber);

                                    int columnHeaderCount = 0;
                                    if (null == planListWindow.planExtraEntity)
                                    {
                                        currentSheet.Cells[rowHeaderCount, columnHeaderCount++] = new Cell("项目名称");
                                        currentSheet.Cells[rowHeaderCount, columnHeaderCount++] = new Cell("生产令号");
                                        currentSheet.Cells[rowHeaderCount, columnHeaderCount++] = new Cell("版本号");
                                    }

                                    int columnModelIndex = model.ColumnModelIndex;
                                    foreach (string itemColumn in columnModel.List[columnModelIndex])
                                    {
                                        currentSheet.Cells[rowHeaderCount, columnHeaderCount++] = new Cell(itemColumn);
                                    }
                                    ++rowHeaderCount;

                                    sheetDictionary.Add(planEntity.SheetName, currentSheet);
                                    rowCountDictionary.Add(planEntity.SheetName, rowHeaderCount);
                                }
                                else
                                {
                                    currentSheet = sheetDictionary[planEntity.SheetName];
                                }

                                int rowCount = rowCountDictionary[planEntity.SheetName];
                                int columnCount = 0;

                                if (null == planListWindow.planExtraEntity)
                                {
                                    currentSheet.Cells[rowCount, columnCount++] = new Cell(planEntity.ProjectName);
                                    currentSheet.Cells[rowCount, columnCount++] = new Cell(planEntity.ManufactureNumber);
                                    currentSheet.Cells[rowCount, columnCount++] = new Cell(planEntity.VersionId);
                                }

                                string value = Convert.ToString(planEntity.SequenceId);
                                currentSheet.Cells[rowCount, columnCount++] = new Cell(value);

                                currentSheet.Cells[rowCount, columnCount++] = new Cell(planEntity.ComponentName);
                                currentSheet.Cells[rowCount, columnCount++] = new Cell(planEntity.TaskDescription);

                                value = Convert.ToString(planEntity.Weight);
                                currentSheet.Cells[rowCount, columnCount++] = new Cell(value);

                                if (planEntity.Score.HasValue)
                                {
                                    value = Convert.ToString(planEntity.Score.Value);
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                int currentColumnModelIndex = model.ColumnModelIndex;
                                if (1 == currentColumnModelIndex)
                                {
                                    if (planEntity.OrderDate.HasValue)
                                    {
                                        value = planEntity.OrderDate.Value.ToString(String.Format("yyyy-MM-dd"));
                                        currentSheet.Cells[rowCount, columnCount] = new Cell(value);
                                    }
                                    ++columnCount;
                                }

                                value = planEntity.TargetDate.ToString(String.Format("yyyy-MM-dd"));
                                currentSheet.Cells[rowCount, columnCount++] = new Cell(value);

                                if (planEntity.TargetDateAdjustment1.HasValue)
                                {
                                    value = planEntity.TargetDateAdjustment1.Value.ToString(String.Format("yyyy-MM-dd"));
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (planEntity.TargetDateAdjustment2.HasValue)
                                {
                                    value = planEntity.TargetDateAdjustment2.Value.ToString(String.Format("yyyy-MM-dd"));
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (planEntity.AccomplishDate.HasValue)
                                {
                                    value = planEntity.AccomplishDate.Value.ToString(String.Format("yyyy-MM-dd"));
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(value);
                                }
                                ++columnCount;

                                if (null != planEntity.DepartmentName && string.Empty != planEntity.DepartmentName)
                                {
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(planEntity.DepartmentName);
                                }
                                ++columnCount;

                                if (null != planEntity.Remark && string.Empty != planEntity.Remark)
                                {
                                    currentSheet.Cells[rowCount, columnCount] = new Cell(planEntity.Remark);
                                }
                                ++columnCount;

                                ++rowCountDictionary[planEntity.SheetName];
                            }
                        }

                        if(sheetDictionary.Count > 0)
                        {
                            Workbook workbook = new Workbook();
                            foreach (KeyValuePair<string, Worksheet> kv in sheetDictionary)
                            {
                                if (null != planListWindow.planExtraEntity)
                                {
                                    PlanExtraEntity planExtraEntity = planListWindow.planExtraEntity;
                                    currentSheet = kv.Value;
                                    currentSheet.Cells[rowCountDictionary[kv.Key]++, 0] = new Cell("编制依据：" + planExtraEntity.CompilationBasis);
                                    currentSheet.Cells[rowCountDictionary[kv.Key]++, 0] = new Cell("第一次调整原因：" + planExtraEntity.ReasonAdjustment1);
                                    currentSheet.Cells[rowCountDictionary[kv.Key]++, 0] = new Cell("第二次调整原因：" + planExtraEntity.ReasonAdjustment2);

                                    currentSheet.Cells[rowCountDictionary[kv.Key], 0] = new Cell("编制：" + planExtraEntity.CompileUserName);
                                    currentSheet.Cells[rowCountDictionary[kv.Key], 3] = new Cell("审核：" + planExtraEntity.ExamineUserName);
                                    currentSheet.Cells[rowCountDictionary[kv.Key]++, 5] = new Cell("批准：" + planExtraEntity.ApproveUserName);

                                    currentSheet.Cells[rowCountDictionary[kv.Key], 0] = new Cell("日期：" + planExtraEntity.CompileDate);
                                    currentSheet.Cells[rowCountDictionary[kv.Key], 3] = new Cell("日期：" + planExtraEntity.ExamineDate);
                                    currentSheet.Cells[rowCountDictionary[kv.Key]++, 5] = new Cell("日期：" + planExtraEntity.ApproveDate);
                                }

                                workbook.Worksheets.Add(kv.Value);
                            }                        
                                                
                            Stream sFile = sDialog.OpenFile();
                            workbook.Save(sFile);                        
                            Message.InfoMessage("导出成功");
                        }
                        else
                        {
                            Message.ErrorMessage("导出失败：无数据！");
                        }
                    }
                    catch (Exception outputE)
                    {
                        string errorMessage = "导出文件失败：" + outputE.Message;
                        Message.ErrorMessage(errorMessage);
                    }
                }               
            }
            if (planListWindow.DialogResult.HasValue)
            {
                ObservableCollection<string> unFinishedProject = new ObservableCollection<string>();
                foreach (PlanListViewModel planListViewModel in planListWindow.planListViewModelList)
                {
                    foreach (PlanEntity planEntity in planListViewModel.PlanList)
                    {
                        if (null == planEntity.AccomplishDate && !unFinishedProject.Contains(planEntity.ManufactureNumber))
                        {
                            unFinishedProject.Add(planEntity.ManufactureNumber);
                        }
                    }
                }

                if (null != planListWindow.planExtraEntity)
                {
                    if(null != SelectProjectEntity)
                    {
                        SelectProjectEntity.AccomplishMark = (0 == unFinishedProject.Count ? 1 : 0);
                        SelectProjectEntity.DUpdate();
                    }
                }
                else
                {
                    foreach (ProjectEntity projectItem in ProjectList)
                    {
                        if (!string.IsNullOrEmpty(projectItem.PlanVersionID))
                        {
                            projectItem.AccomplishMark = unFinishedProject.Contains(projectItem.ManufactureNumber) ? 0 : 1;
                            projectItem.DUpdate();
                        }
                    }
                }
                SaveChanges();
            }
            (OnViewSinglePlan as DelegateCommand).RaiseCanExecuteChanged();
            (OnSetRemind as DelegateCommand).RaiseCanExecuteChanged();
            (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
            (OnViewSingleHistory as DelegateCommand).RaiseCanExecuteChanged(); 
        }
    }
}
