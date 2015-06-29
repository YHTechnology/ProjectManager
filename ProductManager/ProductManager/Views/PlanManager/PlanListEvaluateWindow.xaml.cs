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

using ProductManager.ViewData.Entity;
using ProductManager.ViewModel.PlanManager;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListEvaluateWindow : ChildWindow
    {
        public ObservableCollection<PlanListViewModel> planListViewModelList;
        public Dictionary<string, int> planEvaluateResultDictionary;
        public PlanExtraEntity planExtraEntity;
        public PlanListEvaluateWindow(string aTitle, ObservableCollection<PlanListViewModel> aPlanListViewModelList, 
            Dictionary<string, int> aPlanEvaluateResultDictionary, PlanExtraEntity aPlanExtraEntity)
        {
            InitializeComponent();
            this.Title = aTitle;
            this.planListViewModelList = aPlanListViewModelList;
            this.planEvaluateResultDictionary = aPlanEvaluateResultDictionary;
            this.planExtraEntity = aPlanExtraEntity;

            foreach (PlanListViewModel planListViewModel in this.planListViewModelList)
            {
                PlanListEvaluateDataGrid planListDataGrid = new PlanListEvaluateDataGrid(planListViewModel, null == this.planExtraEntity);
                AddTabItem(planListViewModel.Title, planListDataGrid as UserControl);
            }

            PlanListEvaluateResultChart planListEvaluateResultChart = new PlanListEvaluateResultChart(aPlanEvaluateResultDictionary);
            AddTabItem("完成率", planListEvaluateResultChart as UserControl);
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            FilterPlanByDate();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPlanByDate();
        }

        private void FilterPlanByDate()
        {
            bool timeFilter = timeFilterCheckBox.IsChecked.HasValue ? timeFilterCheckBox.IsChecked.Value : false;
            Nullable<DateTime> startDate = startDatePicker.SelectedDate;
            Nullable<DateTime> endDate = endDatePicker.SelectedDate;

            planEvaluateResultDictionary.Clear();
            foreach (PlanListViewModel planListViewModel in planListViewModelList)
            {
                planListViewModel.OnFilterFinishedCommand(timeFilter, startDate, endDate);

                decimal totalValue = 0;
                decimal accomplishValue = 0;
                foreach (PlanEntity item in planListViewModel.FilterPlanList)
                {
                    totalValue += item.Weight;
                    if (item.AccomplishDate.HasValue && item.Score.HasValue)
                    {
                        accomplishValue += item.Score.Value;
                    }
                }
                decimal resultValue = (0 == totalValue) ? 0 : accomplishValue / totalValue;
                int resultInt = Convert.ToInt16(Convert.ToDouble(resultValue) * 100);
                planEvaluateResultDictionary.Add(planListViewModel.Title, resultInt);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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
                this.planListTabControl.SelectedItem = tabItem;
            }
        }
    }
}

