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
using ProductManager.ViewModel.PlanManager;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanExport : Page
    {
        public PlanExport()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.PlanExportViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlanExportViewModel planExportViewModel = this.DataContext as PlanExportViewModel;
            planExportViewModel.LoadData();
        }

    }
}
