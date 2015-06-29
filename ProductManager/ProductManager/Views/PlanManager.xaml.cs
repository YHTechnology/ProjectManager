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
    public partial class PlanManager : Page
    {
        public PlanManager()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.PlanManagerViewModel;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Add MainPage
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = "计划提醒";
                lHyperLinkButton.NavigateUri = new System.Uri("PlanManagerMain", UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "PlanManagerContentFrame";
                PlanManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;

            }

            PlanManagerViewModel PlanManagerViewModel = this.DataContext as PlanManagerViewModel;
            foreach (ISubMenu lSubMenu in PlanManagerViewModel.SubMenuList)
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = lSubMenu.Content;
                lHyperLinkButton.NavigateUri = new System.Uri(lSubMenu.NavigateUri, UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyleMenu"] as Style;
                lHyperLinkButton.TargetName = "PlanManagerContentFrame";
                PlanManagerMenu.Children.Add(lHyperLinkButton);
                lHyperLinkButton.Click += HyperLinkButton_Click;
            }
        }

        void HyperLinkButton_Click(object sender, RoutedEventArgs e)
        {
            PlanManagerViewModel planManagerViewModel = this.DataContext as PlanManagerViewModel;
            planManagerViewModel.IsExpand = false;
        }

        private void PlanManagerContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in PlanManagerMenu.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (PlanManagerContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(PlanManagerContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                        PlanManagerViewModel planManagerViewModel = this.DataContext as PlanManagerViewModel;
                        planManagerViewModel.CurrentPageName = hb.Content.ToString();
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void PlanManagerContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

    }
}
