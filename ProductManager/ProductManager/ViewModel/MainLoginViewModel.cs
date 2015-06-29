using System;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.Controls;
using ProductManager.Module;
using ProductManager.Views;

namespace ProductManager.ViewModel
{
    [Export("MainLogin")]
    public class MainLoginViewModel : NotifyPropertyChanged
    {
        public LoginInfo LoginInfo { get; set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public Action<object> FinishLogon { get; set; }

        public ICommand OnLogin { get; private set; }

        public void onLogin()
        {
            if (LoginInfo.Validate())
            {
                IsBusy = true;
                WebContext.Current.Authentication.Login(new LoginParameters(LoginInfo.UserName, Cryptography.MD5CryptoServiceProvider.GetMd5String(LoginInfo.Password), LoginInfo.IsRemember, ""), LoginOperation_Completed, null);
            }
        }

        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            IsBusy = false;
            if (loginOperation.LoginSuccess)
            {
                ProductManager.Web.User lUser = loginOperation.User.Identity as ProductManager.Web.User;
                if (lUser.IsFreeze)
                {
                    NotifyWindow notifyWindow = new NotifyWindow("用户已冻结", "用户已冻结！");
                    notifyWindow.Show();
                    return;
                }
               
                App app = Application.Current as App;
                app.UserInfo = new UserInfo();
                
                app.UserInfo.UserName = lUser.UserName;
                app.UserInfo.UserAction = lUser.RightDictionary;
                app.UserInfo.UserDepartment = lUser.Department;
                app.UserInfo.UserID = lUser.UserID;
                app.UserInfo.DepartmentID = lUser.DepartmentID;
                app.UserInfo.UserPassword = lUser.Password;
                app.UserInfo.IsManager = lUser.IsManager;

                if (app.UserInfo.UserPassword == Cryptography.MD5.GetMd5String("123456"))
                {
                    ModifyPasswrodWindow modifyPasswordWindow = new ModifyPasswrodWindow();
                    modifyPasswordWindow.Closed += modifyWindowClosed;
                    modifyPasswordWindow.Show();
                }
                else
                {
                    app.LogonUser(app.UserInfo.UserID, app.UserInfo.UserName);
                    FinishLogon(null);
                }
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("用户名或密码错误", "用户名或密码错误");
                notifyWindow.Show();
            }
        }

        private void modifyWindowClosed(object sender, EventArgs e)
        {
            ModifyPasswrodWindow modifyPasswordWindow = sender as ModifyPasswrodWindow;
            if (modifyPasswordWindow.DialogResult == true)
            {
                App app = Application.Current as App;
                if (app.UserInfo.UserPassword != Cryptography.MD5.GetMd5String("123456"))
                {
                    app.LogonUser(app.UserInfo.UserID, app.UserInfo.UserName);
                    FinishLogon(null);
                    return;
                }
            }
        }

        public MainLoginViewModel()
        {
            LoginInfo = new LoginInfo();
            OnLogin = new DelegateCommand(onLogin);
        }
    }
}
