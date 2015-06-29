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
using ProductManager.ViewModel.PlanManager;
using System.Collections.ObjectModel;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Views.ProductManagers;
using ProductManager.Web.Model;
using ProductManager.ViewModel.ProductManagers;
using ProductManager.Web.Service;
using System.ServiceModel.DomainServices.Client;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListTraceWindow : ChildWindow
    {
        public ObservableCollection<PlanListViewModel> planListViewModelList;
        public PlanExtraEntity planExtraEntity;
        public PlanTraceViewModel PlanTraceViewModel;

        public ObservableCollection<DepartmentEntity> DepartmentEntityList = new ObservableCollection<DepartmentEntity>();
        public ObservableCollection<UserEntity> UserEntityList = new ObservableCollection<UserEntity>();

        private ProductDomainContext ProductDomainContext = new ProductDomainContext();
        private SystemManageDomainContext SystemManageDomainContext = new SystemManageDomainContext();

        public String ManufactureNumber { get; set; }

        public PlanListTraceWindow(string aTitle, ObservableCollection<PlanListViewModel> aPlanListViewModelList,
            PlanExtraEntity aPlanExtraEntity, PlanTraceViewModel aPlanTraceViewModel)
        {
            InitializeComponent();
            this.Title = aTitle;
            this.planListViewModelList = aPlanListViewModelList;
            this.planExtraEntity = aPlanExtraEntity;
            this.PlanTraceViewModel = aPlanTraceViewModel;

            
            if (null == aPlanExtraEntity)
            {
                this.planListTabControl.Margin = new Thickness(2, 2, 2, 2);
                this.planListTabControl.SetValue(Canvas.ZIndexProperty, 3);
                this.planExtraGrid.SetValue(Canvas.ZIndexProperty, 1);
                this.QuestionButton.IsEnabled = false;
            }
            else
            {
                this.planExtraGrid.DataContext = planExtraEntity;
                ManufactureNumber = aPlanExtraEntity.ManufactureNumber;
                this.QuestionButton.IsEnabled = true;
            }

            Dictionary<string, int> accomplishRateDictionary = new Dictionary<string, int>();
            foreach (PlanListViewModel item in planListViewModelList)
            {
                PlanListTraceDataGrid planListDataGrid = new PlanListTraceDataGrid(item, null != planExtraEntity);
                AddTabItem(item.Title, planListDataGrid as UserControl);

                decimal tatal = 0;
                decimal accomplish = 0;
                foreach (PlanEntity planEntity in item.PlanList)
                {
                    tatal += planEntity.Weight;
                    if (planEntity.AccomplishDate.HasValue && planEntity.Score.HasValue)
                    {
                        accomplish += planEntity.Score.Value;
                    }
                }
                decimal resultValue = (0 == tatal) ? 0 : accomplish / tatal;
                int resultInt = Convert.ToInt16(Convert.ToDouble(resultValue) * 100);
                accomplishRateDictionary.Add(item.Title, resultInt);
            }


            PlanListEvaluateResultChart planListEvaluateResultChart = new PlanListEvaluateResultChart(accomplishRateDictionary);
            AddTabItem("完成率", planListEvaluateResultChart as UserControl);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            string error = CheckAccomplish();
            if (string.IsNullOrEmpty(error))
            {
                Save();
                this.DialogResult = false;
            }
            else
            {
                error += "，继续关闭？";
                CustomMessage customMesage = new CustomMessage(error, CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(CancelConfirm_Closed);
                customMesage.Show();
            }           
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string error = CheckAccomplish();
            if (string.IsNullOrEmpty(error))
            {
                Save();
                this.DialogResult = true;
            }
            else
            {
                error += "，继续导出？";
                CustomMessage customMesage = new CustomMessage(error, CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(OKConfirm_Closed);
                customMesage.Show();
            }      
        }

        private void CancelConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                Save();
                this.DialogResult = false;
            }
        }

        private void OKConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                Save();
                this.DialogResult = true;
            }
        }

        private string CheckAccomplish()
        {
            string errorMessage = string.Empty;
            foreach (PlanListViewModel model in planListViewModelList)
            {
                foreach (PlanEntity item in model.PlanList)
                {
                    if (item.AccomplishDate.HasValue && !item.Score.HasValue)
                    {
                        errorMessage = model.Title + item.SequenceId + "未设置分数";
                        break;
                    }
                }
            }
            return errorMessage;
        }

        private void Save()
        {
            foreach (PlanListViewModel model in planListViewModelList)
            {
                foreach (PlanEntity item in model.PlanList)
                {
                    item.DUpdate();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void AddTabItem(string header, UserControl aUserControl)
        {
            if (string.IsNullOrEmpty(header) || null == aUserControl)
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

                var tabContent = aUserControl;

                tabItem.Content = tabContent;
                this.planListTabControl.Items.Add(tabItem);
                if (null == this.planListTabControl.SelectedItem)
                {
                    this.planListTabControl.SelectedItem = tabItem;
                }
            }
        }

        private QuestionTraceEntity AddQuestionTraceEntity { get; set; }

        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            OnAddQuestionCommand();
        }

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

