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
using ProductManager.ViewModel.SystemManager;

namespace ProductManager.Views.SystemManager
{
    public partial class RoleRightManager : Page
    {
        public RoleRightManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.RoleRightManagerViewModel;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RoleRightManagerViewModel roleRightManagerViewModel = this.DataContext as RoleRightManagerViewModel;
            roleRightManagerViewModel.LoadData();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            RoleRightManagerViewModel roleRightManagerViewModel = this.DataContext as RoleRightManagerViewModel;
            roleRightManagerViewModel.ConfirmLeave();
        }
    }
}
