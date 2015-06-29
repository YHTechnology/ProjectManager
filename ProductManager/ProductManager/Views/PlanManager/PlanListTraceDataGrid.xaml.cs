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
using System.Windows.Data;

using ProductManager.ViewModel.PlanManager;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListTraceDataGrid : UserControl
    {
        private PlanListViewModel planListViewModel;
        public PlanListTraceDataGrid(PlanListViewModel aPlanListViewModel, bool aRemove)
        {
            InitializeComponent();

            this.DataContext = aPlanListViewModel;
            this.planListViewModel = aPlanListViewModel;
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 3);
            filterExpander.SetValue(Canvas.ZIndexProperty, 1);

            this.showFinishedCheckBox.IsChecked = true;
            this.showUnfinishedCheckBox.IsChecked = true;

            if (aRemove)
            {
                string projectNameColumn = "项目名称";
                string manufactureNumberColumn = "生产令号";
                string versionIDColumn = "版本号";
                for (int pos = plansDataGrid.Columns.Count - 1; pos >= 0; --pos)
                {
                    string currentColumn = plansDataGrid.Columns[pos].Header as string;
                    if (projectNameColumn == currentColumn
                        || manufactureNumberColumn == currentColumn
                        || versionIDColumn == currentColumn)
                    {
                        plansDataGrid.Columns.Remove(plansDataGrid.Columns[pos]);
                    }
                }
                this.plansDataGrid.ItemsSource = this.planListViewModel.FilterPlanList;

                this.programNameLabel.Visibility = Visibility.Collapsed;
                this.programNameTextBox.Visibility = Visibility.Collapsed;
                this.manufactureNameLabel.Visibility = Visibility.Collapsed;
                this.manufactureNumberTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                PagedCollectionView view = new PagedCollectionView(this.planListViewModel.FilterPlanList);
                view.GroupDescriptions.Add(new PropertyGroupDescription("ProjectName"));
                this.plansDataGrid.ItemsSource = view;
                try
                {
                    foreach (CollectionViewGroup group in view.Groups)
                    {
                        plansDataGrid.CollapseRowGroup(group, true);
                    }
                }
                catch (Exception ex)
                {
                    // Could not collapse group.
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void filterExpander_Expanded(object sender, RoutedEventArgs e)
        {
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 1);
            filterExpander.SetValue(Canvas.ZIndexProperty, 3);
        }

        private void filterExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 3);
            filterExpander.SetValue(Canvas.ZIndexProperty, 1);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            filterExpander.IsExpanded = false;
            string projectName = programNameTextBox.Text;
            string manufactureNumber = manufactureNumberTextBox.Text;
            bool showFinished = showFinishedCheckBox.IsChecked.HasValue ? showFinishedCheckBox.IsChecked.Value : false;
            bool showUnfinished = showUnfinishedCheckBox.IsChecked.HasValue ? showUnfinishedCheckBox.IsChecked.Value : false;
            bool showOvertime = showOvertimeCheckBox.IsChecked.HasValue ? showOvertimeCheckBox.IsChecked.Value : false;
            Nullable<DateTime> startTargetDate = startTargetDatePicker.SelectedDate;
            Nullable<DateTime> endTargetDate = endTargetDatePicker.SelectedDate;
            Nullable<DateTime> startAccomplishDate = startAccomplishDatePicker.SelectedDate;
            Nullable<DateTime> endAccomplishDate = endAccomplishDatePicker.SelectedDate;

            planListViewModel.OnFilterCommand(projectName, manufactureNumber, showFinished, showUnfinished,
                                    startTargetDate, endTargetDate, startAccomplishDate, endAccomplishDate, showOvertime);
        }

        private void plansDataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
//             if (DataGridEditAction.Commit == e.EditAction)
//             {
//                 PlanEntity planEntity = e.Row.DataContext as PlanEntity;
//                 DateTime targetDateTime = planEntity.TargetDateAdjustment2.HasValue ? planEntity.TargetDateAdjustment2.Value :
//                                         (planEntity.TargetDateAdjustment1.HasValue ? planEntity.TargetDateAdjustment1.Value : planEntity.TargetDate);
//                 if (null == planEntity.AccomplishDate)
//                 {
//                     DateTime currentDateTime = DateTime.Now;
//                     TimeSpan difference = targetDateTime - currentDateTime;
//                     if (difference.Days > remindDay)
//                     {
//                         e.Row.Background = new SolidColorBrush(Colors.Gray);
//                     }
//                     else if (difference.Days >= 0 && difference.Days <= remindDay)
//                     {
//                         e.Row.Background = new SolidColorBrush(Colors.Magenta);
//                     }
//                     else
//                     {
//                         e.Row.Background = new SolidColorBrush(Colors.Red);
//                     }
//                 }
//                 else
//                 {
//                     if (targetDateTime >= planEntity.AccomplishDate)
//                     {
//                         e.Row.Background = new SolidColorBrush(Colors.Green);
//                     }
//                     else
//                     {
//                         e.Row.Background = new SolidColorBrush(Colors.Purple);
//                     }
//                 }
//             }
        }

        private void plansDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
//             PlanEntity planEntity = e.Row.DataContext as PlanEntity;
//             DateTime targetDateTime = planEntity.TargetDateAdjustment2.HasValue ? planEntity.TargetDateAdjustment2.Value :
//                                         (planEntity.TargetDateAdjustment1.HasValue ? planEntity.TargetDateAdjustment1.Value : planEntity.TargetDate);           
//             if (null == planEntity.AccomplishDate)
//             {
//                 DateTime currentDateTime = DateTime.Now;
//                 TimeSpan difference = targetDateTime - currentDateTime;
//                 if (difference.Days > remindDay)
//                 {
//                     e.Row.Background = new SolidColorBrush(Colors.Gray);
//                 }
//                 else if (difference.Days >= 0 && difference.Days <= remindDay)
//                 {
//                     e.Row.Background = new SolidColorBrush(Colors.Magenta);
//                 }
//                 else
//                 {
//                     e.Row.Background = new SolidColorBrush(Colors.Red);
//                 }
//             }
//             else
//             {
//                 if (targetDateTime >= planEntity.AccomplishDate)
//                 {
//                     e.Row.Background = new SolidColorBrush(Colors.Green);
//                 }
//                 else
//                 {
//                     e.Row.Background = new SolidColorBrush(Colors.Purple);
//                 }
//             }
        }
    }
}
