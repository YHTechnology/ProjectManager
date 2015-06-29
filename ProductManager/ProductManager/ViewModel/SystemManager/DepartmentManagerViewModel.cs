using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;
using ProductManager.Views.SystemManager;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.SystemManager
{
    [Export("DepartmentManager")]
    public class DepartmentManagerViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext;

        public ObservableCollection<DepartmentEntity> DepartmentEntityList { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;

                }
            }
        }

        private DepartmentEntity selectDepartmentEntity;
        public DepartmentEntity SelectDepartmentEntity
        {
            get
            {
                return selectDepartmentEntity;
            }
            set
            {
                if (selectDepartmentEntity != value)
                {
                    selectDepartmentEntity = value;
                    UpdateChanged("SelectDepartmentEntity");
                    (OnModify as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private DepartmentEntity AddDepartmentEntity;

        public ICommand OnAdd { get; private set; }
        public ICommand OnModify { get; private set; }
        public ICommand DoubleClick { get; private set; }

        public DepartmentManagerViewModel()
        {
            DepartmentEntityList = new ObservableCollection<DepartmentEntity>();
            OnAdd = new DelegateCommand(OnAddCommand);
            OnModify = new DelegateCommand(OnModifyCommand, CanModifyCommand);
            DoubleClick = new DelegateCommand(OnDoubleClickCommand);
        }

        public void LoadData()
        {
            systemManageDomainContext = new SystemManageDomainContext();
            SelectDepartmentEntity = null;
            IsBusy = true;
            LoadOperation<ProductManager.Web.Model.department> loadOperationDepartment =
                systemManageDomainContext.Load<ProductManager.Web.Model.department>(systemManageDomainContext.GetDepartmentQuery());
            loadOperationDepartment.Completed += loadOperationDepartment_Completed;
        }

        private void loadOperationDepartment_Completed(object sender, EventArgs e)
        {
            DepartmentEntityList.Clear();
            LoadOperation loadOperation = sender as LoadOperation;
            foreach (ProductManager.Web.Model.department department in loadOperation.Entities)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.Department = department;
                departmentEntity.Update();
                DepartmentEntityList.Add(departmentEntity);
            }
            UpdateChanged("DepartmentEntityList");
            IsBusy = false;
        }

        private void OnAddCommand()
        {
            AddDepartmentEntity = new DepartmentEntity();
            AddDepartmentEntity.Department = new ProductManager.Web.Model.department();
            AddDepartmentEntity.Update();
            DepartmentWindow departmentWindow = new DepartmentWindow(AddDepartmentEntity);
            departmentWindow.Closed += departmentWindow_Closed;
            departmentWindow.Show();
        }

        private void departmentWindow_Closed(object sender, EventArgs e)
        {
            DepartmentWindow departmentWindow = sender as DepartmentWindow;
            if (departmentWindow.DialogResult == true)
            {
                DepartmentEntityList.Add(AddDepartmentEntity);
                systemManageDomainContext.departments.Add(AddDepartmentEntity.Department);
                IsBusy = true;
                SubmitOperation submitOperation = systemManageDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            SubmitOperation submitOperation = sender as SubmitOperation;

            if (submitOperation.HasError)
            {
                submitOperation.MarkErrorAsHandled();
                NotifyWindow notifyWindow = new NotifyWindow("错误", "保存失败");
                notifyWindow.Show();
            }
            else
            {
                NotifyWindow notifyWindow = new NotifyWindow("保存成功", "保存成功！");
                notifyWindow.Show();
            }
            IsBusy = false;
        }

        private void OnModifyCommand()
        {
            DepartmentWindow departmentWindow = new DepartmentWindow(SelectDepartmentEntity);
            departmentWindow.Closed += departmentWindowmodify_Closed;
            departmentWindow.Show();
        }

        private void departmentWindowmodify_Closed(object sender, EventArgs e)
        {
            DepartmentWindow departmentWindow = sender as DepartmentWindow;
            if (departmentWindow.DialogResult == true)
            {
                IsBusy = true;
                SubmitOperation submitOperation = systemManageDomainContext.SubmitChanges();
                submitOperation.Completed += submitOperation_Completed;
            }
        }

        private bool CanModifyCommand(object aObject)
        {
            if (selectDepartmentEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnDoubleClickCommand()
        {
            if (SelectDepartmentEntity != null)
            {
                DepartmentWindow departmentWindow = new DepartmentWindow(SelectDepartmentEntity);
                departmentWindow.Closed += departmentWindowmodify_Closed;
                departmentWindow.Show();
            }
        }

    }
}
