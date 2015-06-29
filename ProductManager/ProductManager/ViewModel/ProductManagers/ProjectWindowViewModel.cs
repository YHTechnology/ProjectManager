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
using ProductManager.ViewData.Entity;

namespace ProductManager.ViewModel.ProductManagers
{
    public enum ProjectWindowType : uint
    {
        ADD = 0,
        Modify = 1,
        View = 2
    }

    public class ProjectWindowViewModel : NotifyPropertyChanged
    {
        private ChildWindow childWindow;

        public ProjectEntity ProjectEntity { get; set; }

        public String Title { get; set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }
        
        private ProjectWindowType projectWindowType { get; set; }

        public bool IsAdd { get; set; }

        public bool IsModify { get; set; }

        public ProjectWindowViewModel(ChildWindow aChildWindow, ProjectWindowType aProductWindowType, ProjectEntity aProjectEntity)
        {
            
            childWindow = aChildWindow;
            ProjectEntity = aProjectEntity;
            if (!String.IsNullOrWhiteSpace(ProjectEntity.ManufactureNumber))
            {
                Title = "生产令号：" + ProjectEntity.ManufactureNumber;
            }
            else
            {
                Title = "添加";
            }

            projectWindowType = aProductWindowType;
            if (projectWindowType == ProjectWindowType.Modify)
            {
                IsModify = true;
                IsAdd = false;
            }
            else if (projectWindowType == ProjectWindowType.ADD)
            {
                IsAdd = true;
                IsModify = true;
            }
            else if (projectWindowType == ProjectWindowType.View)
            {
                IsAdd = false;
                IsModify = false;
            }


            OnOK = new DelegateCommand(OnOKCommand);
            OnCancel = new DelegateCommand(OnCancelCommand);
        }

        public void OnOKCommand()
        {
            if (this.ProjectEntity.Validate())
            {
                this.ProjectEntity.DUpdate();
                this.childWindow.DialogResult = true;
            }
        }

        public void OnCancelCommand()
        {
            this.ProjectEntity.Update();
            this.ProjectEntity.RaisALL();
            this.childWindow.DialogResult = false;
        }


    }
}
