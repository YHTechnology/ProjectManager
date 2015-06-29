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
    public partial class ProductTypeManager : Page
    {
        public ProductTypeManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.ProductTypeManagerViewModel;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ProductTypeManagerViewModel productTypeManagerViewModel = this.DataContext as ProductTypeManagerViewModel;
            productTypeManagerViewModel.LoadData();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ProductTypeManagerViewModel productTypeManagerViewModel = this.DataContext as ProductTypeManagerViewModel;
            productTypeManagerViewModel.ConfirmLeave();
        }

    }
}
