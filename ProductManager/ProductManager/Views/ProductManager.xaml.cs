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
using ProductManager.ViewModel;

namespace ProductManager.Views
{
    public partial class ProductManager : Page
    {
        public ProductManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.ProductManagerViewModel;
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Add MainPage
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = "重要部件到货信息";
                lHyperLinkButton.NavigateUri = new System.Uri("ProductManagerMain", UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "ProductManagerContentFrame";
                ProductManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;

            }

            ProductManagerViewModel productManagerViewModel = this.DataContext as ProductManagerViewModel;
            foreach (ISubMenu lSubMenu in productManagerViewModel.SubMenuList)
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = lSubMenu.Content;
                lHyperLinkButton.NavigateUri = new System.Uri(lSubMenu.NavigateUri, UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "ProductManagerContentFrame";
                ProductManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;
            }
        }

        void HyperLinkButton_Click(object sender, RoutedEventArgs e)
        {
            ProductManagerViewModel productManagerViewModel = this.DataContext as ProductManagerViewModel;
            productManagerViewModel.IsExpand = false;
        }

        private void ProductManagerContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in ProductManagerMenu.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (ProductManagerContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(ProductManagerContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                        ProductManagerViewModel productManagerViewModel = this.DataContext as ProductManagerViewModel;
                        productManagerViewModel.CurrentPageName = hb.Content.ToString();
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ProductManagerContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }
    }
}
