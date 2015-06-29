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
using System.ServiceModel.DomainServices.Client.ApplicationServices;

namespace ProductManager
{
    public partial class MainLogin : UserControl
    {
        public MainLogin()
        {
            InitializeComponent();
            App app = Application.Current as App;
            this.DataContext = app.MainLoginViewModel;
            app.MainLoginViewModel.FinishLogon = FinishLogin;
        }

        private void Logon_Click(object sender, RoutedEventArgs e)
        {
            LoginIndicator.IsBusy = true;
            WebContext.Current.Authentication.Login(new LoginParameters("", "", true, ""), LoginOperation_Completed, null);
        }

        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            LoginIndicator.IsBusy = false;
            if (loginOperation.LoginSuccess)
            {
                this.Content = new MainPage();
            }
        }

        private void FinishLogin(object aObject)
        {
            //App app = Application.Current as App;
            //app.RootVisual = app.MainPage;
            this.Content = new MainPage();
        }
    }
}
