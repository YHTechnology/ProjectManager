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
    public class UserProjectEntity : NotifyPropertyChanged
    {
        private int userProjectID;
        private int UserProjectID
        {
            get
            {
                return userProjectID;
            }
            set
            {
                if (userProjectID != value)
                {
                    userProjectID = value;
                    UpdateChanged("UserProjectEntity");
                }
            }
        }

        private int userID;
        public int UserID
        {
            get
            {
                return userID;
            }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    UpdateChanged("UserID");
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

        public void Update()
        {
            this.userProjectID = UserProject.user_project_id;
            this.manufactureNumber = UserProject.manufacture_number;
            this.userID = UserProject.user_id.GetValueOrDefault(-1);
        }

        public void DUpdate()
        {
            UserProject.user_project_id = this.userProjectID;
            UserProject.manufacture_number = this.manufactureNumber;
            UserProject.user_id = this.userID;
        }

        public void RaisALL()
        {
            UpdateChanged("UserProjectID");
            UpdateChanged("UserID");
            UpdateChanged("ManufactureNumber");
        }

        public ProductManager.Web.Model.user_project UserProject { get; set; }

    }
}
