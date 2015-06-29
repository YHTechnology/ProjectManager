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
    public partial class PlanEdit : Page
    {
        public PlanEdit()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.PlanEditViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlanEditViewModel planEditViewModel = this.DataContext as PlanEditViewModel;
            planEditViewModel.LoadData();
        }

    }
}
