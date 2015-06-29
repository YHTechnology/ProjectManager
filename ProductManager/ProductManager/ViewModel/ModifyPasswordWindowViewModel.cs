using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;
using System.ServiceModel.DomainServices.Client;
using ProductManager.Controls;

namespace ProductManager.ViewModel
{
    public class ModifyPasswordWindowViewModel : NotifyPropertyChanged
    {
        public ModifyPasswordEntity ModifyPasswordEntity { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        public ICommand OnModifyPassword { get; private set; }

        public ICommand OnCancel { get; private set; }

        private ChildWindow ChildWindow;

        public ModifyPasswordWindowViewModel(ChildWindow aChileWindow)
        {
            this.ChildWindow = aChileWindow;
            ModifyPasswordEntity = new ModifyPasswordEntity();
            OnModifyPassword = new DelegateCommand(OnModifyPasswordCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        private void OnModifyPasswordCommand()
        {
            if (ModifyPasswordEntity.Validate())
            {
                IsBusy = true;

                OperationDomainContext operationDomainContext = new OperationDomainContext();

                App app = Application.Current as App;

                if (app.UserInfo.UserPassword == Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword))
                {
                    NotifyWindow notifyWindow = new NotifyWindow("密码验证", "请输入新密码");
                    notifyWindow.Show();
                    IsBusy = false;
                    return;
                }
                InvokeOperation<bool> lModifyPassword = operationDomainContext.ModifyPassword(app.UserInfo.UserID, Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword));
                lModifyPassword.Completed += ModifyPassword_Completed;
            }
        }

        void ModifyPassword_Completed(object sender, EventArgs e)
        {
            IsBusy = false;
            var lValue = (System.ServiceModel.DomainServices.Client.InvokeOperation<bool>)sender;
            if (lValue.Value)
            {
                App app = Application.Current as App;
                app.UserInfo.UserPassword = Cryptography.MD5.GetMd5String(ModifyPasswordEntity.NewPassword);
                ChildWindow.DialogResult = true;
                MessageBox.Show("密码修改成功");           
            }
            else
            {
                 MessageBox.Show("密码修改失败");
            }
        }

        private void OnCancelCommand()
        {
            ChildWindow.DialogResult = false;
        }

    }
}
