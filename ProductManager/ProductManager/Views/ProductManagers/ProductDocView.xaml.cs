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
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.Views.ProductManagers
{
    public partial class ProductDocView : Page
    {
        public ProductDocView()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.ProductDocViewViewModel;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ProductDocViewViewModel productDocViewViewModel = this.DataContext as ProductDocViewViewModel;
            productDocViewViewModel.LoadData();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ProductDocViewViewModel productDocViewViewModel = this.DataContext as ProductDocViewViewModel;
            productDocViewViewModel.HideExpander();
        }
    }
}
