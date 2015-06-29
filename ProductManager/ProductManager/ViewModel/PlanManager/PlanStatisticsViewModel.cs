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

using ProductManager.Web.Service;
using ProductManager.ViewData.Entity;
using ProductManager.Views.PlanManager;
using ProductManager.Controls;
using ProductManager.Web.Model;

namespace ProductManager.ViewModel.PlanManager
{
    [Export("PlanStatistics")]
    public class PlanStatisticsViewModel : NotifyPropertyChanged
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
            departmentIdNameDictionary.Add(0, "所有部门");

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
            (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
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
                    (OnViewSinglePlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewAllPlan as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public PlanStatisticsViewModel()
        {
            ProjectList = new ObservableCollection<ProjectEntity>();

            departmentIdNameDictionary = new Dictionary<int, string>();

            OnViewSinglePlan = new DelegateCommand(OnViewSinglePlanCommand, CanViewSinglePlan);
            OnViewAllPlan = new DelegateCommand(OnViewAllPlanCommand, CanViewAllPlan);
        }

        public ICommand OnViewSinglePlan { get; private set; }

        public ICommand OnViewAllPlan { get; private set; }

        private void OnViewSinglePlanCommand()
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
                ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();           
                if (GetPorjectPlanList(SelectProjectEntity, ref planList))
                {
                    if (planList.Count > 0)
                    {
                        string title = SelectProjectEntity.ProjectName + " " + SelectProjectEntity.ManufactureNumber.TrimEnd() + " " + SelectProjectEntity.PlanVersionID;
                        PlanListViewModel planListViewModel = new PlanListViewModel(title, planList, 0, departmentIdNameDictionary);
                        planListViewModel.IsReadOnly = true;
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

                        PlanListStatisticsWindow planListStatisticsWindow = new PlanListStatisticsWindow("查询计划", planListViewModel, planExtraEntity);
                        planListStatisticsWindow.Show();
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
    
        private bool CanViewSinglePlan(object aObject)
        {
            return null != SelectProjectEntity
                && null != SelectProjectEntity.PlanVersionID
                && null != SelectProjectEntity.ManufactureNumber
                && string.Empty != SelectProjectEntity.PlanVersionID
                && string.Empty != SelectProjectEntity.ManufactureNumber;
        }

        private void OnViewAllPlanCommand()
        {
            IsBusy = true;

            ObservableCollection<PlanEntity> planList = new ObservableCollection<PlanEntity>();
            foreach (ProjectEntity item in ProjectList)
            {
                if (null == item.PlanVersionID
                || null == item.ManufactureNumber
                || string.Empty == item.PlanVersionID
                || string.Empty == item.ManufactureNumber)
                {
                    continue;
                }

                GetPorjectPlanList(item, ref planList);                
            }

            if (planList.Count > 0)
            {
                PlanListViewModel planListViewModel = new PlanListViewModel("所有项目计划", planList, 0, departmentIdNameDictionary);
                planListViewModel.IsReadOnly = true;
                PlanExtraEntity planExtraEntity = null;
           
                PlanListStatisticsWindow planListStatisticsWindow = new PlanListStatisticsWindow("查询计划", planListViewModel, planExtraEntity);
                planListStatisticsWindow.Show();
            }

            IsBusy = false;
        }

        private bool GetPorjectPlanList(ProjectEntity item, ref ObservableCollection<PlanEntity> planList)
        {
            bool rturnValue = false;
            IEnumerable<string> sheetNames = from c in planManagerDomainContext.plans
                                             where c.manufacture_number == item.ManufactureNumber
                                                && c.version_id == item.PlanVersionID
                                             select c.sheet_name;
            if (sheetNames.Count() > 0)
            {
                ObservableCollection<string> differentSheets = new ObservableCollection<string>();
                foreach (string value in sheetNames)
                {
                    if (null == value || differentSheets.Contains(value))
                    {
                        continue;
                    }
                    differentSheets.Add(value);
                    IEnumerable<plan> selectedPlans = from c in planManagerDomainContext.plans
                                                      where c.manufacture_number == item.ManufactureNumber
                                                        && c.version_id == item.PlanVersionID
                                                        && c.sheet_name == value
                                                      select c;
                    foreach (plan planItem in selectedPlans)
                    {
                        PlanEntity planEntity = new PlanEntity();
                        planEntity.Plan = planItem;
                        string getDepartmentName = string.Empty;
                        planEntity.Update();
                        planEntity.ProjectName = item.ProjectName;
                        if (departmentIdNameDictionary.TryGetValue(planEntity.DepartmentId, out getDepartmentName))
                        {
                            planEntity.DepartmentName = getDepartmentName;
                        }
                        planList.Add(planEntity);
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
    }
}
