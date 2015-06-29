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
    public partial class QuestionTraceList : Page
    {
        public QuestionTraceList()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.QuestionTraceListViewModel;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            QuestionTraceListViewModel questionTraceListViewModel = this.DataContext as QuestionTraceListViewModel;
            questionTraceListViewModel.LoadData();
        }

    }
}
