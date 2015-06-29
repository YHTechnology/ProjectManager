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

using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.Views.ProductManagers
{
    public partial class LinkFileWindow : ChildWindow
    {
        public LinkFileViewModel linkFileViewModel;
        public LinkFileWindow(LinkFileViewModel aLinkFileViewModel)
        {
            InitializeComponent();
            linkFileViewModel = aLinkFileViewModel;
            this.DataContext = linkFileViewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            linkFileViewModel.IsLinkProject = (rb == linkProjectChexkBox);
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            linkFileViewModel.LoadData();
        }

    }
}

