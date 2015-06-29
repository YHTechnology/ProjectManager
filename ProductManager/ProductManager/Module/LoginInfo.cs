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

namespace ProductManager.Module
{
    public class LoginInfo : NotifyPropertyChanged
    {
        private string _userName;

        private string _password;

        private bool _isRemember;


        [Required(ErrorMessage = "用户名必填")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    UpdateChanged("UserName");
                    RemoveError("UserName");
                }
            }
        }

        [Required(ErrorMessage = "密码必填")]
        public string Password
        {
            get { return _password; }
            set 
            {
                if (_password != value)
                {
                    _password = value;
                    UpdateChanged("Password");
                    RemoveError("Password");
                }
            }
        }

        public bool IsRemember
        {
            get { return _isRemember; }
            set
            {
                if (_isRemember != value)
                {
                    _isRemember = value;
                    UpdateChanged("IsRemember");
                }
            }
        }
    }
}
