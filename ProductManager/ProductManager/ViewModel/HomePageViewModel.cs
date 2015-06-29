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
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Windows.Data.DomainServices;

using ProductManager.Web.Service;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Model;
using ProductManager.Controls;
using ProductManager.Views.ProductManagers;
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.ViewModel
{
    public class HomePageViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext productDomainContext = new ProductDomainContext();
        private PlanManagerDomainContext planManagerDomainContext = new PlanManagerDomainContext();
        private SystemManageDomainContext SystemManageDomainContext = new SystemManageDomainContext();

        public ObservableCollection<PlanEntity> planRemindList { get; set; }
        public ObservableCollection<PlanOutlineFileEntity> planOutLineFileList { get; set; }
        public FileUploader.UserFile UserFile { get; set; }
        private PlanOutlineFileEntity newEntity { get; set; }
        private DependencyObject dependencyObject { get; set; }

        public ObservableCollection<ProjectEntity> projectRecentUpdateList { get; set; }

        private string userName;
        private Dictionary<int, string> departmentIdNameDictionary = new Dictionary<int, string>();

        private DomainCollectionView<project_responsible> projectResponsibleView;
        private DomainCollectionViewLoader<project_responsible> projectResponsibleLoader;
        private EntityList<project_responsible> projectResponsibleSource;
        private ObservableCollection<string> responsibleManufactureCollection = new ObservableCollection<string>();

        private DomainCollectionView<project> projectView;
        private DomainCollectionViewLoader<project> projectLoader;
        private EntityList<project> projectSource;

        private DomainCollectionView<plan> planView;
        private DomainCollectionViewLoader<plan> planLoader;
        private EntityList<plan> planSource;

        public ICommand OnUpdateFile { get; private set; }
        public ICommand OnDeleteFile { get; private set; }
        public ICommand OnRefashFile { get; private set; }

        public ObservableCollection<QuestionTraceEntity> QuestionTraceEntityList { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.questiontrace> questionTraceView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace> questionTraceLoader;
        private EntityList<ProductManager.Web.Model.questiontrace> questionTraceSource;

        public ObservableCollection<QuestionTraceEntity> QuestionTraceEntityTraceList { get; set; }

        private DomainCollectionView<ProductManager.Web.Model.questiontrace> questionTraceTraceView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace> questionTraceTraceLoader;
        private EntityList<ProductManager.Web.Model.questiontrace> questionTraceTraceSource;

        private DomainCollectionView<ProductManager.Web.Model.plan_outline_files> planOutlineFilesView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.plan_outline_files> planOutlineFilesLoader;
        private EntityList<ProductManager.Web.Model.plan_outline_files> planOutlineFileSource;

        private Dictionary<int, DepartmentEntity> DepartmentEntityDictionary;
        private Dictionary<int, UserEntity> UserEntityDictionary;

        private ObservableCollection<UserEntity> UserEntityList { get; set; }
        private ObservableCollection<DepartmentEntity> DepartmentEntityList { get; set; }

        private Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        private Dictionary<String, UserRemindEntity> UserRemindEntityDicationary { get; set; }

        public ICommand OnViewQuestion { get; private set; }
        public ICommand OnAnswerQuestion { get; private set; }
        public ICommand OnViewTraceQuestion { get; private set; }
        public ICommand OnCloseTraceQuestion { get; private set; }

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
                    (OnViewQuestion as DelegateCommand).RaiseCanExecuteChanged();
                    (OnAnswerQuestion as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private QuestionTraceEntity selectQuestionTraceTraceEntity;
        public QuestionTraceEntity SelectQuestionTraceTraceEntity
        {
            get
            {
                return selectQuestionTraceTraceEntity;
            }
            set
            {
                if (selectQuestionTraceTraceEntity != value)
                {
                    selectQuestionTraceTraceEntity = value;
                    (OnViewTraceQuestion as DelegateCommand).RaiseCanExecuteChanged();
                    (OnCloseTraceQuestion as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public HomePageViewModel(DependencyObject aDependencyObject)
        {
            dependencyObject = aDependencyObject;
            DepartmentEntityDictionary = new Dictionary<int, DepartmentEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            UserEntityList = new ObservableCollection<UserEntity>();
            DepartmentEntityList = new ObservableCollection<DepartmentEntity>();
            ProjectEntityDictionary = new Dictionary<string, ProjectEntity>();
            UserRemindEntityDicationary = new Dictionary<string, UserRemindEntity>();

            OnUpdateFile = new DelegateCommand(OnUpdateFileCommand, CanUpdateFileCommand);
            OnDeleteFile = new DelegateCommand(OnDeleteFileCommand, CanDeleteFileCommand);
            OnRefashFile = new DelegateCommand(OnRefashFileCommand);
            OnViewQuestion = new DelegateCommand(OnViewQuestionCommand, CanViewQuestionCommand);
            OnAnswerQuestion = new DelegateCommand(OnAnswerQuestionCommand, CanAnswerQuestionCommand);
            OnViewTraceQuestion = new DelegateCommand(OnViewTraceQuestionCommand, CanViewTraceQuestionCommand);
            OnCloseTraceQuestion = new DelegateCommand(OnCloseTraceQuestionCommand, CanCloseTraceQuestionCommand);
        }

        private void OnViewQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.VIEW, UserEntityList, DepartmentEntityList, SelectQuestionTraceEntity);
            questionTraceWindow.Show();
        }

        private bool CanViewQuestionCommand(object aObject)
        {
            if (SelectQuestionTraceEntity != null)
            {
                return true;
            }
            return false;
        }

        private void OnAnswerQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.ANSWER, UserEntityList, DepartmentEntityList, SelectQuestionTraceEntity);
            questionTraceWindow.Closed += QuestionWindowClosed;
            questionTraceWindow.Show();
        }

        private bool CanAnswerQuestionCommand(object aObject)
        {
            if (SelectQuestionTraceEntity != null)
            {
                return true;
            }
            return false;
        }

        private void OnViewTraceQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.VIEW, UserEntityList, DepartmentEntityList, SelectQuestionTraceTraceEntity);
            questionTraceWindow.Show();
        }

        private bool CanViewTraceQuestionCommand(object aObject)
        {
            if (SelectQuestionTraceTraceEntity != null)
            {
                return true;
            }
            return false;
        }

        private void OnCloseTraceQuestionCommand()
        {
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.CLOSE, UserEntityList, DepartmentEntityList, SelectQuestionTraceTraceEntity);
            questionTraceWindow.Closed += QuestionWindowClosed;
            questionTraceWindow.Show();
        }

        private bool CanCloseTraceQuestionCommand(object aObject)
        {
            if (SelectQuestionTraceTraceEntity != null)
            {
                return true;
            }
            return false;
        }

        private void QuestionWindowClosed(object sender, EventArgs e)
        {
            QuestionTraceWindow questionTraceWindow = sender as QuestionTraceWindow;
            if (questionTraceWindow.DialogResult == true)
            {
                IsBusy = true;
                SubmitOperation submitOperation = productDomainContext.SubmitChanges();
                submitOperation.Completed += submitQuestionOperation_Completed;
            }
        }

        void submitQuestionOperation_Completed(object sender, EventArgs e)
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
            (OnViewQuestion as DelegateCommand).RaiseCanExecuteChanged();
            (OnAnswerQuestion as DelegateCommand).RaiseCanExecuteChanged();
            (OnViewTraceQuestion as DelegateCommand).RaiseCanExecuteChanged();
            (OnCloseTraceQuestion as DelegateCommand).RaiseCanExecuteChanged();

            using (questionTraceView.DeferRefresh())
            {
                questionTraceView.MoveToFirstPage();
            }

            using (questionTraceTraceView.DeferRefresh())
            {
                questionTraceTraceView.MoveToFirstPage();
            }

            IsBusy = false;
        }

        private void OnUpdateFileCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                String fileName = ofd.File.Name;
                foreach (PlanOutlineFileEntity entity in planOutLineFileList)
                {
                    if (entity.fileName == fileName)
                    {
                        NotifyWindow notificationWindow = new NotifyWindow("错误", "已上传相同的文件！");
                        notificationWindow.Show();
                        return;
                    }
                }

                App app = Application.Current as App;
                newEntity = new PlanOutlineFileEntity();
                newEntity.UserID = app.UserInfo.UserID;
                newEntity.UserName = app.UserInfo.UserName;
                newEntity.FileName = fileName;

                UserFile = new FileUploader.UserFile();
                UserFile.FileName = fileName;
                UserFile.FolderName = "PlanOutline";
                UserFile.FileStream = ofd.File.OpenRead();
                newEntity.FileBytes = UserFile.FileStream.Length;

                IsBusy = true;
                UserFile.FinishUpdate += UserFile_FinishUpdate;
                UserFile.Upload("", dependencyObject.Dispatcher);
            }
        }

        private bool CanUpdateFileCommand(Object aObject)
        {
            App app = Application.Current as App;
            return app.UserInfo.GetUerRight(4010000);
        }

        private PlanOutlineFileEntity selectPlanOutlineFileEntity;
        public PlanOutlineFileEntity SelectPlanOutlineFileEntity
        {
            get
            {
                return selectPlanOutlineFileEntity;
            }
            set
            {
                if (selectPlanOutlineFileEntity != value)
                {
                    selectPlanOutlineFileEntity = value;
                    UpdateChanged("SelectPlanOutlineFileEntity");
                    (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private void OnDeleteFileCommand()
        {
            planManagerDomainContext.plan_outline_files.Remove(SelectPlanOutlineFileEntity.PlanFiles);
            SubmitOperation deleteSubmitOperation = planManagerDomainContext.SubmitChanges();
            deleteSubmitOperation.Completed += DeleteSubmitOperation_Completed;

        }

        void DeleteSubmitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (!submitOperation.HasError)
            {
                NotifyWindow notifyWindow = new NotifyWindow("删除", "删除成功");
                notifyWindow.Show();
                using (planOutlineFilesView.DeferRefresh())
                {
                    planOutlineFilesView.MoveToFirstPage();
                }
                
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("删除", "删除失败：" + submitOperation.Error.Message);
                notifyWindow.Show();
            }
        }

        private bool CanDeleteFileCommand(Object aObject)
        {
            if (SelectPlanOutlineFileEntity != null)
            {
                //return true;
                App app = Application.Current as App;
                return app.UserInfo.GetUerRight(4020000);
            }
            return false;
        }

        private void OnRefashFileCommand()
        {
            using (planOutlineFilesView.DeferRefresh())
            {
                planOutlineFilesView.MoveToFirstPage();
            }
        }

        private void UserFile_FinishUpdate(object sender, EventArgs e)
        {
            newEntity.FileUploadTime = DateTime.Now;
            NotifyWindow notificationWindow = new NotifyWindow("上传文件", "上传文件完成！");
            notificationWindow.Show();
            newEntity.DUpdate();
            planManagerDomainContext.plan_outline_files.Add(newEntity.PlanFiles);
            SubmitOperation submitOperation = planManagerDomainContext.SubmitChanges();
            submitOperation.Completed += SubmitOperation_Completed;
        }

        void SubmitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;
            if (!submitOperation.HasError)
            {
                using (planOutlineFilesView.DeferRefresh())
                {
                    planOutlineFilesView.MoveToFirstPage();
                }
                //LoadData();
            }
            IsBusy = false;
        }

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
            DepartmentEntityDictionary.Clear();
            UserEntityDictionary.Clear();
            App app = Application.Current as App;
            planRemindList = new ObservableCollection<PlanEntity>();
            planOutLineFileList = new ObservableCollection<PlanOutlineFileEntity>();
            QuestionTraceEntityList = new ObservableCollection<QuestionTraceEntity>();
            QuestionTraceEntityTraceList = new ObservableCollection<QuestionTraceEntity>();
            projectRecentUpdateList = new ObservableCollection<ProjectEntity>();
            if (null == app.UserInfo)
            {
                IsBusy = false;
                return;
            }
            userName = app.UserInfo.UserName;

            //App app = Application.Current as App;
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

            LoadOperation<ProductManager.Web.Model.project> LoadOperationProject =
                productDomainContext.Load<ProductManager.Web.Model.project>(productDomainContext.GetProjectQuery());
            LoadOperationProject.Completed += loadOperationProject_Completed;
        }

        void loadOperationProject_Completed(object sender, EventArgs e)
        {
            ProjectEntityDictionary.Clear();
            LoadOperation loadOperation = sender as LoadOperation;

            foreach (ProductManager.Web.Model.project project in loadOperation.Entities)
            {
                ProjectEntity projectEnttiy = new ProjectEntity();
                projectEnttiy.Project = project;
                projectEnttiy.Update();
                ProjectEntityDictionary.Add(projectEnttiy.ManufactureNumber, projectEnttiy);
            }

            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                SystemManageDomainContext.Load<ProductManager.Web.Model.department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed_Dictionary;
        }

        void loadOperationDepartment_Completed_Dictionary(object sender, EventArgs e)
        {
            DepartmentEntityDictionary.Clear();
            DepartmentEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmentEntityDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
                DepartmentEntityList.Add(departmentEntity);
            }

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
                departmentIdNameDictionary.Add(department.department_id, department.department_name);
            }

            productDomainContext.project_responsibles.Clear();
            projectResponsibleSource = new EntityList<project_responsible>(productDomainContext.project_responsibles);
            projectResponsibleLoader = new DomainCollectionViewLoader<project_responsible>(
                LoadProjectResponsibleEntities,
                LoadOperationProjectResponsibleCompleted);
            projectResponsibleView = new DomainCollectionView<project_responsible>(projectResponsibleLoader, projectResponsibleSource);
            using (projectResponsibleView.DeferRefresh())
            {
                projectResponsibleView.MoveToFirstPage();
            }

            productDomainContext.questiontraces.Clear();
            questionTraceSource = new EntityList<ProductManager.Web.Model.questiontrace>(productDomainContext.questiontraces);
            questionTraceLoader = new DomainCollectionViewLoader<questiontrace>(
                LoadQuestionTraceEntities,
                LoadOperationQuestionTraceCompleted
                );
            questionTraceView = new DomainCollectionView<ProductManager.Web.Model.questiontrace>(questionTraceLoader, questionTraceSource);
            using (questionTraceView.DeferRefresh())
            {
                questionTraceView.MoveToFirstPage();
            }

            questionTraceTraceSource = new EntityList<ProductManager.Web.Model.questiontrace>(productDomainContext.questiontraces);
            questionTraceTraceLoader = new DomainCollectionViewLoader<questiontrace>(
                LoadQuestionTraceTraceEntities,
                LoadOperationQuestionTraceTraceCompleted
                );
            questionTraceTraceView = new DomainCollectionView<ProductManager.Web.Model.questiontrace>(questionTraceTraceLoader, questionTraceTraceSource);
            using (questionTraceTraceView.DeferRefresh())
            {
                questionTraceTraceView.MoveToFirstPage();
            }

            planOutlineFileSource = new EntityList<ProductManager.Web.Model.plan_outline_files>(planManagerDomainContext.plan_outline_files);
            planOutlineFilesLoader = new DomainCollectionViewLoader<plan_outline_files>(
                LoadPlanOutLineEntities,
                LoadPlanOutLineEntitiesCompleted
                );
            planOutlineFilesView = new DomainCollectionView<ProductManager.Web.Model.plan_outline_files>(planOutlineFilesLoader, planOutlineFileSource);

            using (planOutlineFilesView.DeferRefresh())
            {
                planOutlineFilesView.MoveToFirstPage();
            }
        }

        public String FileFilter { get; set; }

        private LoadOperation<plan_outline_files> LoadPlanOutLineEntities()
        {
            EntityQuery<plan_outline_files> lQuery = planManagerDomainContext.Getplan_outline_filesQuery();
            if (FileFilter != null && FileFilter.Length > 0)
            {
                lQuery = lQuery.Where(c => c.file_name.Contains(FileFilter));
            }

            return planManagerDomainContext.Load(lQuery.SortAndPageBy(planOutlineFilesView));
        }

        private void LoadPlanOutLineEntitiesCompleted(LoadOperation<plan_outline_files> aLoadOperation)
        {
            planOutLineFileList.Clear();
            foreach (plan_outline_files file in aLoadOperation.Entities)
            {
                PlanOutlineFileEntity entity = new PlanOutlineFileEntity();
                entity.PlanFiles = file;
                entity.Update();
                planOutLineFileList.Add(entity);
            }
            UpdateChanged("planOutLineFileList");
            (OnDeleteFile as DelegateCommand).RaiseCanExecuteChanged();
        }

        private LoadOperation<project_responsible> LoadProjectResponsibleEntities()
        {
            EntityQuery<project_responsible> lQuery = productDomainContext.GetProject_responsibleQuery();
            App app = Application.Current as App;
            if (app.UserInfo.IsManager)
            {
                lQuery = lQuery.Where(e => e.department_id == app.UserInfo.DepartmentID);
            }
            else
            {
                lQuery = lQuery.Where(e => e.responsible_persionName == userName);
            }
            //lQuery = lQuery.Where(e => e.responsible_persionName == userName);
            return productDomainContext.Load(lQuery.SortAndPageBy(projectResponsibleView));
        }

        private void LoadOperationProjectResponsibleCompleted(LoadOperation<project_responsible> aLoadOperation)
        {
            responsibleManufactureCollection.Clear();
            foreach (project_responsible pr in aLoadOperation.Entities)
            {
                responsibleManufactureCollection.Add(pr.manufacture_number);
            }

            /*
            if (0 == responsibleManufactureCollection.Count)
            {
                IsBusy = false;
                return;
            }
            */
            planManagerDomainContext.projects.Clear();
            projectSource = new EntityList<project>(planManagerDomainContext.projects);
            projectLoader = new DomainCollectionViewLoader<project>(
                LoadProjectEntities,
                LoadOperationProjectCompleted);
            projectView = new DomainCollectionView<project>(projectLoader, projectSource);
            using (projectView.DeferRefresh())
            {
                projectView.MoveToFirstPage();
            }
        }

        private LoadOperation<questiontrace> LoadQuestionTraceEntities()
        {
            EntityQuery<questiontrace> lQuery = productDomainContext.GetQuestiontraceQuery();
            App app = Application.Current as App;
            if (app.UserInfo.IsManager)
            {
                lQuery = lQuery.Where(e => e.question_hand_departmentid == app.UserInfo.DepartmentID);
            }
            else
            {
                lQuery = lQuery.Where(e => e.question_user_id_main == app.UserInfo.UserID);
            }
            return productDomainContext.Load(lQuery.SortAndPageBy(questionTraceView));
        }

        private void LoadOperationQuestionTraceCompleted(LoadOperation<questiontrace> aLoadOperation)
        {
            QuestionTraceEntityList.Clear();
            foreach (ProductManager.Web.Model.questiontrace questiontrace in aLoadOperation.Entities)
            {
                QuestionTraceEntity questionTraceEntity = new QuestionTraceEntity();
                questionTraceEntity.QuestionTrace = questiontrace;
                questionTraceEntity.Update();
                if (questionTraceEntity.QuestionIsClose)
                {
                    continue;
                }

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
                ProjectEntity lProjectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(questionTraceEntity.ManufactureNumber, out lProjectEntityTemp))
                {
                    questionTraceEntity.ProjectName = lProjectEntityTemp.ProjectName;
                }
                QuestionTraceEntityList.Add(questionTraceEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.questionTraceView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("QuestionTraceEntityList");
            UpdateChanged("CollectionQuestionTraceView");
        }

        private LoadOperation<questiontrace> LoadQuestionTraceTraceEntities()
        {
            EntityQuery<questiontrace> lQuery = productDomainContext.GetQuestiontraceQuery();
            App app = Application.Current as App;
            lQuery = lQuery.Where(e => e.question_owner_id == app.UserInfo.UserID);

            return productDomainContext.Load(lQuery.SortAndPageBy(questionTraceTraceView));
        }

        private void LoadOperationQuestionTraceTraceCompleted(LoadOperation<questiontrace> aLoadOperation)
        {
            QuestionTraceEntityTraceList.Clear();
            foreach (ProductManager.Web.Model.questiontrace questiontrace in aLoadOperation.Entities)
            {
                QuestionTraceEntity questionTraceEntity = new QuestionTraceEntity();
                questionTraceEntity.QuestionTrace = questiontrace;
                questionTraceEntity.Update();
                if (questionTraceEntity.QuestionIsClose)
                {
                    continue;
                }
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
                ProjectEntity lProjectEntityTemp;
                if (ProjectEntityDictionary.TryGetValue(questionTraceEntity.ManufactureNumber, out lProjectEntityTemp))
                {
                    questionTraceEntity.ProjectName = lProjectEntityTemp.ProjectName;
                }
                QuestionTraceEntityTraceList.Add(questionTraceEntity);
            }
            if (aLoadOperation.TotalEntityCount != -1)
            {
                this.questionTraceView.SetTotalItemCount(aLoadOperation.TotalEntityCount);
            }
            UpdateChanged("QuestionTraceEntityList");
            UpdateChanged("CollectionQuestionTraceView");
        }

        private LoadOperation<project> LoadProjectEntities()
        {
            EntityQuery<project> lQuery = planManagerDomainContext.GetProjectQuery();
            lQuery = lQuery.Where(e => !string.IsNullOrEmpty(e.plan_version_id));
            return planManagerDomainContext.Load(lQuery.SortAndPageBy(projectView));
        }

        private void LoadOperationProjectCompleted(LoadOperation<project> aLoadOperation)
        {
            projectRecentUpdateList.Clear();
            DateTime currentDateTime = DateTime.Now;
            foreach (project projectItem in aLoadOperation.Entities)
            {
                if (!projectItem.plan_update_date.HasValue)
                {
                    continue;
                }

                TimeSpan difference = projectItem.plan_update_date.Value - currentDateTime;
                if (difference.Days >= 7)
                {
                    continue;
                }

                ProjectEntity projectEntity = new ProjectEntity();
                projectEntity.Project = projectItem;
                projectEntity.Update();
                projectEntity.Project = null;
                projectRecentUpdateList.Add(projectEntity);
            }

            planManagerDomainContext.plans.Clear();

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

        private LoadOperation<plan> LoadPlanEntities()
        {
            EntityQuery<plan> lQuery = planManagerDomainContext.GetPlanNewQuery();
            lQuery = lQuery.Where(e => !e.accomplish_date.HasValue);
            return planManagerDomainContext.Load(lQuery.SortAndPageBy(projectView));
        }

        private void LoadOperationPlanCompleted(LoadOperation<plan> aLoadOperation)
        {
            planRemindList.Clear();
            App app = Application.Current as App;
            planRemindList = new ObservableCollection<PlanEntity>();
            if (null != app.UserInfo)
            {
                string userName = app.UserInfo.UserName;

                IEnumerable<string> project_responsibles_MN =
                                                      from c in productDomainContext.project_responsibles
                                                      where c.responsible_persionName == app.UserInfo.UserName
                                                      || (app.UserInfo.IsManager ? c.department_id == app.UserInfo.DepartmentID : false)
                                                      select c.manufacture_number;

                if (0 != project_responsibles_MN.Count())
                {
                    foreach (string itemMN in project_responsibles_MN)
                    {
                        IEnumerable<project> projects =
                                                      from c in planManagerDomainContext.projects
                                                      where c.manufacture_number == itemMN
                                                      select c;
                        if (0 == projects.Count())
                        {
                            continue;
                        }

                        project projectInfo = projects.First<project>();
                        if (string.IsNullOrEmpty(projectInfo.plan_version_id)
                            || (projectInfo.isdelete.HasValue && projectInfo.isdelete.Value))
                        {
                            continue;
                        }
                        ProjectEntity projectDetail = new ProjectEntity();
                        projectDetail.Project = projectInfo;
                        projectDetail.Update();

                        UserRemindEntity lUserRemindEntity;
                        if (UserRemindEntityDicationary.TryGetValue(projectDetail.ManufactureNumber, out lUserRemindEntity))
                        {
                            projectDetail.UserRemindEntity = lUserRemindEntity;
                        }

                        IEnumerable<string> respUserName = from c in productDomainContext.project_responsibles
                                                           where c.manufacture_number == itemMN && c.department_id == app.UserInfo.DepartmentID
                                                           select c.responsible_persionName;

                        String respUserNameString = respUserName.First();

                        IEnumerable<plan> plans;
                        if (app.UserInfo.DepartmentID != 1000)
                        {
                            plans = from c in planManagerDomainContext.plans
                                    where c.manufacture_number == itemMN && c.department_id == app.UserInfo.DepartmentID
                                              && c.version_id == projectInfo.plan_version_id
                                    select c;
                        }
                        else
                        {
                            plans = from c in planManagerDomainContext.plans
                                    where c.manufacture_number == itemMN && c.version_id == projectInfo.plan_version_id
                                    select c;
                        }
                        //IEnumerable<plan> plans = from c in planManagerDomainContext.plans
                        //                          where c.manufacture_number == itemMN && c.department_id == app.UserInfo.DepartmentID
                        //                                    && c.version_id == projectInfo.plan_version_id
                        //                          select c;
                        foreach (plan planItem in plans)
                        {
                            if (planItem.accomplish_date.HasValue)
                            {
                                continue;
                            }

                            int remindDay = ("设计节点" == planItem.sheet_name) ? projectDetail.RemindDayDesign :
                                (("采购节点" == planItem.sheet_name) ? projectDetail.RemindDayPurchase : projectDetail.RemindDayProduce);

                            Nullable<DateTime> targetDateTime = planItem.target_date_adjustment2.HasValue ? planItem.target_date_adjustment2.Value :
                                                (planItem.target_date_adjustment1.HasValue ? planItem.target_date_adjustment1.Value : planItem.target_date);

                            if (!targetDateTime.HasValue)
                            {
                                continue;
                            }

                            DateTime currentDateTime = DateTime.Now;
                            TimeSpan difference = targetDateTime.Value - currentDateTime;

                            if (difference.Days > remindDay)
                            {
                                continue;
                            }

                            PlanEntity planEntity = new PlanEntity();
                            planEntity.Plan = planItem;
                            planEntity.Update();
                            planEntity.Plan = null;
                            planEntity.PlanRemindDay = remindDay;
                            planEntity.ProjectName = projectInfo.project_name;
                            planEntity.TargetDate = targetDateTime.Value;
                            planEntity.RespUserName = respUserNameString;
                            string getDepartmentName;
                            if (departmentIdNameDictionary.TryGetValue(planEntity.DepartmentId, out getDepartmentName))
                            {
                                planEntity.DepartmentName = getDepartmentName;
                            }
                            planRemindList.Add(planEntity);
                        }
                    }
                }
            }
            UpdateChanged("planRemindList");
            IsBusy = false;
        }
    }
}
