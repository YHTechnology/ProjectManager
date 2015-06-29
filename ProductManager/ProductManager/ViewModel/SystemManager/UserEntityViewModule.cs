using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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

namespace ProductManager.ViewModel.SystemManager
{
    public enum UserEntityViewType : uint
    {
        ADD = 0,
        Modify = 1
    }

    public class UserEntityViewModule : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public UserEntity UserEntity { get; set; }

        public String Title { get; set; }

        private ObservableCollection<DepartmentEntity> departmentList;

        //[Import("DepartmentList")]
        public ObservableCollection<DepartmentEntity> DepartmentList
        {
            get
            {
                return departmentList;
            }
            set
            {
                if (departmentList != value)
                {
                    departmentList = value;
                }
            }
        }

        public DepartmentEntity SelectDepartmentEntity { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public ICommand OnClose { get; private set; }

        private UserEntityViewType UserEntityVIewType { get; set; }

        public bool IsAdd { get; set; }

        public UserEntityViewModule(ChildWindow aChileWindow, UserEntityViewType aUserEntityViewType, UserEntity aUserEntity, ObservableCollection<DepartmentEntity> aDepartmentEntity)
        {
            this.UserEntityVIewType = aUserEntityViewType;
            this.childWindow = aChileWindow;
            this.UserEntity = aUserEntity;
            this.DepartmentList = aDepartmentEntity;
            if(String.IsNullOrEmpty(aUserEntity.UserName))
            {
                Title = "添加用户";
            }
            else
            {
                Title = "修改用户 编号：" + aUserEntity.UserID.ToString();
            }
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
            OnClose = new DelegateCommand(OnCloseCommand);

            if (aUserEntityViewType == UserEntityViewType.ADD)
            {
                IsAdd = true;
            }
            else
            {
                IsAdd = false;
            }

            CompositionInitializer.SatisfyImports(this);
        }

        public void OnOKCommand()
        {
            if (this.UserEntity.Validate())
            {
                if (SelectDepartmentEntity != null)
                {
                    this.UserEntity.UserDepartment = SelectDepartmentEntity.DepartmentName;
                }
                this.UserEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.UserEntity.Update();
            this.UserEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }

        public void OnCloseCommand()
        {
            this.UserEntity.Update();
            this.childWindow.DialogResult = false;
        }
    }
}
