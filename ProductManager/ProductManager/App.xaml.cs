using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.Module;
using ProductManager.ViewModel;
using ProductManager.ViewModel.SystemManager;
using ProductManager.ViewModel.PlanManager;
using ProductManager.ViewModel.ProductManagers;
using ProductManager.Web.Service;

namespace ProductManager
{
    public partial class App : Application
    {
        [Import("MainPage")]
        public MainPageViewModel MainPageViewModel { get; set; }

        [Import("SystemManager")]
        public SystemManagerViewModel SystemManagerViewModel { get; set; }

        [Import("PlanManager")]
        public PlanManagerViewModel PlanManagerViewModel { get; set; }

        [Import("ProductManager")]
        public ProductManagerViewModel ProductManagerViewModel { get; set; }

        [Import("MainLogin")]
        public MainLoginViewModel MainLoginViewModel { get; set; }

        [Import("UserManager")]
        public UserManagerViewModel UserManagerViewModel { get; set; }

        [Import("PlanEdit")]
        public PlanEditViewModel PlanEditViewModel { get; set; }

        [Import("PlanTrace")]
        public PlanTraceViewModel PlanTraceViewModel { get; set; }

        [Import("PlanEvaluate")]
        public PlanEvaluateViewModel PlanEvaluateViewModel { get; set; }

        [Import("PlanStatistics")]
        public PlanStatisticsViewModel PlanStatisticsViewModel { get; set; }

        [Import("PlanExport")]
        public PlanExportViewModel PlanExportViewModel { get; set; }
        
        [Import("RoleRightManager")]
        public RoleRightManagerViewModel RoleRightManagerViewModel { get; set; }

        [Import("RightManager")]
        public RightManagerViewModel RightManagerViewModel { get; set; }

        [Import("ProductManagers")]
        public ProductManagersViewModel ProductManagersViewModel { get; set; }

        [Import("ProductDocManager")]
        public ProductDocManagerViewModel ProductDocManagerViewModel { get; set; }

        [Import("ProductDocView")]
        public ProductDocViewViewModel ProductDocViewViewModel { get; set; }

        [Import("QuestionTrace")]
        public QuestionTraceViewModel QuestionTraceViewModel { get; set; }

        [Import("FileTypeManager")]
        public FileTypeManagerViewModel FileTypeManagerViewModel { get; set; }

        [Import("ProductTypeManager")]
        public ProductTypeManagerViewModel ProductTypeManagerViewModel { get; set; }

        [Import("ProductPartTypeManager")]
        public ProductPartTypeManagerViewModel ProductPartTypeManagerViewModel { get; set; }

        [Import("ImportantPartManager")]
        public ImportantPartManagerViewModel ImportantPartManagerViewModel { get; set; }

        [Import("DepartmentManager")]
        public DepartmentManagerViewModel DepartmentManagerViewModel { get; set; }

        [Import("OnLineUserList")]
        public OnLineUserListViewModel OnLineUserListViewModel { get; set; }

        [Import("ProductManagerMain")]
        public ProductManagerMainViewModel ProductManagerMainViewModel { get; set;}

        [Import("ProductList")]
        public ProductListViewModel ProductListViewModel { get; set; }

        [Import("ResponsiblePersonList")]
        public ResponsiblePersonListViewModel ResponsiblePersonListViewModel { get; set; }

        [Import("QuestionTraceList")]
        public QuestionTraceListViewModel QuestionTraceListViewModel { get; set; }

        public UserInfo UserInfo { get; set; }

        //public ILogonPersionCallBack LogonPersionCallBack;

        public App()
        {
            SystemManageDomainContext = new SystemManageDomainContext();
            
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
            CompositionInitializer.SatisfyImports(this);
            WebContext webContext = new WebContext();
            webContext.Authentication = new FormsAuthentication();
            this.ApplicationLifetimeObjects.Add(webContext);

            //LogonPersionCallBack = new OnLinePersionCallBack();
            
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WebContext.Current.Authentication.LoadUser(this.LoadUser_Completed, null);
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            if (UserInfo != null)
            {
                LogoutUser(UserInfo.UserID, UserInfo.UserName);
            }
            
        }

        private void LoadUser_Completed(LoadUserOperation operation)
        {
            if (!operation.User.Identity.IsAuthenticated)
            {
                this.RootVisual = new MainLogin();
            }
            else
            {
                UserInfo = new UserInfo();
                ProductManager.Web.User lUser = operation.User.Identity as ProductManager.Web.User;
                UserInfo.UserName = lUser.UserName;
                UserInfo.UserAction = lUser.RightDictionary;
                UserInfo.UserDepartment = lUser.Department;
                UserInfo.UserID = lUser.UserID;
                UserInfo.DepartmentID = lUser.DepartmentID;
                UserInfo.UserPassword = lUser.Password;
                UserInfo.IsManager = lUser.IsManager;

                //InstanceContext context = new InstanceContext(LogonPersionCallBack);
                //IOnLinePersionService sc = new ProductManager.Web.OnLinePersionServiceContext(context);
                //sc.LogonPersion(UserInfo.UserID, UserInfo.UserName);
                LogonUser(UserInfo.UserID, UserInfo.UserName);
                this.RootVisual = new MainPage();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }

        private SystemManageDomainContext SystemManageDomainContext;//= new SystemManageDomainContext();

        public void LogonUser(int aUserID, String aUserName)
        {
            SystemManageDomainContext.LogonUser(aUserID, aUserName);
        }

        public void LogoutUser(int aUserID, String aUserName)
        {
            SystemManageDomainContext.LogoutUser(aUserID, aUserName);
        }

    }
}