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

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListStatisticsDataGrid : UserControl
    {
        private PlanListViewModel planListViewModel;
        public PlanListStatisticsDataGrid(PlanListViewModel aPlanListViewModel, bool aRemove)
        {
            InitializeComponent();
            if (aRemove)
            {
                string projectNameColumn = "项目名称";
                string manufactureNumberColumn = "生产令号";
                string versionIDColumn = "版本号";
                for (int pos = plansDataGrid.Columns.Count - 1; pos >= 0; --pos )
                {
                    string currentColumn = plansDataGrid.Columns[pos].Header as string;
                    if (projectNameColumn == currentColumn
                        || manufactureNumberColumn == currentColumn
                        || versionIDColumn == currentColumn)
                    {
                        plansDataGrid.Columns.Remove(plansDataGrid.Columns[pos]);
                    }
                }
            }
            this.planListViewModel = aPlanListViewModel;
            this.DataContext = this.planListViewModel;
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 3);
            filterExpander.SetValue(Canvas.ZIndexProperty, 1);
        }

        private void filterExpander_Expanded(object sender, RoutedEventArgs e)
        {
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 1);
            filterExpander.SetValue(Canvas.ZIndexProperty, 3);
            departmentNameComboBox.SelectedValue = 0;
        }

        private void filterExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            plansDataGrid.SetValue(Canvas.ZIndexProperty, 3);
            filterExpander.SetValue(Canvas.ZIndexProperty, 1);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            filterExpander.IsExpanded = false;
            int sequenceId = Convert.ToInt32(sequenceIdNumericUpDown.Value);
            Nullable<int> departmentId = departmentNameComboBox.SelectedValue as Nullable<int>;
            decimal weight = Convert.ToDecimal(weightNumericUpDown.Value);
            string projectName = programNameTextBox.Text;
            string manufactureNumber = manufactureNumberTextBox.Text;
            //planListViewModel.OnFilterCommand(sequenceId, weight, departmentId, projectName, manufactureNumber);
        }
    }
}
