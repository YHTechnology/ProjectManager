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
    public class DepartmentEntity : NotifyPropertyChanged
    {
        private int departmentID;
        public int DepartmentID
        {
            get
            {
                return departmentID;
            }
            set
            {
                if ( departmentID != value)
                {
                    departmentID = value;
                    UpdateChanged("DepartmentID");
                }
            }
        }

        private String departmentName;
        public String DepartmentName
        {
            get
            {
                return departmentName;
            }
            set
            {
                if (departmentName != value)
                {
                    departmentName = value;
                    UpdateChanged("DepartmentName");
                }
            }
        }

        public void Update()
        {
            this.departmentID   = Department.department_id;
            this.departmentName = Department.department_name;
        }

        public void DUpdate()
        {
            Department.department_id = this.departmentID;
            Department.department_name = this.departmentName;
        }

        public void RaisALL()
        {
            UpdateChanged("DepartmentID");
            UpdateChanged("DepartmentName");
        }

        public ProductManager.Web.Model.department Department { get; set; }
    }
}
