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
    public class QuestionTraceEntity : NotifyPropertyChanged
    {
        private int questionTraceID;
        public int QuestionTraceID
        {
            get
            {
                return questionTraceID;
            }
            set
            {
                if (questionTraceID != value)
                {
                    questionTraceID = value;
                    UpdateChanged("QuestionTraceID");
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

        private String projectName;
        public String ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                if (projectName != value)
                {
                    projectName = value;
                    UpdateChanged(ProjectName);
                }
            }
        }

        private String questionDescript;
        public String QuestionDescript
        {
            get
            {
                return questionDescript;
            }
            set
            {
                if (questionDescript != value)
                {
                    questionDescript = value;
                    UpdateChanged("QuestionDescript");
                }
            }
        }

        private String questionAnswer;
        public String QuestionAnswer
        {
            get
            {
                return questionAnswer;
            }
            set
            {
                if (questionAnswer != value)
                {
                    questionAnswer = value;
                    UpdateChanged("QuestionAnswer");
                }
            }
        }

        private int questionResDepartmentID;
        public int QuestionResDepartmentID
        {
            get
            {
                return questionResDepartmentID;
            }
            set
            {
                if (questionResDepartmentID != value)
                {
                    questionResDepartmentID = value;
                    UpdateChanged("QuestionResDepartmentID");
                }
            }
        }

        private String questionResDepartmentName;
        public String QuestionResDepartmentName
        {
            get
            {
                return questionResDepartmentName;
            }
            set
            {
                if (questionResDepartmentName != value)
                {
                    questionResDepartmentName = value;
                    UpdateChanged("QuestionResDepartmentName");
                }
            }
        }

        private int questionHandDepartmentID;
        public int QuestionHandDepartmentID
        {
            get
            {
                return questionHandDepartmentID;
            }
            set
            {
                if (questionHandDepartmentID != value)
                {
                    questionHandDepartmentID = value;
                    UpdateChanged("QuestionHandDepartmentID");
                }
            }
        }

        private String questionHandDepartmentName;
        public String QuestionHandDepartmentName
        {
            get
            {
                return questionHandDepartmentName;
            }
            set
            {
                if (questionHandDepartmentName != value)
                {
                    questionHandDepartmentName = value;
                    UpdateChanged("QuestionHandDepartmentName");
                }
            }
        }

        private int questionTraceDepartmentID;
        public int QuestionTraceDepartmentID
        {
            get
            {
                return questionTraceDepartmentID;
            }
            set
            {
                if (questionTraceDepartmentID != value)
                {
                    questionTraceDepartmentID = value;
                    UpdateChanged("QuestionTraceDepartmentID");
                }
            }
        }

        private String questionTraceDepartmentName;
        public String QuestionTraceDepartmentName
        {
            get
            {
                return questionTraceDepartmentName;
            }
            set
            {
                if (questionTraceDepartmentName != value)
                {
                    questionTraceDepartmentName = value;
                    UpdateChanged("QuestionTraceDepartmentName");
                }
            }
        }

        private Nullable<DateTime> questionStartTime;
        public Nullable<DateTime> QuestionStartTime
        {
            get
            {
                return questionStartTime;
            }
            set
            {
                if (questionStartTime != value)
                {
                    questionStartTime = value;
                    UpdateChanged("QuestionStartTime");
                    UpdateChanged("QuestionStartTimeString");
                }
            }
        }

        public String QuestionStartTimeString
        {
            get
            {
                if (questionStartTime.HasValue)
                {
                    return questionStartTime.Value.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }


        private Nullable<DateTime> questionLastAnswerTime;
        public Nullable<DateTime> QuestionLastAnswerTime
        {
            get
            {
                return questionLastAnswerTime;
            }
            set
            {
                if (questionLastAnswerTime != value)
                {
                    questionLastAnswerTime = value;
                    UpdateChanged("QuestionLastAnswerTime");
                    UpdateChanged("QuestionLastAnswerTimeString");
                }
            }
        }
        public String QuestionLastAnswerTimeString
        {
            get
            {
                if (questionLastAnswerTime.HasValue)
                {
                    return questionLastAnswerTime.Value.ToShortDateString() + " " + questionLastAnswerTime.Value.ToShortTimeString(); 
                }
                else
                {
                    return "";
                }
            }
        }

        private Nullable<DateTime> questionReqAnswerTime;
        public Nullable<DateTime> QuestionReqAnswerTime
        {
            get
            {
                return questionReqAnswerTime;
            }
            set
            {
                if (questionReqAnswerTime != value)
                {
                    questionReqAnswerTime = value;
                    UpdateChanged("QuestionReqAnswerTime");
                    UpdateChanged("questionReqAnswerTimeString");
                }
            }
        }
        public String questionReqAnswerTimeString
        {
            get
            {
                if (questionReqAnswerTime.HasValue)
                {
                    return questionReqAnswerTime.Value.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private bool questionIsClose;
        public bool QuestionIsClose
        {
            get
            {
                return questionIsClose;
            }
            set
            {
                if (questionIsClose != value)
                {
                    questionIsClose = value;
                    UpdateChanged("QuestionIsClose");
                }
            }
        }

        public String QuestionIsCloseString
        {
            get
            {
                if (questionIsClose)
                {
                    return "是";
                }
                else
                {
                    return "否";
                }
            }
        }

        private int userIDMain;
        public int UserIDMain
        {
            get
            {
                return userIDMain;
            }
            set
            {
                if (userIDMain != value)
                {
                    userIDMain = value;
                    UpdateChanged("userIDMain");
                }
            }
        }

        private String userIDMainString;
        public String UserIDMainString
        {
            get
            {
                return userIDMainString;
            }
            set
            {
                if (userIDMainString != value)
                {
                    userIDMainString = value;
                    UpdateChanged("UserIDMainString");
                }
            }
        }

        private int userIDCP1;
        public int UserIDCP1
        {
            get
            {
                return userIDCP1;
            }
            set
            {
                if (userIDCP1 != value)
                {
                    userIDCP1 = value;
                    UpdateChanged("UserIDCP1");
                }
            }
        }

        private String userIDCP1String;
        public String UserIDCP1String
        {
            get
            {
                return userIDCP1String;
            }
            set
            {
                if (userIDCP1String != value)
                {
                    userIDCP1String = value;
                    UpdateChanged("UserIDCP1String");
                }
            }
        }

        private int userIDCP2;
        public int UserIDCP2
        {
            get
            {
                return userIDCP2;
            }
            set
            {
                if (userIDCP2 != value)
                {
                    userIDCP2 = value;
                    UpdateChanged("UserIDCP2");
                }
            }
        }

        private String userIDCP2String;
        public String UserIDCP2String
        {
            get
            {
                return userIDCP2String;
            }
            set
            {
                if (userIDCP2String != value)
                {
                    userIDCP2String = value;
                    UpdateChanged("UserIDCP2String");
                }
            }
        }

        private int lastAnswerUserID;
        public int LastAnswerUserID
        {
            get
            {
                return lastAnswerUserID;
            }
            set
            {
                if (lastAnswerUserID != value)
                {
                    lastAnswerUserID = value;
                    UpdateChanged("LastAnswerUserID");
                }
            }
        }

        private String lastAnswerUserString;
        public String LastAnswerUserString
        {
            get
            {
                return lastAnswerUserString;
            }
            set
            {
                if (lastAnswerUserString != value)
                {
                    lastAnswerUserString = value;
                    UpdateChanged("LastAnswerUserString");
                }
            }
        }

        private int ownerUserID;
        public int OwnerUserID
        {
            get
            {
                return ownerUserID;
            }
            set
            {
                if (ownerUserID != value)
                {
                    ownerUserID = value;
                    UpdateChanged("OwnerUserID");
                }
            }
        }

        private String ownerUserString;
        public String OwnerUserString
        {
            get
            {
                return ownerUserString;
            }
            set
            {
                if (ownerUserString != value)
                {
                    ownerUserString = value;
                    UpdateChanged("OwnerUserString");
                }
            }
        }

        public void Update()
        {
            this.questionTraceID = QuestionTrace.questiontrace_id;
            this.manufactureNumber = QuestionTrace.manufacture_number;
            this.questionDescript = QuestionTrace.question_descript;
            this.questionAnswer = QuestionTrace.question_answer;
            this.questionResDepartmentID = QuestionTrace.question_res_departmentid.GetValueOrDefault(0);
            this.questionHandDepartmentID = QuestionTrace.question_hand_departmentid.GetValueOrDefault(0);
            this.questionTraceDepartmentID = QuestionTrace.question_trace_departmendid.GetValueOrDefault(0);
            this.questionStartTime = QuestionTrace.question_starttime;
            this.questionLastAnswerTime = QuestionTrace.question_lastanswertime;
            this.questionReqAnswerTime = QuestionTrace.question_req_answertime;
            this.questionIsClose = QuestionTrace.question_isclose.GetValueOrDefault(false);
            this.userIDMain = QuestionTrace.question_user_id_main.GetValueOrDefault(-1);
            this.userIDCP1 = QuestionTrace.question_user_id_cp1.GetValueOrDefault(-1);
            this.userIDCP2 = QuestionTrace.question_user_id_cp2.GetValueOrDefault(-1);
            this.lastAnswerUserID = QuestionTrace.question_last_answer_userid.GetValueOrDefault(-1);
            this.ownerUserID = QuestionTrace.question_owner_id.GetValueOrDefault(-1);
        }

        public void DUpdate()
        {
            //QuestionTrace.questiontrace_id = this.questionTraceID;
            QuestionTrace.manufacture_number = this.manufactureNumber;
            QuestionTrace.question_descript = this.questionDescript;
            QuestionTrace.question_answer = this.questionAnswer;
            QuestionTrace.question_res_departmentid = this.questionResDepartmentID;
            QuestionTrace.question_hand_departmentid = this.questionHandDepartmentID;
            QuestionTrace.question_trace_departmendid = this.questionTraceDepartmentID;
            QuestionTrace.question_starttime = this.questionStartTime;
            QuestionTrace.question_lastanswertime = this.questionLastAnswerTime;
            QuestionTrace.question_req_answertime = this.questionReqAnswerTime;
            QuestionTrace.question_isclose = this.questionIsClose;
            QuestionTrace.question_user_id_main = this.userIDMain;
            QuestionTrace.question_user_id_cp1 = this.userIDCP1;
            QuestionTrace.question_user_id_cp2 = this.userIDCP2;
            QuestionTrace.question_last_answer_userid = this.lastAnswerUserID;
            QuestionTrace.question_owner_id = this.ownerUserID;
        }

        public void RaisALL()
        {
            UpdateChanged("QuestionTraceID");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("QuestionDescript");
            UpdateChanged("QuestionAnswer");
            UpdateChanged("QuestionResDepartmentID");
            UpdateChanged("QuestionResDepartmentName");
            UpdateChanged("QuestionHandDepartmentID");
            UpdateChanged("QuestionHandDepartmentName");
            UpdateChanged("QuestionTraceDepartmentID");
            UpdateChanged("QuestionTraceDepartmentName");
            UpdateChanged("QuestionStartTime");
            UpdateChanged("QuestionStartTimeString");
            UpdateChanged("QuestionLastAnswerTime");
            UpdateChanged("QuestionLastAnswerTimeString");
            UpdateChanged("QuestionReqAnswerTime");
            UpdateChanged("questionReqAnswerTimeString");
            UpdateChanged("QuestionIsClose");
            UpdateChanged("userIDMain");
            UpdateChanged("UserIDMainString");
            UpdateChanged("UserIDCP1");
            UpdateChanged("UserIDCP1String");
            UpdateChanged("UserIDCP2");
            UpdateChanged("UserIDCP2String");
            UpdateChanged("LastAnswerUserID");
            UpdateChanged("LastAnswerUserString");
            UpdateChanged("OwnerUserID");
        }

        public ProductManager.Web.Model.questiontrace QuestionTrace { get; set; }
    }
}
