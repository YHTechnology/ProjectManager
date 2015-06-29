using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using ProductManager.Controls;
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.ViewData.Entity
{
    public class ProjectEntity : NotifyPropertyChanged
    {
        private string manufactureNumber;

        [Required(ErrorMessage = "生产令号必填")]
        [RegularExpressionAttribute(@"[a-zA-Z0-9]*", ErrorMessage = "生产令号只能用大小写字母和数字组成。")]
        public string ManufactureNumber
        {
            get { return manufactureNumber; }
            set
            {
                if (manufactureNumber != value)
                {
                    manufactureNumber = value;
                    UpdateChanged("ManufactureNumber");
                    RemoveError("ManufactureNumber");
                    CheckManufactureNumber();
                }
            }
        }

        private string modelNumber;
        public string ModelNumber
        {
            get { return modelNumber; }
            set
            {
                if (modelNumber != value)
                {
                    modelNumber = value;
                    UpdateChanged("ModelNumber");
                }
            }
        }

        private string projectName;

        [Required(ErrorMessage = "项目名称必填")]
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                if (projectName != value)
                {
                    projectName = value;
                    UpdateChanged("ProjectName");
                    RemoveError("ProjectName");
                }
            }
        }

        private string planVersionID;
        public string PlanVersionID
        {
            get { return planVersionID; }
            set
            {
                if (planVersionID != value)
                {
                    planVersionID = value;
                    UpdateChanged("PlanVersionID");
                }
            }
        }

        public Dictionary<string, string> PlanVersionDictionary { get; set; }
        private bool hasHistory = false;
        public bool HasHistory
        {
            get
            {
                return hasHistory;
            }
            set
            {
                if (hasHistory != value)
                {
                    hasHistory = value;
                    UpdateChanged("HasHistory");
                }
            }
        }

        private Nullable<DateTime> recordDate;
        public Nullable<DateTime> RecordDate
        {
            get { return recordDate; }
            set
            {
                if (recordDate != value)
                {
                    recordDate = value;
                    UpdateChanged("RecordDate");
                }
            }
        }

        private Nullable<DateTime> planUpdateDate;
        public Nullable<DateTime> PlanUpdateDate
        {
            get { return planUpdateDate; }
            set
            {
                if (planUpdateDate != value)
                {
                    planUpdateDate = value;
                    UpdateChanged("PlanUpdateDate");
                }
            }
        }

        public String RecordDateString
        {
            get
            {
                if (recordDate.HasValue)
                {
                    return recordDate.Value.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private string remark;
        public string Remark
        {
            get { return remark; }
            set
            {
                if (remark != value)
                {
                    remark = value;
                    UpdateChanged("Remark");
                }
            }
        }

        private Nullable<int> yearNumber;
        public Nullable<int> YearNumber
        {
            get { return yearNumber; }
            set
            {
                if (yearNumber != value)
                {
                    yearNumber = value;
                    UpdateChanged("YearNumber");
                }
            }
        }

        private Nullable<int> accomplishMark;
        public Nullable<int> AccomplishMark
        {
            get { return accomplishMark; }
            set
            {
                if (accomplishMark != value)
                {
                    accomplishMark = value;
                    UpdateChanged("AccomplishMark");
                    UpdateChanged("AccomplishMarkString");
                }
            }
        }

        public string AccomplishMarkString
        {
            get 
            {
                if (null == accomplishMark || 0 == accomplishMark)
                {
                    return "未完";
                }

                return "完成"; 
            }
        }

        private int projectRemindDay;
        public int ProjectRemindDay
        {
            get { return projectRemindDay; }
            set
            {
                if (projectRemindDay != value)
                {
                    projectRemindDay = value;
                    remindDayDesign = projectRemindDay & 0xff;
                    remindDayPurchase = ((projectRemindDay & 0xff00) >> 8);
                    remindDayProduce = ((projectRemindDay & 0xff0000) >> 16);
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayDesign");
                    UpdateChanged("RemindDayPurchase");
                    UpdateChanged("RemindDayProduce");
                }
            }
        }

        private int remindDayDesign;
        public int RemindDayDesign
        {
            get
            {
                if (UserRemindEntity != null)
                {
                    return UserRemindEntity.RemindOne;
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                if (UserRemindEntity != null && UserRemindEntity.RemindOne != value)
                {
                    UserRemindEntity.RemindOne = value;
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayDesign");
                }
            }
            /*
            get { return remindDayDesign; }
            set
            {
                if (remindDayDesign != value)
                {
                    remindDayDesign = value;
                    projectRemindDay = (projectRemindDay & 0xffff00) + remindDayDesign;
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayDesign");
                }
            }
            */
        }

        private int remindDayPurchase;
        public int RemindDayPurchase
        {
            get
            {
                if (UserRemindEntity != null)
                {
                    return UserRemindEntity.RemindTwo;
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                if (UserRemindEntity != null && UserRemindEntity.RemindTwo != value)
                {
                    UserRemindEntity.RemindTwo = value;
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayPurchase");
                }
            }
            /*
            get { return remindDayPurchase; }
            set
            {
                if (remindDayPurchase != value)
                {
                    remindDayPurchase = value;
                    projectRemindDay = (projectRemindDay & 0xff00ff) + (remindDayPurchase << 8);
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayPurchase");
                }
            }
            */
        }

        private int remindDayProduce;
        public int RemindDayProduce
        {
            get
            {
                if (UserRemindEntity != null)
                {
                    return UserRemindEntity.RemindThree;
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                if (UserRemindEntity != null && UserRemindEntity.RemindThree != value)
                {
                    UserRemindEntity.RemindThree = value;
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayProduce");
                }
            }
            /*
            get { return remindDayProduce; }
            set
            {
                if (remindDayProduce != value)
                {
                    remindDayProduce = value;
                    projectRemindDay = (projectRemindDay & 0x00ffff) + (remindDayProduce << 16);
                    UpdateChanged("ProjectRemindDay");
                    UpdateChanged("RemindDayProduce");
                }
            }
            */
        }

        private bool isDelete;
        public bool IsDelete
        {
            get
            {
                return isDelete;
            }
            set
            {
                if (isDelete != value)
                {
                    isDelete = value;
                    UpdateChanged("IsDelete");
                }
            }
        }

        public String IsDeleteString
        {
            get
            {
                if (isDelete)
                {
                    return "是";
                } 
                else
                {
                    return "否";
                }
            }
        }

        private String deleteDescript;
        public String DeleteDescript
        {
            get
            {
                return deleteDescript;
            }
            set
            {
                if (deleteDescript != value)
                {
                    deleteDescript = value;
                    UpdateChanged("DeleteDescript");
                }
            }
        }

        private string compileUserName;
        public string CompileUserName
        {
            get
            {
                return compileUserName;
            }
            set
            {
                if (compileUserName != value)
                {
                    compileUserName = value;
                    UpdateChanged("CompileUserName");
                }
            }
        }

        private Nullable<DateTime> compileDate;
        public Nullable<DateTime> CompileDate
        {
            get
            {
                return compileDate;
            }
            set
            {
                if (compileDate != value)
                {
                    compileDate = value;
                    UpdateChanged("CompileDate");
                }
            }
        }

        private bool isFreeze;
        public bool IsFreeze
        {
            get
            {
                return isFreeze;
            }
            set
            {
                if (isFreeze != value)
                {
                    isFreeze = value;
                    UpdateChanged("IsFreeze");
                }
                
            }
        }

        public String IsFreezeString
        {
            get
            {
                if (isFreeze)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        private String isFreezeDescript;
        public String IsFreezeDescript
        {
            get
            {
                return isFreezeDescript;
            }
            set
            {
                if (isFreezeDescript != value)
                {
                    isFreezeDescript = value;
                    UpdateChanged("IsFreezeDescript");
                }
            }
        }

        private int deleteUserID;
        public int DeleteUserID
        {
            get
            {
                return deleteUserID;
            }
            set
            {
                if (deleteUserID != value)
                {
                    deleteUserID = value;
                    UpdateChanged("DeleteUserID");
                }
            }
        }

        private int addUserID;
        public int AddUserID
        {
            get
            {
                return addUserID;
            }
            set
            {
                if (addUserID != value)
                {
                    addUserID = value;
                    UpdateChanged("AddUserID");
                }
            }
        }

        private int lastModifyUserID;
        public int LastModifyUserID
        {
            get
            {
                return lastModifyUserID;
            }
            set
            {
                if (lastModifyUserID != value)
                {
                    lastModifyUserID = value;
                    UpdateChanged("LastModifyUserID");
                }
            }
        }

        private bool isUserProject = false;
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
                    OnUserProjectCommand();
                    UpdateChanged("IsUserProject");
                    UpdateChanged("IsUserProjectString");
                }
            }
        }

        public String IsUserProjectString
        {
            get
            {
                if (isUserProject)
                {
                    return "取消自选";
                }
                else
                {
                    return "加入自选";
                }
            }
        }

        private Nullable<DateTime> deliveryTime;
        public Nullable<DateTime> DeliveryTime
        {
            get
            {
                return deliveryTime;
            }
            set
            {
                if (deliveryTime != value)
                {
                    deliveryTime = value;
                    UpdateChanged("DeliveryTime");
                }
            }
        }

        private String contractNumber;
        public String ContractNumber
        {
            get
            {
                return contractNumber;
            }
            set
            {
                if (contractNumber != value)
                {
                    contractNumber = value;
                    UpdateChanged("ContractNumber");
                }
            }
        }

        private Nullable<DateTime> invoiceCompletionTime;
        public Nullable<DateTime> InvoiceCompletionTime
        {
            get
            {
                return invoiceCompletionTime;
            }
            set
            {
                if (invoiceCompletionTime != value)
                {
                    invoiceCompletionTime = value;
                    UpdateChanged("InvoiceCompletionTime");
                }
            }
        }

        public void Update()
        {
            manufactureNumber = Project.manufacture_number;
            modelNumber = Project.model_number;
            projectName = Project.project_name;
            planVersionID = Project.plan_version_id;
            recordDate = Project.record_date;
            planUpdateDate = Project.plan_update_date;
            remark = Project.remark;
            yearNumber = Project.year_number;  
            projectRemindDay = Project.remind_day.HasValue ? Project.remind_day.Value : 0;
            remindDayDesign = projectRemindDay & 0xff;
            remindDayPurchase = ((projectRemindDay & 0xff00) >> 8);
            remindDayProduce = ((projectRemindDay & 0xff0000) >> 16);
            accomplishMark = Project.accomplish_mark;
            isDelete = Project.isdelete.GetValueOrDefault(false);
            deleteDescript = Project.deletedescript;
            isFreeze = Project.isFreeze.GetValueOrDefault(false);
            isFreezeDescript = Project.isFreezedescript;
            deleteUserID = Project.delete_user_id.GetValueOrDefault(-1);
            addUserID = Project.add_user_id.GetValueOrDefault(-1);
            lastModifyUserID = Project.last_modify_user_id.GetValueOrDefault(-1);
            deliveryTime = Project.delivery_time;
            contractNumber = Project.contract_number;
            invoiceCompletionTime = Project.invoice_completion_time;
        }

        public void DUpdate()
        {
            Project.manufacture_number = ManufactureNumber;
            Project.model_number = ModelNumber;
            Project.project_name = ProjectName;
            Project.plan_version_id = PlanVersionID;
            Project.record_date = RecordDate;
            Project.plan_update_date = PlanUpdateDate;
            Project.remark = Remark; ;
            Project.year_number = YearNumber;
            Project.remind_day = ProjectRemindDay;
            Project.accomplish_mark = accomplishMark;
            Project.isdelete = this.isDelete;
            Project.deletedescript = deleteDescript;
            Project.isFreeze = isFreeze;
            Project.isFreezedescript = isFreezeDescript;
            Project.delete_user_id = deleteUserID;
            Project.add_user_id = addUserID;
            Project.last_modify_user_id = lastModifyUserID;
            Project.delivery_time = deliveryTime;
            Project.contract_number = contractNumber;
            Project.invoice_completion_time = invoiceCompletionTime;
            if (UserRemindEntity != null)
            {
                UserRemindEntity.DUpdate();
            }
        }

        public void RaisALL()
        {
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ModelNumber");
            UpdateChanged("ProjectName");
            UpdateChanged("PlanVersionID");
            UpdateChanged("RecordDate");
            UpdateChanged("Remark");
            UpdateChanged("YearNumber");
            UpdateChanged("ProjectRemindDay");
            UpdateChanged("RemindDayDesign");
            UpdateChanged("RemindDayPurchase");
            UpdateChanged("RemindDayProduce");
            UpdateChanged("AccomplishMark");
            UpdateChanged("AccomplishMarkString");
            UpdateChanged("IsDelete");
            UpdateChanged("IsDeleteString");
            UpdateChanged("DeleteDescript");
            UpdateChanged("IsFreeze");
            UpdateChanged("IsFreezeString");
            UpdateChanged("IsFreezeDescript");
            UpdateChanged("DeleteUserID");
            UpdateChanged("AddUserID");
            UpdateChanged("LastModifyUserID");
            UpdateChanged("DeliveryTime");
            UpdateChanged("ContractNumber");
            UpdateChanged("InvoiceCompletionTime");
        }

        private void CheckManufactureNumber()
        {
            if (string.IsNullOrWhiteSpace(this.manufactureNumber)
                || string.IsNullOrWhiteSpace(this.manufactureNumber))
            {
                return;
            }
            if (ProjectEntityDictionary != null)
            {
                ProjectEntity lProjectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(this.manufactureNumber, out lProjectEntityTemp))
                {
                    this.AddError("ManufactureNumber", "生产令号不能重复");
                }
                else
                {
                    this.RemoveError("ManufactureNumber");
                }
            }
        }

        public ProductManager.Web.Model.project Project { get; set; }

        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        public ICommand OnUserProject { get; private set; }

        public ProductManager.Web.Service.ProductDomainContext ProductDomainContext { get; set; }

        public UserProjectEntity UserProjectEntity { get; set; }

        public Dictionary<String, UserProjectEntity> UserProjectEntityDictionary { get; set; }

        public ProductManagersViewModel ProductManagersViewModel { get; set; }

        public ProductManager.Web.Service.PlanManagerDomainContext PlanManagerDomainContext { get; set; }

        public UserRemindEntity UserRemindEntity { get; set;}

        private void OnUserProjectCommand()
        {
            ProductManagersViewModel.IsBusy = true;
            if (!isUserProject)
            {
                ProductDomainContext.user_projects.Remove(UserProjectEntity.UserProject);
            }
            else
            {
                UserProjectEntity = new UserProjectEntity();
                UserProjectEntity.UserProject = new ProductManager.Web.Model.user_project();
                UserProjectEntity.ManufactureNumber = manufactureNumber;
                App app = Application.Current as App;
                UserProjectEntity.UserID = app.UserInfo.UserID;
                UserProjectEntity.DUpdate();
                UserProjectEntity.RaisALL();
                ProductDomainContext.user_projects.Add(UserProjectEntity.UserProject);
            }

            SubmitOperation submitOperation = ProductDomainContext.SubmitChanges();
            submitOperation.Completed += submitOperation_Completed;
        }

        public void SetIsUserProject(bool aIsUserProject)
        {
            isUserProject = aIsUserProject;
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            ProductManagersViewModel.IsBusy = false;
            if (submitOperation.HasError)
            {
                if (!isUserProject)
                {
                    submitOperation.MarkErrorAsHandled();
                    NotifyWindow notifyWindow = new NotifyWindow("错误", "取消自选失败！");
                    notifyWindow.Show();
                }
                else
                {
                    submitOperation.MarkErrorAsHandled();
                    NotifyWindow notifyWindow = new NotifyWindow("错误", "添加自选失败！");
                    notifyWindow.Show();
                }
            }
            else
            {
                if (!isUserProject)
                {
                    NotifyWindow notifyWindow = new NotifyWindow("成功", "取消自选成功！");
                    notifyWindow.Show();
                
                    //isUserProject = false;
                    UserProjectEntityDictionary.Remove(manufactureNumber);
                }
                else
                {
                    NotifyWindow notifyWindow = new NotifyWindow("成功", "添加自选成功！");
                    notifyWindow.Show();
                    //isUserProject = true;
                    UserProjectEntityDictionary.Add(manufactureNumber, UserProjectEntity);
                }
            }
            
        }

        public ProjectEntity()
        {
            OnUserProject = new DelegateCommand(OnUserProjectCommand);
        }

        public void StartRemindEdit()
        {
            if (PlanManagerDomainContext != null && UserRemindEntity == null)
            {
                UserRemindEntity = new UserRemindEntity();
                UserRemindEntity.UserRemind = new ProductManager.Web.Model.user_remind();
                UserRemindEntity.ManufactureNumber = manufactureNumber;
                App app = Application.Current as App;
                UserRemindEntity.UserId = app.UserInfo.UserID;
                UserRemindEntity.DUpdate();
                PlanManagerDomainContext.user_reminds.Add(UserRemindEntity.UserRemind);
            }
        }
    }
}
