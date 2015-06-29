using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.ViewData.Entity;
using ProductManager.ViewModel.SystemManager;

namespace ProductManager.Views.SystemManager
{
    public partial class UserEntityWindow : ChildWindow
    {
        public UserEntityWindow(UserEntityViewType aUserEntityViewType, UserEntity aUserEntity, ObservableCollection<DepartmentEntity> aDepartmentEntity)
        {
            InitializeComponent();
            UserEntityViewModule userEntityViewModule = new UserEntityViewModule(this, aUserEntityViewType, aUserEntity, aDepartmentEntity);
            this.DataContext = userEntityViewModule;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

