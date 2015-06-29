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

namespace ProductManager.ViewData.Entity
{
    public class UserRemindEntity : NotifyPropertyChanged
    {
        private int userRemindId;
        public int UserRemindId
        {
            get
            {
                return userRemindId;
            }
            set
            {
                if (userRemindId != value)
                {
                    userRemindId = value;
                    UpdateChanged("UserRemindId");
                }
            }
        }

        private int userId;
        public int UserId
        {
            get
            {
                return userId;
            }
            set
            {
                if (userId != value)
                {
                    userId = value;
                    UpdateChanged("UserId");
                }
            }
        }

        private String manufactureNumber;
        public String ManufactureNumber
        {
            get
            {
                return manufactureNumber;
            }
            set
            {
                if (manufactureNumber != value)
                {
                    manufactureNumber = value;
                    UpdateChanged("ManufactureNumber");
                }
            }
        }

        private int remindOne;
        public int RemindOne
        {
            get
            {
                return remindOne;
            }
            set
            {
                if (remindOne != value)
                {
                    remindOne = value;
                    UpdateChanged("RemindOne");
                }
            }
        }

        private int remindTwo;
        public int RemindTwo
        {
            get
            {
                return remindTwo;
            }
            set
            {
                if (remindTwo != value)
                {
                    remindTwo = value;
                    UpdateChanged("RemindTwo");
                }
            }
        }

        private int remindThree;
        public int RemindThree
        {
            get
            {
                return remindThree;
            }
            set
            {
                if (remindThree != value)
                {
                    remindThree = value;
                    UpdateChanged("RemindThree");
                }
            }
        }

        public void Update()
        {
            this.userRemindId = UserRemind.user_remind_id;
            this.userId = UserRemind.user_id.GetValueOrDefault(-2);
            this.manufactureNumber = UserRemind.manufacture_number;
            this.remindOne = UserRemind.remind_one.GetValueOrDefault(2);
            this.remindTwo = UserRemind.remind_two.GetValueOrDefault(2);
            this.remindThree = UserRemind.remind_three.GetValueOrDefault(2);
        }

        public void DUpdate()
        {
            UserRemind.user_id = this.userId;
            UserRemind.manufacture_number = this.manufactureNumber;
            UserRemind.remind_one = this.remindOne;
            UserRemind.remind_two = this.remindTwo;
            UserRemind.remind_three = this.remindThree;
        }

        public void RaisALL()
        {
            UpdateChanged("UserRemindId");
            UpdateChanged("UserID");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("RemindOne");
            UpdateChanged("RemindTwo");
            UpdateChanged("RemindThree");
        }

        public ProductManager.Web.Model.user_remind UserRemind { get; set; }

    }
}
