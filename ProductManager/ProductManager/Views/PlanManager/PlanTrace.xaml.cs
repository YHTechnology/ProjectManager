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
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using ProductManager.ViewModel.PlanManager;
using ProductManager.ViewData.Entity;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanTrace : Page
    {
        private PlanTraceViewModel planListViewModel;
        public PlanTrace()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.PlanTraceViewModel;
            this.planListViewModel = app.PlanTraceViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlanTraceViewModel planTraceViewModel = this.DataContext as PlanTraceViewModel;
            planTraceViewModel.LoadData();
        }

        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (null != dataGrid && dataGrid == projectDataGrid)
            {
                if (dataGrid.SelectedItems.Count > 1)
                {
                    ObservableCollection<ProjectEntity> projectList = new ObservableCollection<ProjectEntity>();
                    foreach (var item in dataGrid.SelectedItems)
                    {
                        projectList.Add(item as ProjectEntity);
                    }
                    traceSelectButton.Content = "跟踪多项";
                    planListViewModel.SelectedProjectList = projectList;
                }
                else
                {
                    planListViewModel.SelectedProjectList = null;
                    traceSelectButton.Content = "跟踪单项";
                }
            }
        }
    }
}
