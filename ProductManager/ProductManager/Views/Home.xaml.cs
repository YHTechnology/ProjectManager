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

using ProductManager.ViewModel;

namespace ProductManager
{
    public partial class Home : Page
    {
        private HomePageViewModel homePageViewModel;
        public Home()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.homePageViewModel = new HomePageViewModel(this);
            this.DataContext = homePageViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            homePageViewModel.LoadData();
        }


    }
}
