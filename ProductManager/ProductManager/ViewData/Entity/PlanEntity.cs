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
using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewData.Entity
{
    public class PlanEntity : NotifyPropertyChanged
    {
        private Nullable<DateTime> accomplishDate;
        public Nullable<DateTime> AccomplishDate
        {
            get { return accomplishDate; }
            set
            {
                if (accomplishDate != value)
                {
                    accomplishDate = value;
                    IsScoreSetable = accomplishDate.HasValue && isSetable;
                    if (!accomplishDate.HasValue)
                    {
                        Score = null;
                    }
                    UpdateChanged("AccomplishDate");
                    UpdateChanged("RowColor");
                }
            }
        }

        public SolidColorBrush RowColor
        {
            get
            {
                SolidColorBrush retrunValue;
                DateTime targetDateTime = TargetDateAdjustment2.HasValue ? TargetDateAdjustment2.Value :
                                            (TargetDateAdjustment1.HasValue ? TargetDateAdjustment1.Value : TargetDate);           
                if (null == AccomplishDate)
                {
                    DateTime currentDateTime = DateTime.Now;
                    TimeSpan difference = targetDateTime - currentDateTime;
                    if (difference.Days > PlanRemindDay)
                    {
                        retrunValue = new SolidColorBrush(Colors.Gray);
                    }
                    else if (difference.Days >= 0 && difference.Days <= PlanRemindDay)
                    {
                        retrunValue = new SolidColorBrush(Colors.Magenta);
                    }
                    else
                    {
                        retrunValue = new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    if (targetDateTime >= AccomplishDate)
                    {
                        retrunValue = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        retrunValue = new SolidColorBrush(Colors.Purple);
                    }
                }
                return retrunValue;
            }
           
        }

        private bool isScoreSetable = false;
        public bool IsScoreSetable
        {
            get { return isScoreSetable; }
            set
            {
                if (isScoreSetable != value)
                {
                    isScoreSetable = value && isSetable;
                    UpdateChanged("IsScoreSetable");
                }
            }
        }

        private bool isSetable = true;
        public bool IsSetable
        {
            get { return isSetable; }
            set
            {
                if (isSetable != value)
                {
                    isSetable = value;
                    isScoreSetable = isScoreSetable && isSetable;
                    UpdateChanged("IsScoreSetable");
                    UpdateChanged("IsSetable");
                }
            }
        }

        private string componentName;
        public string ComponentName
        {
            get { return componentName; }
            set
            {
                if (componentName != value)
                {
                    componentName = value;
                    UpdateChanged("ComponentName");
                }
            }
        }

        private int departmentId;
        public int DepartmentId
        {
            get { return departmentId; }
            set
            {
                if (departmentId != value)
                {
                    departmentId = value;
                    UpdateChanged("DepartmentId");
                }
            }
        }

        private string departmentName;
        public string DepartmentName
        {
            get { return departmentName; }
            set
            {
                if (departmentName != value)
                {
                    departmentName = value;
                    UpdateChanged("DepartmentName");
                }
            }
        }

        private string manufactureNumber;
        public string ManufactureNumber
        {
            get { return manufactureNumber; }
            set
            {
                if (manufactureNumber != value)
                {
                    manufactureNumber = value;
                    UpdateChanged("ManufactureNumber");
                }
            }
        }

        private string remark;
        public string Remark
        {
            get { return remark; }
            set
            {
                if (remark != value)
                {
                    remark = value;
                    UpdateChanged("Remark");
                }
            }
        }

        private Nullable<decimal> score;
        public Nullable<decimal> Score
        {
            get { return score; }
            set
            {
                if (score != value)
                {
                    score = value;
                    UpdateChanged("Score");
                }
            }
        }

        private int sequenceId;
        public int SequenceId
        {
            get { return sequenceId; }
            set
            {
                if (sequenceId != value)
                {
                    sequenceId = value;
                    UpdateChanged("SequenceId");
                }
            }
        }

        private string sheetName;
        public string SheetName
        {
            get { return sheetName; }
            set
            {
                if (sheetName != value)
                {
                    sheetName = value;
                    UpdateChanged("SheetName");
                }
            }
        }

        private DateTime targetDate;
        public DateTime TargetDate
        {
            get { return targetDate; }
            set
            {
                if (targetDate != value)
                {
                    targetDate = value;
                    UpdateChanged("TargetDate");
                }
            }
        }

        private Nullable<DateTime> targetDateAdjustment1;
        public Nullable<DateTime> TargetDateAdjustment1
        {
            get { return targetDateAdjustment1; }
            set
            {
                if (targetDateAdjustment1 != value)
                {
                    targetDateAdjustment1 = value;
                    UpdateChanged("TargetDateAdjustment1");
                }
            }
        }

        private Nullable<DateTime> targetDateAdjustment2;
        public Nullable<DateTime> TargetDateAdjustment2
        {
            get { return targetDateAdjustment2; }
            set
            {
                if (targetDateAdjustment2 != value)
                {
                    targetDateAdjustment2 = value;
                    UpdateChanged("TargetDateAdjustment2");
                }
            }
        }

        private string taskDescription;
        public string TaskDescription
        {
            get { return taskDescription; }
            set
            {
                if (taskDescription != value)
                {
                    taskDescription = value;
                    UpdateChanged("TaskDescription");
                }
            }
        }

        private string versionId;
        public string VersionId
        {
            get { return versionId; }
            set
            {
                if (versionId != value)
                {
                    versionId = value;
                    UpdateChanged("VersionId");
                }
            }
        }

        private decimal weight;
        public decimal Weight
        {
            get { return weight; }
            set
            {
                if (weight != value)
                {
                    weight = value;
                    UpdateChanged("Weigth");
                }
            }
        }

        private Nullable<DateTime> orderDate;
        public Nullable<DateTime> OrderDate
        {
            get { return orderDate; }
            set
            {
                if (orderDate != value)
                {
                    orderDate = value;
                    UpdateChanged("OrderDate");
                }
            }
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                if (projectName != value)
                {
                    projectName = value;
                    UpdateChanged("ProjectName");
                }
            }
        }

        private int planRemindDay;
        public int PlanRemindDay
        {
            get { return planRemindDay; }
            set
            {
                if (planRemindDay != value)
                {
                    planRemindDay = value;
                    UpdateChanged("PlanRemindDay");
                    UpdateChanged("RowColor");
                }
            }
        }

        public String RespUserName { get; set; }

        public void Update()
        {
            if (Plan.accomplish_date.HasValue)
            {
                accomplishDate = Plan.accomplish_date.Value;
            }
            IsScoreSetable = accomplishDate.HasValue && isSetable;
            componentName = Plan.component_name;

            if (Plan.department_id.HasValue)
            {
                departmentId = Plan.department_id.Value;
            }
            else
            {
                departmentId = 0;
            }
            manufactureNumber = Plan.manufacture_number;
            remark = Plan.remark;
            if (Plan.score.HasValue)
            {
                score = Plan.score.Value;
            }
 
            if (Plan.sequence_id.HasValue)
            {
                sequenceId = Plan.sequence_id.Value;
            }
           
            sheetName = Plan.sheet_name;

            if (Plan.target_date.HasValue)
            {
                targetDate = Plan.target_date.Value;
            }

            if (Plan.target_date_adjustment1.HasValue)
            {
                targetDateAdjustment1 = Plan.target_date_adjustment1.Value;
            }

            if (Plan.target_date_adjustment2.HasValue)
            {
                targetDateAdjustment2 = Plan.target_date_adjustment2.Value;
            }

            taskDescription = Plan.task_description;
            versionId = Plan.version_id;

            if (Plan.weight.HasValue)
            {
                weight = Plan.weight.Value;
            }

            if (Plan.accomplish_date.HasValue)
            {
                AccomplishDate = Plan.accomplish_date.Value;
            }

            if (Plan.order_date.HasValue)
            {
                OrderDate = Plan.order_date.Value;
            }
        }

        public bool IsNew()
        {
            bool returnValue = (null == Plan
                               || Plan.target_date_adjustment1 != targetDateAdjustment1
                               || Plan.target_date_adjustment2 != targetDateAdjustment2);
            return returnValue;
        }

        public void DUpdate()
        {
            if (null == Plan)
            {
                Plan = new ProductManager.Web.Model.plan();
            }
            Plan.accomplish_date = accomplishDate;
            Plan.component_name = componentName;
            if (0 != departmentId)
            {
                Plan.department_id = departmentId;
            }
            Plan.manufacture_number = manufactureNumber;
            Plan.remark = remark;
            Plan.score = score;
            Plan.sequence_id = sequenceId;
            Plan.sheet_name = sheetName;
            Plan.target_date = targetDate;
            Plan.target_date_adjustment1 = targetDateAdjustment1;
            Plan.target_date_adjustment2 = targetDateAdjustment2;
            Plan.accomplish_date = accomplishDate;
            Plan.task_description = taskDescription;
            Plan.version_id = versionId;
            Plan.weight = weight;
            Plan.order_date = orderDate;
        }

        public void RaisALL()
        {
            UpdateChanged("AccomplishDate");
            UpdateChanged("IsScoreSetable");
            UpdateChanged("IsSetable");
            UpdateChanged("ComponentName");
            UpdateChanged("DepartmentId");
            UpdateChanged("DepartmentName");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("Remark");
            UpdateChanged("Score");
            UpdateChanged("SequenceId");
            UpdateChanged("SheetName");
            UpdateChanged("TargetDate");
            UpdateChanged("TargetDateAdjustment1");
            UpdateChanged("TargetDateAdjustment2");
            UpdateChanged("TaskDescription");
            UpdateChanged("VersionId");
            UpdateChanged("Weigth");
            UpdateChanged("OrderDate");
            UpdateChanged("ProjectName");
            UpdateChanged("PlanRemindDay");
        }

        public ProductManager.Web.Model.plan Plan { get; set; }
    }
}
