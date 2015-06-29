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
    public class PlanExtraEntity : NotifyPropertyChanged
    {
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

        private string fileId;
        public string FileId
        {
            get { return fileId; }
            set
            {
                if (fileId != value)
                {
                    fileId = value;
                    UpdateChanged("FileId");
                }
            }
        }

        private string requirement;
        public string Requirement
        {
            get { return requirement; }
            set
            {
                if (requirement != value)
                {
                    requirement = value;
                    UpdateChanged("Requirement");
                }
            }
        }

        private string compilationBasis;
        public string CompilationBasis
        {
            get { return compilationBasis; }
            set
            {
                if (compilationBasis != value)
                {
                    compilationBasis = value;
                    UpdateChanged("CompilationBasis");
                }
            }
        }

        private string reasonAdjustment1;
        public string ReasonAdjustment1
        {
            get { return reasonAdjustment1; }
            set
            {
                if (reasonAdjustment1 != value)
                {
                    reasonAdjustment1 = value;
                    UpdateChanged("ReasonAdjustment1");
                }
            }
        }

        private string reasonAdjustment2;
        public string ReasonAdjustment2
        {
            get { return reasonAdjustment2; }
            set
            {
                if (reasonAdjustment2 != value)
                {
                    reasonAdjustment2 = value;
                    UpdateChanged("ReasonAdjustment2");
                }
            }
        }

        private string compileUserName;
        public string CompileUserName
        {
            get { return compileUserName; }
            set
            {
                if (compileUserName != value)
                {
                    compileUserName = value;
                    UpdateChanged("CompileUserName");
                }
            }
        }

        private Nullable<DateTime> compileDate;
        public Nullable<DateTime> CompileDate
        {
            get { return compileDate; }
            set
            {
                if (compileDate != value)
                {
                    compileDate = value;
                    UpdateChanged("CompileDate");
                }
            }
        }

        private string examineUserName;
        public string ExamineUserName
        {
            get { return examineUserName; }
            set
            {
                if (examineUserName != value)
                {
                    examineUserName = value;
                    UpdateChanged("ExamineUserName");
                }
            }
        }

        private Nullable<DateTime> examineDate;
        public Nullable<DateTime> ExamineDate
        {
            get { return examineDate; }
            set
            {
                if (examineDate != value)
                {
                    examineDate = value;
                    UpdateChanged("ExamineDate");
                }
            }
        }

        private string approveUserName;
        public string ApproveUserName
        {
            get { return approveUserName; }
            set
            {
                if (approveUserName != value)
                {
                    approveUserName = value;
                    UpdateChanged("ApproveUserName");
                }
            }
        }

        private Nullable<DateTime> approveDate;
        public Nullable<DateTime> ApproveDate
        {
            get { return approveDate; }
            set
            {
                if (approveDate != value)
                {
                    approveDate = value;
                    UpdateChanged("ApproveDate");
                }
            }
        }

        public void Update()
        {
            manufactureNumber = PlanExtra.manufacture_number;
            versionId = PlanExtra.version_id;
            fileId = PlanExtra.file_id;
            requirement = PlanExtra.requirement;
            compilationBasis = PlanExtra.compilation_basis;
            reasonAdjustment1 = PlanExtra.reason_adjustment1;
            reasonAdjustment2 = PlanExtra.reason_adjustment2;
            compileUserName = PlanExtra.compile_user_name;
            compileDate = PlanExtra.compile_date;
            examineUserName = PlanExtra.examine_user_name;
            examineDate = PlanExtra.examine_date;
            approveUserName = PlanExtra.approve_user_name;
            approveDate = PlanExtra.approve_date;
        }

        public void DUpdate()
        {
            if (null == PlanExtra)
            {
                PlanExtra = new ProductManager.Web.Model.plan_extra();
            }
            PlanExtra.manufacture_number = manufactureNumber;
            PlanExtra.version_id = versionId;
            PlanExtra.file_id = fileId;
            PlanExtra.requirement = requirement;
            PlanExtra.compilation_basis = compilationBasis;
            PlanExtra.reason_adjustment1 = reasonAdjustment1;
            PlanExtra.reason_adjustment2 = reasonAdjustment2;
            PlanExtra.compile_user_name = compileUserName;
            PlanExtra.compile_date = compileDate;
            PlanExtra.examine_user_name = examineUserName;
            PlanExtra.examine_date = examineDate;
            PlanExtra.approve_user_name = approveUserName;
            PlanExtra.approve_date = approveDate;        
        }

        public void RaisALL()
        {
            UpdateChanged("ManufactureNumber");
            UpdateChanged("VersionId");
            UpdateChanged("FileId");
            UpdateChanged("Requirement");
            UpdateChanged("CompilationBasis");
            UpdateChanged("ReasonAdjustment1");
            UpdateChanged("ReasonAdjustment2");
            UpdateChanged("CompileUserName");
            UpdateChanged("CompileDate");
            UpdateChanged("ExamineUserName");
            UpdateChanged("ExamineDate");
            UpdateChanged("ApproveUserName");
            UpdateChanged("ApproveDate");
        }

        public ProductManager.Web.Model.plan_extra PlanExtra { get; set; }
    }
}
