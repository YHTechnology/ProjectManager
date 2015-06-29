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
    public partial class ImportantPartRejesterWindow : ChildWindow
    {
        public ImportantPartRejesterWindow(ImportantPartRejesterWindowState aWindowState, ImportantPartRejesterEntity aImportantPartRejesterEntity)
        {
            InitializeComponent();
            this.DataContext = new ImportantPartRejesterWindowViewModel(this, aWindowState, aImportantPartRejesterEntity);
        }

    }
}

