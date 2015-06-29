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
    public partial class LinkResponsePersonWindow : ChildWindow
    {
        public LinkResponsePersonWindow(ProductDomainContext aProductDomainContext
                                        , Dictionary<int, DepartmentEntity> aDepartmentEntityDictionary
                                        , Dictionary<String, UserEntity> aUserEntityDictionary)
        {
            InitializeComponent();
            this.DataContext = new LinkResponsePersonModel(this, aProductDomainContext, aDepartmentEntityDictionary, aUserEntityDictionary);
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            LinkResponsePersonModel lLinkResponsePersonModel = this.DataContext as LinkResponsePersonModel;
            lLinkResponsePersonModel.LoadData();
        }

    }
}

