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
using ProductManager.Web.Service;

namespace ProductManager.Views.ProductManagers
{
    public partial class ImportProjectWindow : ChildWindow
    {
        public ImportProjectWindow(ProductDomainContext aProductDomainContext, Dictionary<String, ProjectEntity> aProjectEntityDictionary)
        {
            InitializeComponent();
            this.DataContext = new ImportProjectWindowViewModel(this, aProductDomainContext, aProjectEntityDictionary);
        }
    }
}

