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
using ProductManager.ViewData.Entity;
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.Views.ProductManagers
{
    public partial class DeleteImportantPartRejisterWindow : ChildWindow
    {
        public DeleteImportantPartRejisterWindow(ImportantPartRejesterEntity aImportantPartRejesterEntity)
        {
            InitializeComponent();
            this.DataContext = new DeleteImportantPartRejisterWindowViewModel(this, aImportantPartRejesterEntity);
        }
    }
}

