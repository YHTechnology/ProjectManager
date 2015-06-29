using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.ProductManagers
{
    [Export("QuestionTraceList")]
    public class QuestionTraceListViewModel : NotifyPropertyChanged
    {
        private ProductDomainContext ProductDomainContext = new ProductDomainContext();
        private SystemManageDomainContext SystemManageDomainContext = new SystemManageDomainContext();

        public ObservableCollection<QuestionTraceEntity> QuestionTraceEntityList { get; set; }
        public ObservableCollection<DepartmentEntity> DepartmentEntityList { get; set; }

        public String ManufactureNumberSearch { get; set; }
        public String QuestionContext { get; set; }
        public Nullable<int> QuestionRspDepartmentID { get; set; }
        public Nullable<int> QuestionTraceDepartmentID { get; set; }
        public Nullable<DateTime> QuestionStartTime { get; set; }
        public Nullable<DateTime> QuestionEndTime { get; set; }
        public Nullable<DateTime> AnswerStartTime { get; set; }
        public Nullable<DateTime> AnswerEndTime { get; set; }

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

        private DomainCollectionView<ProductManager.Web.Model.questiontrace> questionTraceView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace> questionTraceLoader;
        private EntityList<ProductManager.Web.Model.questiontrace> questionTraceSource;

        private Dictionary<int, DepartmentEntity> DepartmentEntityDictionary;
        private Dictionary<int, UserEntity> UserEntityDictionary;

        public ICommand OnSearch { get; private set; }

        public QuestionTraceListViewModel()
        {
            QuestionTraceEntityList = new ObservableCollection<QuestionTraceEntity>();
            DepartmentEntityList = new ObservableCollection<DepartmentEntity>();

            this.questionTraceSource = new EntityList<ProductManager.Web.Model.questiontrace>(this.ProductDomainContext.questiontraces);
            this.questionTraceLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.questiontrace>(
                this.LoadQuestionTraceEntities,
                this.LoadOperationQuestionTraceCompleted);
            this.questionTraceView = new DomainCollectionView<ProductManager.Web.Model.questiontrace>(this.questionTraceLoader, this.questionTraceSource);

            DepartmentEntityDictionary = new Dictionary<int, DepartmentEntity>();
            UserEntityDictionary = new Dictionary<int, UserEntity>();
            OnSearch = new DelegateCommand(OnSearchCommand);
        }

        public void LoadData()
        {
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                SystemManageDomainContext.Load<ProductManager.Web.Model.department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        void loadOperationDepartment_Completed(object sender, EventArgs e)
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

            DepartmentEntity departmentEntityZero = new DepartmentEntity();
            departmentEntityZero.DepartmentID = 0;
            departmentEntityZero.DepartmentName = "所有部门";
            DepartmentEntityList.Add(departmentEntityZero);

            LoadOperation<ProductManager.Web.Model.user> loadOperationUser =
                SystemManageDomainContext.Load<ProductManager.Web.Model.user>(SystemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            UserEntityDictionary.Clear();
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
            }
            using (questionTraceView.DeferRefresh())
            {
                questionTraceView.MoveToFirstPage();
            }
        }

        private LoadOperation<ProductManager.Web.Model.questiontrace> LoadQuestionTraceEntities()
        {

            this.IsBusy = true;
            EntityQuery<ProductManager.Web.Model.questiontrace> lQuery = this.ProductDomainContext.GetQuestiontraceQuery();

            if (!String.IsNullOrEmpty(QuestionContext))
            {
                lQuery = lQuery.Where(c => c.question_descript.ToLower().Contains(QuestionContext.ToLower()));
            }

            if (QuestionRspDepartmentID.HasValue && QuestionRspDepartmentID.Value != 0)
            {
                lQuery = lQuery.Where(c => c.question_res_departmentid == QuestionRspDepartmentID.Value);
            }

            if (QuestionTraceDepartmentID.HasValue && QuestionTraceDepartmentID.Value != 0)
            {
                lQuery = lQuery.Where(c => c.question_trace_departmendid == QuestionTraceDepartmentID.Value);
            }

            if (QuestionStartTime.HasValue)
            {
                lQuery = lQuery.Where(c => c.question_starttime >= QuestionStartTime.Value);
            }
            
            if (QuestionEndTime.HasValue)
            {
                lQuery = lQuery.Where(c => c.question_starttime <= QuestionEndTime.Value);
            }

            if (AnswerStartTime.HasValue)
            {
                lQuery = lQuery.Where(c => c.question_lastanswertime >= AnswerStartTime.Value);
            }
            
            if (AnswerEndTime.HasValue)
            {
                lQuery = lQuery.Where(c => c.question_lastanswertime <= AnswerEndTime.Value);
            }

            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.questionTraceView));

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

        public void OnSearchCommand()
        {
            using (questionTraceView.DeferRefresh())
            {
                questionTraceView.MoveToFirstPage();
            }
        }
    }
}
