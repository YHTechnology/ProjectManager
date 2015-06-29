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

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListEditDataGrid : UserControl
    {
        public PlanListEditDataGrid(PlanListViewModel aPlanListViewModel)
        {
            InitializeComponent();
            this.DataContext = aPlanListViewModel;

            if (1 != aPlanListViewModel.ColumnModelIndex)
            {
                string removeColumn = "计划下订单时间";
                foreach (DataGridColumn column in plansDataGrid.Columns)
                {
                    if (removeColumn == column.Header as string)
                    {
                        plansDataGrid.Columns.Remove(column);
                        break;
                    }
                }
            }

            if(0 == aPlanListViewModel.ColumnModelIndex
                || 2 == aPlanListViewModel.ColumnModelIndex)
            {
                orderDateLable.Visibility = Visibility.Collapsed;
                OrderDatePicker.Visibility = Visibility.Collapsed;
            }

            if (1 == aPlanListViewModel.ColumnModelIndex
                || 2 == aPlanListViewModel.ColumnModelIndex)
            {
                targetDateLable.Content = "计划到货时间";
                accomplishDateLable.Content = "实际到货时间";
            }

            if (aPlanListViewModel.IsReadOnly)
            {
                this.plansDataGrid.Margin = new Thickness(0, 0, 0, 0);
                this.plansDataGrid.SetValue(Canvas.ZIndexProperty, 3);
                this.selectedPlanGrid.SetValue(Canvas.ZIndexProperty, 1);
            }
        }
    }
}
