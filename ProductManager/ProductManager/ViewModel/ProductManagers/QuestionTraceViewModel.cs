using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using Microsoft.Windows.Data.DomainServices;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Views.ProductManagers;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("QuestionTrace")]
    public class QuestionTraceViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext;// = new ProductDomainContext();
        private SystemManageDomainContext SystemManageDomainContext;// = new SystemManageDomainContext();

        private DomainCollectionView<ProductManager.Web.Model.project> projectView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.project> projectLoader;
        private EntityList<ProductManager.Web.Model.project> prjectSource;

        private DomainCollectionView<ProductManager.Web.Model.questiontrace> questionTraceView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace> questionTraceLoader;
        private EntityList<ProductManager.Web.Model.questiontrace> questionTraceSource;

        private Dictionary<int, DepartmentEntity> DepartmentEntityDictionary;
        private Dictionary<int, UserEntity> UserEntityDictionary;

        public ObservableCollection<ProjectEntity> ProjectEntityList { get; set; }
        public ObservableCollection<QuestionTraceEntity> QuestionTraceEntityList { get; set; }
        public ObservableCollection<DepartmentEntity> DepartmentList { get; set; }
        public ObservableCollection<UserEntity> UserEntityList { get; set; }

        public Dictionary<String, UserProjectEntity> UserProjectEntityDictionary { get; set; }

        private int UserProjectCount;

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

        private ProjectEntity selectProjectEntity;
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
                    SelectQuestionTraceEntity = null;
                    (OnAddQuestion as DelegateCommand).RaiseCanExecuteChanged();
                    using (this.questionTraceView.DeferRefresh())
                    {
                        //this.questionTraceView.SetTotalItemCount(-1);
                        this.questionTraceView.MoveToFirstPage();
                    }
                }
            }
        }

        private QuestionTraceEntity selectQuestionTraceEntity;
        public QuestionTraceEntity SelectQuestionTraceEntity
        {
            get
            {
                return selectQuestionTraceEntity;
            }
            set
            {
                if (selectQuestionTraceEntity != value)
                {
                    selectQuestionTraceEntity = value;
                    UpdateChanged("SelectQuestionTraceEntity");
                    (OnAnswerQuestion as DelegateCommand).RaiseCanExecuteChanged();
                    (OnViewQuestion as DelegateCommand).RaiseCanExecuteChanged();
                    (OnCloseQuestion as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        
        
        public ICollectionView CollectionProjectView
        {
            get
            {
                return projectView;
            }
        }

        public ICollectionView CollectionQuestionTraceView
        {
            get
            {
                return questionTraceView;
            }
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

        public ICommand OnAddQuestion { get; private set; }
        public ICommand OnAnswerQuestion { get; private set; }
        public ICommand OnViewQuestion { get; private set; }
        public ICommand OnCloseQuestion { get; private set; }

        public ICommand OnRefash { get; private set; }

        private void OnCloseQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.CLOSE, UserEntityList, DepartmentList, SelectQuestionTraceEntity);
            questionTraceWindow.Closed += AnswerQuestionClosed;
            questionTraceWindow.Show();
        }

        private bool CanCloseQuestionCommand(object aObject)
        {
            if(SelectQuestionTraceEntity !=null && !SelectQuestionTraceEntity.QuestionIsClose)
            {
                App app = Application.Current as App;
                if (SelectQuestionTraceEntity.OwnerUserID == app.UserInfo.UserID)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefashCommand()
        {
            SelectProjectEntity = null;
            SelectQuestionTraceEntity = null;
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }
        }

        public ICommand DoubleClickProject { get; private set; }

        private void DoubleClickProjectCommand()
        {
            ShowExpander = true;
        }

        public QuestionTraceViewModel()
        {
            DepartmentEntityDictionary = new Dictionary<int, DepartmentEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            ProjectEntityList = new ObservableCollection<ProjectEntity>();
            QuestionTraceEntityList = new ObservableCollection<QuestionTraceEntity>();
            DepartmentList = new ObservableCollection<DepartmentEntity>();
            UserEntityList = new ObservableCollection<UserEntity>();
            UserProjectEntityDictionary = new Dictionary<String, UserProjectEntity>();

            OnAddQuestion = new DelegateCommand(OnAddQuestionCommand, CanAddQuestionCommand);
            OnAnswerQuestion = new DelegateCommand(OnAnswerQuestionCommand, CanAnswerQuestionCommand);
            OnViewQuestion = new DelegateCommand(OnViewQuestionCommand, CanViewQuestionCommand);
            OnCloseQuestion = new DelegateCommand(OnCloseQuestionCommand, CanCloseQuestionCommand);
            OnRefash = new DelegateCommand(OnRefashCommand);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);

            FilterList = new ObservableCollection<string>();
            FilterList.Add("生产令号");
            FilterList.Add("项目名称");
            FilterList.Add("备注");
            FilterList.Add("年份");
            FilterList.Add("记录时间");
            selectFilerList = "生产令号";
        }

        private QuestionTraceEntity AddQuestionTraceEntity { get; set; }

        private void OnAddQuestionCommand()
        {
            AddQuestionTraceEntity = new QuestionTraceEntity();
            AddQuestionTraceEntity.ManufactureNumber = SelectProjectEntity.ManufactureNumber;
            AddQuestionTraceEntity.UserIDMain = -2;
            AddQuestionTraceEntity.UserIDCP1 = -2;
            AddQuestionTraceEntity.UserIDCP2 = -2;
            AddQuestionTraceEntity.QuestionTrace = new ProductManager.Web.Model.questiontrace();
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.ADD, UserEntityList, DepartmentList, AddQuestionTraceEntity);
            questionTraceWindow.Closed += AddQuestionClosed;
            questionTraceWindow.Show();
        }

        private void AddQuestionClosed(object sender, EventArgs e)
        {
            QuestionTraceWindow questionTraceWindow = sender as QuestionTraceWindow;
            if (questionTraceWindow.DialogResult == true)
            {
                QuestionTraceEntityList.Add(AddQuestionTraceEntity);
                ProductDomainContext.questiontraces.Add(AddQuestionTraceEntity.QuestionTrace);
                IsBusy = true;
                SubmitOperation submitOperation = ProductDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
            }
            (OnAnswerQuestion as DelegateCommand).RaiseCanExecuteChanged();
            (OnViewQuestion as DelegateCommand).RaiseCanExecuteChanged();
            (OnCloseQuestion as DelegateCommand).RaiseCanExecuteChanged();
            IsBusy = false;
        }

        private bool CanAddQuestionCommand(object aObject)
        {
            if (selectProjectEntity == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnAnswerQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.ANSWER, UserEntityList, DepartmentList, SelectQuestionTraceEntity);
            questionTraceWindow.Closed += AnswerQuestionClosed;
            questionTraceWindow.Show();
        }

        private void AnswerQuestionClosed(object sender, EventArgs e)
        {
            QuestionTraceWindow questionTraceWindow = sender as QuestionTraceWindow;
            if (questionTraceWindow.DialogResult == true)
            {
                IsBusy = true;
                SubmitOperation submitOperation = ProductDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        private bool CanAnswerQuestionCommand(object aObject)
        {
            if (SelectQuestionTraceEntity != null && !SelectQuestionTraceEntity.QuestionIsClose)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnViewQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.VIEW, UserEntityList, DepartmentList, SelectQuestionTraceEntity);
            questionTraceWindow.Show();
        }

        private bool CanViewQuestionCommand(object aObject)
        {
            if (selectQuestionTraceEntity == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void LoadData()
        {
            IsBusy = true;
            ProductDomainContext = new ProductDomainContext();
            SystemManageDomainContext = new SystemManageDomainContext();
            ProductDomainContext.RejectChanges();
            selectQuestionTraceEntity = null;
            selectProjectEntity = null;

            App app = Application.Current as App;
            LoadOperation<ProductManager.Web.Model.user_project> loadOperationUserProject =
               ProductDomainContext.Load<ProductManager.Web.Model.user_project>(ProductDomainContext.GetUserProjectQuery(app.UserInfo.UserID));
            loadOperationUserProject.Completed += LoadUserProjectComplete;
        }

        void LoadUserProjectComplete(object sender, EventArgs e)
        {
            UserProjectEntityDictionary.Clear();

            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user_project user_project in loadOperation.Entities)
            {
                UserProjectEntity lUserProjectEntity = new UserProjectEntity();
                lUserProjectEntity.UserProject = user_project;
                lUserProjectEntity.Update();
                UserProjectEntity lUserProjectEntityTemp;
                if (!UserProjectEntityDictionary.TryGetValue(lUserProjectEntity.ManufactureNumber, out lUserProjectEntityTemp))
                {
                    UserProjectEntityDictionary.Add(lUserProjectEntity.ManufactureNumber, lUserProjectEntity);
                }

            }

            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                SystemManageDomainContext.Load<ProductManager.Web.Model.department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            DepartmentEntityDictionary.Clear();
            DepartmentList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmentEntityDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
                DepartmentList.Add(departmentEntity);
            }

            DepartmentEntity departmentEntityZero = new DepartmentEntity();
            departmentEntityZero.DepartmentID = 0;
            departmentEntityZero.DepartmentName = "请选择部门";
            DepartmentList.Add(departmentEntityZero);

            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                SystemManageDomainContext.Load<ProductManager.Web.Model.user>(SystemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
            UserEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOperation.Entities)
            {
                if (user.user_name == "admin")
                {
                    continue;
                }
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                UserEntityDictionary.Add(userEntity.UserID, userEntity);
                UserEntityList.Add(userEntity);
            }

            UserEntity userEntityZero = new UserEntity();
            userEntityZero.UserID = -2;
            userEntityZero.CUserName = "请选择用户";
            UserEntityList.Add(userEntityZero);


            this.prjectSource = new EntityList<ProductManager.Web.Model.project>(this.ProductDomainContext.projects);
            this.projectLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.project>(
                this.LoadProjectEntities,
                this.LoadOperationProjectCompleted);
            this.projectView = new DomainCollectionView<ProductManager.Web.Model.project>(this.projectLoader, this.prjectSource);
            using (this.CollectionProjectView.DeferRefresh())
            {
                this.projectView.MoveToFirstPage();
            }

            this.questionTraceSource = new EntityList<ProductManager.Web.Model.questiontrace>(this.ProductDomainContext.questiontraces);
            this.questionTraceLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace>(
                this.LoadQuestionTraceEntities,
                this.LoadOperationQuestionTraceCompleted);
            this.questionTraceView = new DomainCollectionView<ProductManager.Web.Model.questiontrace>(this.questionTraceLoader, this.questionTraceSource);

            this.IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.project> LoadProjectEntities()
        {
            if (!isFilter)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.ProductDomainContext.GetProjectQuery();
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.project> lQuery = this.ProductDomainContext.GetProjectQuery();
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
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.projectView));
            }
            
        }

        private void LoadOperationProjectCompleted(LoadOperation<ProductManager.Web.Model.project> aLoadOperation)
        {
            ProjectEntityList.Clear();
            UserProjectCount = 0;
            foreach (ProductManager.Web.Model.project project in aLoadOperation.Entities)
            {
                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = project;
                projectEntity.Update();

                UserProjectEntity lUserProjectEntity;
                if (UserProjectEntityDictionary.TryGetValue(projectEntity.ManufactureNumber, out lUserProjectEntity))
                {
                    projectEntity.UserProjectEntity = lUserProjectEntity;
                    projectEntity.SetIsUserProject(true);
                }
                if (IsUserProject && !projectEntity.IsUserProject)
                {
                    continue;
                }
                ProjectEntityList.Add(projectEntity);
                UserProjectCount++;
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.projectView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("ProjectEntityList");
            UpdateChanged("CollectionProjectView");
            UpdateChanged("RecorderCount");
            this.IsBusy = false;
        }

        private LoadOperation<ProductManager.Web.Model.questiontrace> LoadQuestionTraceEntities()
        {
            if (SelectProjectEntity != null)
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.questiontrace> lQuery = this.ProductDomainContext.GetQuestiontraceQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == SelectProjectEntity.ManufactureNumber);
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.questionTraceView));
            }
            else
            {
                this.IsBusy = true;
                EntityQuery<ProductManager.Web.Model.questiontrace> lQuery = this.ProductDomainContext.GetQuestiontraceQuery();
                lQuery = lQuery.Where(e => e.manufacture_number == "");
                return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.questionTraceView));
            }
        }

        private void LoadOperationQuestionTraceCompleted(LoadOperation<ProductManager.Web.Model.questiontrace> aLoadOperation)
        {
            QuestionTraceEntityList.Clear();
            foreach (ProductManager.Web.Model.questiontrace questiontrace in aLoadOperation.Entities)
            {
                QuestionTraceEntity questionTraceEntity = new QuestionTraceEntity();
                questionTraceEntity.QuestionTrace = questiontrace;
                questionTraceEntity.Update();
                DepartmentEntity lDepartmentTemp;
                if (DepartmentEntityDictionary.TryGetValue(questionTraceEntity.QuestionResDepartmentID, out lDepartmentTemp))
                {
                    questionTraceEntity.QuestionResDepartmentName = lDepartmentTemp.DepartmentName;
                }
                if (DepartmentEntityDictionary.TryGetValue(questionTraceEntity.QuestionHandDepartmentID, out lDepartmentTemp))
                {
                    questionTraceEntity.QuestionHandDepartmentName = lDepartmentTemp.DepartmentName;
                }
                if (DepartmentEntityDictionary.TryGetValue(questionTraceEntity.QuestionTraceDepartmentID, out lDepartmentTemp))
                {
                    questionTraceEntity.QuestionTraceDepartmentName = lDepartmentTemp.DepartmentName;
                }
                UserEntity lUserEntityTemp;
                if (UserEntityDictionary.TryGetValue(questionTraceEntity.UserIDMain, out lUserEntityTemp))
                {
                    questionTraceEntity.UserIDMainString = lUserEntityTemp.CUserName;
                }
                if (UserEntityDictionary.TryGetValue(questionTraceEntity.UserIDCP1, out lUserEntityTemp))
                {
                    questionTraceEntity.UserIDCP1String = lUserEntityTemp.CUserName;
                }
                if (UserEntityDictionary.TryGetValue(questionTraceEntity.UserIDCP2, out lUserEntityTemp))
                {
                    questionTraceEntity.UserIDCP2String = lUserEntityTemp.CUserName;
                }
                if (UserEntityDictionary.TryGetValue(questionTraceEntity.OwnerUserID, out lUserEntityTemp))
                {
                    questionTraceEntity.OwnerUserString = lUserEntityTemp.CUserName;
                }
                if (UserEntityDictionary.TryGetValue(questionTraceEntity.LastAnswerUserID, out lUserEntityTemp))
                {
                    questionTraceEntity.LastAnswerUserString = lUserEntityTemp.CUserName;
                }
                QuestionTraceEntityList.Add(questionTraceEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.questionTraceView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("QuestionTraceEntityList");
            UpdateChanged("CollectionQuestionTraceView");
            this.IsBusy = false;
        }

        private bool showExpander = false;
        public bool ShowExpander
        {
            get
            {
                return showExpander;
            }
            set
            {
                //if (showExpander != value)
                {
                    showExpander = value;
                    UpdateChanged("ShowExpander");
                }
            }
        }

        public void HideExpander()
        {
            ShowExpander = false;
        }

        public String RecorderCount
        {
            get
            {
                if (projectView != null)
                {
                    return "总共 " + UserProjectCount.ToString() + " 个项目";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
