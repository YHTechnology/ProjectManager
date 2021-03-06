﻿using System;
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
    public partial class ImportProductWindow : ChildWindow
    {
        public ImportProductWindow(ProductDomainContext aProductDomainContext
                                , Dictionary<String, ProjectEntity> aProjectEntityDictionary
                                , Dictionary<String, ProductEntity> aProductEntityDictionary
                                , Dictionary<int, ProductTypeEntity> aProductTypeEntityDictionary)
        {
            InitializeComponent();
            this.DataContext = new ImportProductWindowViewModel(this, aProductDomainContext, aProjectEntityDictionary, aProductEntityDictionary, aProductTypeEntityDictionary);
        }

    }
}

