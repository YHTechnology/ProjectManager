using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.ViewData.Entity;

namespace ProductManager.ViewModel.ProductManagers
{
    public enum QuestionTraceOperation : uint
    {
        ADD = 0,
        ANSWER = 1,
        VIEW = 2,
        CLOSE = 3
    }

    public class QuestionTraceWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ObservableCollection<DepartmentEntity> DepartmentEntityList { get; set; }

        public ObservableCollection<UserEntity> UserEntityList { get; set; }

        public QuestionTraceEntity QuestionTraceEntity { get; set; }

        public QuestionTraceOperation QuestionTraceOperation { get; set; }

        private DepartmentEntity selectRspDepartmentEntity;
        public DepartmentEntity SelectRspDepartmentEntity
        {
            get
            {
                return selectRspDepartmentEntity;
            }
            set
            {
                if (selectRspDepartmentEntity != value)
                {
                    selectRspDepartmentEntity = value;
                    UpdateChanged("SelectRspDepartmentEntity");
                    QuestionTraceEntity.QuestionResDepartmentName = selectRspDepartmentEntity.DepartmentName;
                }
            }
        }
        
        private DepartmentEntity selectHandleDepartmentEntity;
        public DepartmentEntity SelectHandleDepartmentEntity
        {
            get
            {
                return selectHandleDepartmentEntity;
            }
            set
            {
                if (selectHandleDepartmentEntity != value)
                {
                    selectHandleDepartmentEntity = value;
                    UpdateChanged("SelectHandleDepartmentEntity");
                    QuestionTraceEntity.QuestionHandDepartmentName = selectHandleDepartmentEntity.DepartmentName;
                }
            }
        }

        private DepartmentEntity selectTraceDepartmentEntity;
        public DepartmentEntity SelectTraceDepartmentEntity
        {
            get
            {
                return selectTraceDepartmentEntity;
            }
            set
            {
                if (selectTraceDepartmentEntity != value)
                {
                    selectTraceDepartmentEntity = value;
                    UpdateChanged("SelectTraceDepartmentEntity");
                    QuestionTraceEntity.QuestionTraceDepartmentName = selectTraceDepartmentEntity.DepartmentName;
                }
            }
        }

        private UserEntity selectMainUserEntity;
        public UserEntity SelectMainUserEntity
        {
            get
            {
                return selectMainUserEntity;
            }
            set
            {
                if (selectMainUserEntity != value)
                {
                    selectMainUserEntity = value;
                    UpdateChanged("SelectMainUserEntity");
                }
            }
        }

        private UserEntity selectCP1UserEntity;
        public UserEntity SelectCP1UserEntity
        {
            get
            {
                return selectCP1UserEntity;
            }
            set
            {
                if (selectCP1UserEntity != value)
                {
                    selectCP1UserEntity = value;
                    UpdateChanged("SelectCP1UserEntity");
                }
            }
        }

        private UserEntity selectCP2UserEntity;
        public UserEntity SelectCP2UserEntity
        {
            get
            {
                return selectCP2UserEntity;
            }
            set
            {
                if (selectCP2UserEntity != value)
                {
                    selectCP2UserEntity = value;
                    UpdateChanged("SelectCP2UserEntity");
                }
            }
        }

        public ICommand OnOK { get; private set; }
        public ICommand OnCancel { get; private set; }

        public bool IsAdd { get; set; }
        public bool IsAnswer { get; set; }

        public String Title { get; set; }

        public QuestionTraceWindowViewModel(ChildWindow aChildWindow,  QuestionTraceOperation aQuestionTraceOperation, ObservableCollection<UserEntity> aUserEntityList, ObservableCollection<DepartmentEntity> aDepartmentEntityList, QuestionTraceEntity aQuestionTraceEntity )
        {
            childWindow = aChildWindow;
            QuestionTraceOperation = aQuestionTraceOperation;
            DepartmentEntityList = aDepartmentEntityList;
            UserEntityList = aUserEntityList;
            QuestionTraceEntity = aQuestionTraceEntity;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);

            if (aQuestionTraceOperation == QuestionTraceOperation.ADD)
            {
                IsAdd = true;
                IsAnswer = false;
                Title = "添加问题 " + QuestionTraceEntity.ManufactureNumber;
            }
            else if (aQuestionTraceOperation == QuestionTraceOperation.ANSWER)
            {
                IsAdd = false;
                IsAnswer = true;
                Title = "回答问题 " + QuestionTraceEntity.ManufactureNumber;
            }
            else if (aQuestionTraceOperation == QuestionTraceOperation.VIEW)
            {
                IsAdd = false;
                IsAnswer = false;
                Title = "查看问题 " + QuestionTraceEntity.ManufactureNumber;
            }
            else if (aQuestionTraceOperation == QuestionTraceOperation.CLOSE)
            {
                IsAdd = false;
                IsAnswer = false;
                Title = "关闭问题 " + QuestionTraceEntity.ManufactureNumber;
            }

        }

        private void OnOKCommand()
        {
            if (QuestionTraceOperation == QuestionTraceOperation.ANSWER)
            {
                this.QuestionTraceEntity.QuestionLastAnswerTime = DateTime.Now;
                App app = Application.Current as App;
                this.QuestionTraceEntity.LastAnswerUserID = app.UserInfo.UserID;
                this.QuestionTraceEntity.LastAnswerUserString = app.UserInfo.UserName;
            }
            if (QuestionTraceOperation == QuestionTraceOperation.CLOSE)
            {
                this.QuestionTraceEntity.QuestionIsClose = true;
            }
            if (selectRspDepartmentEntity != null && selectRspDepartmentEntity.DepartmentID != -1)
            {
                this.QuestionTraceEntity.QuestionResDepartmentName = selectRspDepartmentEntity.DepartmentName;
            }
            if (selectHandleDepartmentEntity != null && selectHandleDepartmentEntity.DepartmentID != -1)
            {
                this.QuestionTraceEntity.QuestionHandDepartmentName = selectHandleDepartmentEntity.DepartmentName;
            }
            if (selectTraceDepartmentEntity != null && selectTraceDepartmentEntity.DepartmentID != -1)
            {
                this.QuestionTraceEntity.QuestionTraceDepartmentName = selectTraceDepartmentEntity.DepartmentName;
            }
            if (selectMainUserEntity != null && selectMainUserEntity.UserID != -2)
            {
                this.QuestionTraceEntity.UserIDMainString = selectMainUserEntity.CUserName;
            }
            if (selectCP1UserEntity != null && selectCP1UserEntity.UserID != -2)
            {
                this.QuestionTraceEntity.UserIDCP1String = selectCP1UserEntity.CUserName;
            }
            if (selectCP2UserEntity != null && selectCP2UserEntity.UserID != -2)
            {
                this.QuestionTraceEntity.UserIDCP2String = selectCP2UserEntity.CUserName;
            }
            if (QuestionTraceOperation == QuestionTraceOperation.ADD)
            {
                App app = Application.Current as App;
                this.QuestionTraceEntity.OwnerUserID = app.UserInfo.UserID;
                this.QuestionTraceEntity.OwnerUserString = app.UserInfo.UserName;
                this.QuestionTraceEntity.QuestionStartTime = DateTime.Now;
            }
            this.QuestionTraceEntity.DUpdate();
            this.childWindow.DialogResult = true;
        }

        private void OnCancelCommand()
        {
            this.QuestionTraceEntity.Update();
            this.QuestionTraceEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
