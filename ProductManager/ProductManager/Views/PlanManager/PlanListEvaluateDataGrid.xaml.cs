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
    public partial class PlanListEvaluateDataGrid : UserControl
    {
        private PlanListViewModel planListViewModel;
        public PlanListEvaluateDataGrid(PlanListViewModel aPlanListViewModel, bool aRemove)
        {
            InitializeComponent();
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
            }
            this.planListViewModel = aPlanListViewModel;
            this.DataContext = this.planListViewModel;
        }
    }
}
