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
using System.ComponentModel.Composition;
using ProductManager.ViewModel;

namespace ProductManager
{
    public partial class SystemManager : Page
    {
        public SystemManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.SystemManagerViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Add MainPage
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = "系统管理首页";
                lHyperLinkButton.NavigateUri = new System.Uri("SystemManagerMain", UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "SystemManagerContentFrame";
                SystemManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;

            }


            SystemManagerViewModel systemManagerViewModel = this.DataContext as SystemManagerViewModel;
            foreach (ISubMenu lSubMenu in systemManagerViewModel.SubMenuList)
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = lSubMenu.Content;
                lHyperLinkButton.NavigateUri = new System.Uri(lSubMenu.NavigateUri, UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "SystemManagerContentFrame";
                SystemManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;
            }
        }

        void HyperLinkButton_Click(object sender, RoutedEventArgs e)
        {
            SystemManagerViewModel systemManagerViewModel = this.DataContext as SystemManagerViewModel;
            systemManagerViewModel.IsExpand = false;
        }

        private void SystemManagerContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in SystemManagerMenu.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (SystemManagerContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(SystemManagerContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                        SystemManagerViewModel systemManagerViewModel = this.DataContext as SystemManagerViewModel;
                        systemManagerViewModel.CurrentPageName = hb.Content.ToString();
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void SystemManagerContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

    }
}
