using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProductManager.ViewData.Entity
{
    public class ModifyPasswordEntity : NotifyPropertyChanged
    {
        private String oldPassword;

        [Required(ErrorMessage = "原密码必填")]
        public String OldPassword
        {
            get
            {
                return oldPassword;
            }
            set
            {
                if (oldPassword != value)
                {
                    oldPassword = value;
                    CheckOldPassword();
                    UpdateChanged("OldPassword");
                }
            }
        }

        private String newPassword;

        [Required(ErrorMessage = "新密码必填")]
        public String NewPassword
        {
            get
            {
                return newPassword;
            }
            set
            {
                if (newPassword != value)
                {
                    newPassword = value;
                    UpdateChanged("NewPassword");
                    RemoveError("NewPassword");
                }
            }
        }

        private String repNewPassword;
        
        [Required(ErrorMessage = "确认密码必填")]
        public String RepNewPassword
        {
            get
            {
                return repNewPassword;
            }
            set
            {
                if (repNewPassword != value)
                {
                    repNewPassword = value;
                    CheckPassword();
                    UpdateChanged("RepNewPassword");
                }
            }
        }

        private void CheckPassword()
        {
            if (string.IsNullOrWhiteSpace(this.newPassword)
                || string.IsNullOrWhiteSpace(this.newPassword))
            {
                return;
            }

            if (this.newPassword != this.repNewPassword)
            {
                this.AddError("RepNewPassword", "两次输入密码应该一致" );
            }
            else
            {
                this.RemoveError("RepNewPassword");
            }
        }

        private void CheckOldPassword()
        {
            if (string.IsNullOrWhiteSpace(this.oldPassword)
                || string.IsNullOrWhiteSpace(this.oldPassword))
            {
                return;
            }

            App app = Application.Current as App;
            String lOldPassword = app.UserInfo.UserPassword;

            if (lOldPassword != Cryptography.MD5CryptoServiceProvider.GetMd5String(oldPassword))
            {
                this.AddError("OldPassword", "与旧密码应该一致");
            }
            else
            {
                this.RemoveError("OldPassword");
            }
        }
    }
}
