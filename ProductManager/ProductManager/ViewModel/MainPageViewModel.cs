using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ProductManager.Views;


namespace ProductManager.ViewModel
{
    [Export("MainPage")]
    public class MainPageViewModel : NotifyPropertyChanged
    {
        public String UserName
        {
            get
            {
                App app = Application.Current as App;
                return app.UserInfo.UserName;
            }
        }

        public String DepartmentName
        {
            get
            {
                App app = Application.Current as App;
                return app.UserInfo.UserDepartment;
            }
        }

        private List<IMainMenu> _mainMenuList = new List<IMainMenu>();
        
        [ImportMany(typeof(IMainMenu))]
        public List<IMainMenu> mainMenuList { set; get; }

        public List<IMainMenu> MainMenuList
        {
            get
            {
                _mainMenuList.Clear();
                foreach (IMainMenu mainMenu in mainMenuList)
                {
                    App app = Application.Current as App;
                    bool isPermit;
                    if (app.UserInfo.UserAction.TryGetValue(mainMenu.ActionID, out isPermit))
                    {
                        if (isPermit)
                        {
                            _mainMenuList.Add(mainMenu);
                        }
                    }
                }
                _mainMenuList.Add(new HomeMainMenu());
                _mainMenuList.Sort((a, b) => a.OrderNumber.CompareTo(b.OrderNumber));
                return _mainMenuList;
            }
        }

        public ICommand OnLogout { get; private set; }

        public ICommand OnModifyPassword { get; private set; }

        public Action LogoutCallBack { get; set; }

        public MainPageViewModel()
        {
            OnLogout = new DelegateCommand(onLogout);
            OnModifyPassword = new DelegateCommand(onModifyPassword);
        }

        private void onLogout(/*object aObject*/)
        {
            WebContext.Current.Authentication.Logout(Logout_Complete, null);
            App app = Application.Current as App;
            app.LogoutUser(app.UserInfo.UserID, app.UserInfo.UserName);
        }

        private void Logout_Complete(LogoutOperation aLogoutOperation)
        {
            if (!aLogoutOperation.User.Identity.IsAuthenticated)
            {
                //this.Content = new Main();
                LogoutCallBack();
            }
        }

        private void onModifyPassword(/*object aObject*/)
        {
            ModifyPasswrodWindow modifyPasswordWindow = new ModifyPasswrodWindow();
            modifyPasswordWindow.Show();
        }

        public void CheckUserPassword()
        {
            App app = Application.Current as App;
            if (app.UserInfo.UserPassword == Cryptography.MD5.GetMd5String("123456"))
            {
                NotifyWindow notifyWindow = new NotifyWindow("修改密码", "用户密码与初始密码一致，请修改密码！");
                notifyWindow.Show();
            }
        }

    }
}
