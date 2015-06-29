using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProductManager.ViewModel;

namespace ProductManager
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.MainPageViewModel;
            app.MainPageViewModel.LogoutCallBack = OnLogoutCallBack;
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (ContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(ContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageViewModel mainPageViewModel = this.DataContext as MainPageViewModel;
            foreach (IMainMenu lMainMenu in mainPageViewModel.MainMenuList)
            {
                HyperlinkButton lHyperLinkButton = new HyperlinkButton();
                lHyperLinkButton.Content = lMainMenu.Content;
                lHyperLinkButton.NavigateUri = new System.Uri(lMainMenu.NavigateUri,UriKind.Relative);
                lHyperLinkButton.Style = Application.Current.Resources["LinkStyle"] as Style;
                lHyperLinkButton.TargetName = "ContentFrame";
                LinksStackPanel.Children.Add(lHyperLinkButton);
            }
            mainPageViewModel.CheckUserPassword();
        }

        private void OnLogoutCallBack()
        {
            //NavigationService.Navigate(new Uri("MainLogin"));
            //App app = Application.Current as App;
            this.Content = new MainLogin();
        }
    }
}