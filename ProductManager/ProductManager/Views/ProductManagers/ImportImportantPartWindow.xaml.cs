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
    public partial class ImportImportantPartWindow : ChildWindow
    {
        public ImportImportantPartWindow(Dictionary<string, ProjectEntity> aProjectEntityDictionary, ProductDomainContext aProductDomainContext, Dictionary<int, UserEntity> aDictionaryUser, ProjectEntity aProjectEntity)
        {
            InitializeComponent();
            this.DataContext = new ImportImportantPartWindowViewModel(this, aProjectEntityDictionary, aProductDomainContext, aDictionaryUser, aProjectEntity);
        }

    }
}

