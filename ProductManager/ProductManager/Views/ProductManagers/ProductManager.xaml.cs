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
using ProductManager.ViewData.Entity;

namespace ProductManager.Views.ProductManagers
{
    public partial class ProductManager : Page
    {
        public ProductManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.ProductManagersViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ProductManagersViewModel productManagersViewModel = this.DataContext as ProductManagersViewModel;
            productManagersViewModel.LoadData();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ProductManagersViewModel productManagersViewModel = this.DataContext as ProductManagersViewModel;
            productManagersViewModel.ConfirmLeave();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ProductManagersViewModel productManagersViewModel = this.DataContext as ProductManagersViewModel;
            productManagersViewModel.HideExpander();
        }

        private void ProductFiles_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ProjectFilesEntity lprojectFilesEntity = e.Row.DataContext as ProjectFilesEntity;
            if (lprojectFilesEntity.FileDelete)
            {
                e.Row.Background = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
