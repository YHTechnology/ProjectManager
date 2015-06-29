using System;
using System.Collections.Generic;
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
    public class UserEntity : NotifyPropertyChanged
    {
        private int userID;
        public int UserID
        {
            get { return userID; }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    UpdateChanged("UserID");
                }
            }
        }

        private String userName;

        [Required(ErrorMessage = "用户名必填")]
        public String UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    UpdateChanged("UserName");
                    this.RemoveError("UserName");
                    CheckUserName();
                }
            }
        }

        private String cuserName;

        [Required(ErrorMessage = "姓名必填")]
        public String CUserName
        {
            get { return cuserName; }
            set
            {
                if (cuserName != value)
                {
                    cuserName = value;
                    UpdateChanged("CUserName");
                    this.RemoveError("CUserName");
                }
            }
        }

        private int userDepartmentID;

        [Range(1, 100000, ErrorMessage="请选择部门")]
        public int UserDepartmentID
        {
            get
            {
                return userDepartmentID;
            }
            set
            {
                if (userDepartmentID != value)
                {
                    userDepartmentID = value;
                    UpdateChanged("UserDepartmentID");
                    this.RemoveError("UserDepartmentID");
                }
            }
        }

        private String userDepartment;
        public String UserDepartment
        {
            get 
            {
                return userDepartment;
            }
            set 
            {
                if (userDepartment != value)
                {
                    userDepartment = value;
                    UpdateChanged("UserDepartment");
                }
            }
        }

        private bool isFreeze;
        public bool IsFreeze
        {
            get
            {
                return isFreeze;
            }
            set
            {
                if (isFreeze != value)
                {
                    isFreeze = value;
                    UpdateChanged("IsFreeze");
                    UpdateChanged("IsFreezeS");
                }
            }
        }

        public String IsFreezeS
        {
            get
            {
                if (IsFreeze)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        private bool isManager;
        public bool IsManager
        {
            get
            {
                return isManager;
            }
            set
            {
                if (isManager != value)
                {
                    isManager = value;
                    UpdateChanged("IsManager");
                }
            }
        }

        public String IsManagerString
        {
            get
            {
                if (isManager)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        public String userPhoneNumber;
        public String UserPhoneNumber
        {
            get
            {
                return userPhoneNumber;
            }
            set
            {
                if (userPhoneNumber != value)
                {
                    userPhoneNumber = value;
                    UpdateChanged("UserPhoneNumber");
                }
            }
        }

        public void Update()
        {
            userID = User.user_id;
            userName = User.user_name;
            cuserName = User.user_cname;
            if (User.user_department_id.HasValue)
            {
                userDepartmentID = User.user_department_id.Value;
            }
            else
            {
                userDepartmentID = 0;
            }

            if (User.user_is_freeze.HasValue)
            {
                isFreeze = User.user_is_freeze.Value;
            }
            else
            {
                isFreeze = false;
            }
            isManager = User.isManager.GetValueOrDefault(false);
            userPhoneNumber = User.user_phone_num;
        }

        public void DUpdate()
        {
            User.user_id            = userID;
            User.user_name          = userName;
            User.user_cname         = cuserName;
            User.user_is_freeze     = isFreeze;
            User.user_department_id = userDepartmentID;
            User.isManager          = isManager;
            User.user_phone_num     = userPhoneNumber;
        }

        private void CheckUserName()
        {
            if (string.IsNullOrWhiteSpace(this.userName)
                || string.IsNullOrWhiteSpace(this.userName))
            {
                return;
            }

            UserEntity lUserEntityTemp;
            if (UserNameDictionary.TryGetValue(this.userName, out lUserEntityTemp))
            {
                this.AddError("UserName", "用户名不能重复");
            }
            else
            {
                this.RemoveError("UserName");
            }
        }

        public void RaisALL()
        {
            UpdateChanged("UserID");
            UpdateChanged("UserName");
            UpdateChanged("CUserName");
            UpdateChanged("UserDepartmentID");
            UpdateChanged("UserDepartment");
            UpdateChanged("IsFreeze");
            UpdateChanged("IsFreezeS");
            UpdateChanged("IsManager");
            UpdateChanged("IsManagerString");
            UpdateChanged("UserPhoneNumber");
        }

        public Dictionary<String, UserEntity> UserNameDictionary { get; set; }

        public ProductManager.Web.Model.user User { get; set; }
    }
}
