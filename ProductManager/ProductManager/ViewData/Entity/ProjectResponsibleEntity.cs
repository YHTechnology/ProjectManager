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
    public class ProjectResponsibleEntity : NotifyPropertyChanged
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
                if (departmentID != value)
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

        private DateTime finishTime;
        public DateTime FinishTime
        {
            get
            {
                return finishTime;
            }
            set
            {
                if (finishTime != value)
                {
                    finishTime = value;
                    UpdateChanged("FinishTime");
                    UpdateChanged("FinishTimeString");
                }
            }
        }
        public String FinishTimeString
        {
            get
            {
                if (finishTime != DateTime.MinValue)
                {
                    return finishTime.ToShortDateString();
                } 
                else
                {
                    return "";
                }
            }
        }

        private bool isfinished;
        public bool Isfinished
        {
            get
            {
                return isfinished;
            }
            set
            {
                if (isfinished != value)
                {
                    isfinished = value;
                    UpdateChanged("Isfinished");
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

        private int projectResponsibleID;
        public int ProjectResponsibleID
        {
            get
            {
                return projectResponsibleID;
            }
            set
            {
                if (projectResponsibleID != value)
                {
                    projectResponsibleID = value;
                    UpdateChanged("ProjectResponsibleID");
                }
            }
        }

        private String responsiblePersionName;
        public String ResponsiblePersionName
        {
            get
            {
                return responsiblePersionName;
            }
            set
            {
                if (responsiblePersionName != value)
                {
                    responsiblePersionName = value;
                    UpdateChanged("ResponsiblePersionName");
                }
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    UpdateChanged("StartTime");
                    UpdateChanged("StartTimeString");
                }
            }
        }
        public String StartTimeString
        {
            get
            {
                if (startTime != DateTime.MinValue)
                {
                    return startTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String descript;
        public String Descript
        {
            get
            {
                return descript;
            }
            set
            {
                if (descript != value)
                {
                    descript = value;
                    UpdateChanged("Descript");
                }
            }
        }

        private String userPhoneNumber;
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
            this.departmentID = ProjectResponsible.department_id.GetValueOrDefault(0);
            this.finishTime = ProjectResponsible.finish_time.GetValueOrDefault();
            this.isfinished = ProjectResponsible.isfinished.GetValueOrDefault(false);
            this.manufactureNumber = ProjectResponsible.manufacture_number;
            this.projectResponsibleID = ProjectResponsible.project_responsible_id;
            this.responsiblePersionName = ProjectResponsible.responsible_persionName;
            this.startTime = ProjectResponsible.start_time.GetValueOrDefault();
            this.descript = ProjectResponsible.descript;
        }

        public void DUpdate()
        {
            ProjectResponsible.department_id = this.departmentID;
            ProjectResponsible.finish_time = this.finishTime;
            ProjectResponsible.isfinished = this.isfinished;
            ProjectResponsible.manufacture_number = this.manufactureNumber;
            ProjectResponsible.project_responsible_id = this.projectResponsibleID;
            ProjectResponsible.responsible_persionName = this.responsiblePersionName;
            ProjectResponsible.start_time = this.startTime;
            ProjectResponsible.descript = this.descript;
        }

        public void RaisALL()
        {
            UpdateChanged("DepartmentID");
            UpdateChanged("FinishTime");
            UpdateChanged("FinishTimeString");
            UpdateChanged("Isfinished");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ProjectResponsibleID");
            UpdateChanged("ResponsiblePersionName");
            UpdateChanged("StartTime");
            UpdateChanged("StartTimeString");
            UpdateChanged("Descript");
        }

        public ProductManager.Web.Model.project_responsible ProjectResponsible { get; set; }

    }
}
