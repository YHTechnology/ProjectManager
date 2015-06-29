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
using System.Windows.Data;

namespace ProductManager.Views.SystemManager
{
    public partial class RightManager : Page
    {
        public RightManager()
        {
            InitializeComponent();
            //App app = Application.Current as App;
            this.DataContext = new RightManagerViewModel();
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RightManagerViewModel userManagerViewModel = this.DataContext as RightManagerViewModel;
            userManagerViewModel.SetCallBackLoaded(FinishLoaded);
            userManagerViewModel.LoadData();
//             foreach (CollectionViewGroup group in userManagerViewModel.UserDataView.Groups)
//             {
//                 UserGrid.CollapseRowGroup(group, true);
//             }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            RightManagerViewModel userManagerViewModel = this.DataContext as RightManagerViewModel;
            userManagerViewModel.ConfirmLeave();
        }

        private void FinishLoaded()
        {
            RightManagerViewModel userManagerViewModel = this.DataContext as RightManagerViewModel;
            foreach (CollectionViewGroup group in userManagerViewModel.UserDataView.Groups)
            {
                UserGrid.CollapseRowGroup(group, true);
            }
        }
    }
}
