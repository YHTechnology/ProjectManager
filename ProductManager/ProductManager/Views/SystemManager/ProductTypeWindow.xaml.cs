using System.Windows;
using System.Windows.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.ViewModel.SystemManager;

namespace ProductManager.Views.SystemManager
{
    public partial class ProductTypeWindow : ChildWindow
    {
        public ProductTypeWindow(ProductTypeEntity aProductTypeEntity)
        {
            InitializeComponent();
            this.DataContext = new ProductTypeWindowViewModel(this, aProductTypeEntity);
        }
    }
}

