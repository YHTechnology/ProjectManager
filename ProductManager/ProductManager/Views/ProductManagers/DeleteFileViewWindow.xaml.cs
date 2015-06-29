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
    public partial class DeleteFileViewWindow : ChildWindow
    {
        public DeleteFileViewWindow(ProjectFilesEntity aProjectFilesEntity)
        {
            InitializeComponent();
            this.DataContext = new DeleteFileViewWindowViewModel(this, aProjectFilesEntity);
        }
    }
}

