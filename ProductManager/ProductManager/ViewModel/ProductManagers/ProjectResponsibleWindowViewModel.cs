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
    public class ProjectResponsibleWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProjectResponsibleEntity ProjectResponsibleEntity { get; set; }

        public String Title { get; set; }

        private ObservableCollection<DepartmentEntity> departmentList;

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

        public ProjectResponsibleWindowViewModel(ChildWindow aChileWindow, ObservableCollection<DepartmentEntity> aDepartmentEntity, ProjectResponsibleEntity aProjectResponsibleEntity)
        {
            this.childWindow = aChileWindow;
            this.ProjectResponsibleEntity = aProjectResponsibleEntity;
            this.DepartmentList = aDepartmentEntity;

            Title = "生产令号：" + aProjectResponsibleEntity.ManufactureNumber;
            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.ProjectResponsibleEntity.Validate())
            {
                if (SelectDepartmentEntity != null)
                {
                    this.ProjectResponsibleEntity.DepartmentName = SelectDepartmentEntity.DepartmentName;
                }
                this.ProjectResponsibleEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.ProjectResponsibleEntity.Update();
            this.ProjectResponsibleEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }
    }
}
