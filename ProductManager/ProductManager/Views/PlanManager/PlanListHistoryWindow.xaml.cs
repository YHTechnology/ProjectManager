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

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListHistoryWindow : ChildWindow
    {
        public ObservableCollection<PlanListViewModel> planListViewModelList;
        public PlanExtraEntity planExtraEntity;
        public PlanTraceViewModel PlanTraceViewModel;
        public ProjectEntity projectEntity;
        public string LastPlanVersiontId;
        public string SelectVersionId;
        public PlanListHistoryWindow(string aTitle, ObservableCollection<PlanListViewModel> aPlanListViewModelList,
            PlanExtraEntity aPlanExtraEntity, PlanTraceViewModel aPlanTraceViewModel, ProjectEntity aProjectEntity,
            string aLastPlanVersiontId)
        {
            InitializeComponent();
            this.Title = aTitle;
            this.planListViewModelList = aPlanListViewModelList;
            this.planExtraEntity = aPlanExtraEntity;
            this.PlanTraceViewModel = aPlanTraceViewModel;
            this.projectEntity = aProjectEntity;
            this.versionComboBox.ItemsSource = projectEntity.PlanVersionDictionary;
            this.versionComboBox.IsEnabled = projectEntity.HasHistory;
            this.versionComboBox.SelectedValuePath = "Key";
            this.versionComboBox.DisplayMemberPath = "Key";
            this.versionComboBox.SelectedValue = aLastPlanVersiontId;
            this.LastPlanVersiontId = aLastPlanVersiontId;
            this.SelectVersionId = aLastPlanVersiontId;
       
            if (null == aPlanExtraEntity)
            {
                this.planListTabControl.Margin = new Thickness(2, 2, 2, 2);
                this.planListTabControl.SetValue(Canvas.ZIndexProperty, 3);
                this.planExtraGrid.SetValue(Canvas.ZIndexProperty, 1);
            }
            else
            {
                this.planExtraGrid.DataContext = planExtraEntity;
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
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SelectVersionId = versionComboBox.SelectedValue as string;
            if (SelectVersionId != LastPlanVersiontId)
            {
                this.DialogResult = true;
            }
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
    }
}

