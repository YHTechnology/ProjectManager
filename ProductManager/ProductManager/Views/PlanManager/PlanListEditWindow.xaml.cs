using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using ProductManager.ViewModel.PlanManager;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Model;
using ProductManager.Web.Service;
using System.ServiceModel.DomainServices.Client;
using ProductManager.Views.ProductManagers;
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListEditWindow : ChildWindow
    {
        public ObservableCollection<PlanListViewModel> planListViewModelList;
        public PlanExtraEntity planExtraEntity;
        public string VersionId { get; set; }

        public ObservableCollection<DepartmentEntity> DepartmentEntityList = new ObservableCollection<DepartmentEntity>();
        public ObservableCollection<UserEntity> UserEntityList = new ObservableCollection<UserEntity>();

        private ProductDomainContext ProductDomainContext = new ProductDomainContext();
        private SystemManageDomainContext SystemManageDomainContext = new SystemManageDomainContext();

        public String ManufactureNumber { get; set; }

        public PlanListEditWindow(string aTitle, string aOKContent, string aVersionId,
            ObservableCollection<PlanListViewModel> aPlanListViewModelList, PlanExtraEntity aPlanExtraEntity)
        {
            InitializeComponent();
           
            this.Title = aTitle;
            this.VersionId = aVersionId;
            this.OKButton.Content = aOKContent;
            this.planListViewModelList = aPlanListViewModelList;
            this.planExtraEntity = aPlanExtraEntity;

            for(int pos = 0; pos < planListViewModelList.Count; ++pos)
            {
                PlanListViewModel planListViewModel =  planListViewModelList[pos];
                AddTabItem(planListViewModel.Title, planListViewModel);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasEditing = false;
            bool hasChanged = false;
            for (int pos = 0; pos < planListViewModelList.Count; ++pos)
            {
                PlanListViewModel planListViewModel = planListViewModelList[pos];
                if (planListViewModel.IsEditing)
                {
                    hasEditing = true;
                }

                if (planListViewModel.IsChanged)
                {
                    hasChanged = true;
                }
            }

            if (hasEditing)
            {
                CustomMessage customMesage = new CustomMessage("存在正在编辑计划，自动保存?", CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(EditingConfirm_Closed);
                customMesage.Show();
            }
            else
            {
                if (hasChanged)
                {
                    this.DialogResult = true;
                }
                else
                {
                    CustomMessage customMesage = new CustomMessage("无修改", CustomMessage.MessageType.Info);
                    customMesage.Show();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasEditing = false;
            bool hasChanged = false;
            for (int pos = 0; pos < planListViewModelList.Count; ++pos)
            {
                PlanListViewModel planListViewModel = planListViewModelList[pos];
                if (planListViewModel.IsEditing)
                {
                    hasEditing = true;
                }

                if (planListViewModel.IsChanged)
                {
                    hasChanged = true;
                }
            }

            if (hasEditing || hasChanged)
            {
                CustomMessage customMesage = new CustomMessage("放弃修改?", CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(CancelConfirm_Closed);
                customMesage.Show();
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private QuestionTraceEntity AddQuestionTraceEntity { get; set; }

        private void OnAddQuestionCommand()
        {
            AddQuestionTraceEntity = new QuestionTraceEntity();
            AddQuestionTraceEntity.ManufactureNumber = ManufactureNumber;
            AddQuestionTraceEntity.UserIDMain = -2;
            AddQuestionTraceEntity.UserIDCP1 = -2;
            AddQuestionTraceEntity.UserIDCP2 = -2;
            AddQuestionTraceEntity.QuestionTrace = new questiontrace();
            QuestionTraceWindow questionTraceWindow = new QuestionTraceWindow(QuestionTraceOperation.ADD, UserEntityList, DepartmentEntityList, AddQuestionTraceEntity);
            questionTraceWindow.Closed += AddQuestionClosed;
            questionTraceWindow.Show();
        }

        private void AddQuestionClosed(object sender, EventArgs e)
        {
            QuestionTraceWindow questionTraceWindow = sender as QuestionTraceWindow;
            if (questionTraceWindow.DialogResult == true)
            {
                ProductDomainContext.questiontraces.Add(AddQuestionTraceEntity.QuestionTrace);
                BusyIndicator.IsBusy = true;
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
            BusyIndicator.IsBusy = false;
        }


        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            OnAddQuestionCommand();
        }

        private void CancelConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                this.DialogResult = false;
            }
        }

        private void EditingConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                for (int pos = 0; pos < planListViewModelList.Count; ++pos)
                {
                    PlanListViewModel planListViewModel = planListViewModelList[pos];
                    if (planListViewModel.IsEditing)
                    {
                        planListViewModelList[pos].OnOKCommand();
                    }
                }
                this.DialogResult = true;
            }
        }

        private void AddTabItem(string header, PlanListViewModel aPlanListViewModel)
        {
            if (string.IsNullOrEmpty(header) || null == aPlanListViewModel)
            {
                return;
            }

            var list = this.planListTabControl.Items.Where(w => ((TabItem)w).Header.ToString() == header);
            if (list.Count() > 0)
            {
                ((TabItem)list.First()).Visibility = Visibility.Visible;

                this.planListTabControl.SelectedItem = list.First();
            }
            else
            {
                TabItem tabItem = new TabItem();

                tabItem.Header = header;

                PlanListEditDataGrid planListDataGrid = new PlanListEditDataGrid(aPlanListViewModel);
                var tabContent = planListDataGrid as UserControl;

                tabItem.Content = tabContent;
                this.planListTabControl.Items.Add(tabItem);
                if (null == this.planListTabControl.SelectedItem)
                {
                    this.planListTabControl.SelectedItem = tabItem;
                }
            }
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            DepartmentEntityList.Clear();
            UserEntityList.Clear();
            LoadOperation<department> loadOperationDepartment =
                SystemManageDomainContext.Load<department>(SystemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed_Dictionary;
        }

        void loadOperationDepartment_Completed_Dictionary(object sender, EventArgs e)
        {
            //DepartmentEntityDictionary.Clear();
            DepartmentEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                //DepartmentEntityDictionary.Add(departmentEntity.DepartmentID, departmentEntity);
                DepartmentEntityList.Add(departmentEntity);
            }

            DepartmentEntity departmentEntityZero = new DepartmentEntity();
            departmentEntityZero.DepartmentID = 0;
            departmentEntityZero.DepartmentName = "请选择部门";
            DepartmentEntityList.Add(departmentEntityZero);

            LoadOperation<user> loadOperationUser =
                SystemManageDomainContext.Load<user>(SystemManageDomainContext.GetUserQuery());
            loadOperationUser.Completed += loadOperationUser_Completed;
        }

        void loadOperationUser_Completed(object sender, EventArgs e)
        {
            //UserEntityDictionary.Clear();
            UserEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (user user in loadOperation.Entities)
            {
                if (user.user_name == "admin")
                {
                    continue;
                }
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                userEntity.Update();
                //UserEntityDictionary.Add(userEntity.UserID, userEntity);
                UserEntityList.Add(userEntity);
            }

            UserEntity userEntityZero = new UserEntity();
            userEntityZero.UserID = -2;
            userEntityZero.CUserName = "请选择用户";
            UserEntityList.Add(userEntityZero);

            BusyIndicator.IsBusy = false;
        }
    }
}

