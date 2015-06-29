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
using ProductManager.ViewModel.ProductManagers;

namespace ProductManager.Views.ProductManagers
{
    public partial class UpdateFileWindow : ChildWindow
    {
        public UpdateFileWindow(ObservableCollection<FileTypeEntity> aFileTypeEntityList, ObservableCollection<ProjectFilesEntity> aProjectFilesEntityList, ProjectFilesEntity aProjectFileEntity)
        {
            InitializeComponent();
            this.DataContext = new UpdateFileWindowViewModel(this, aFileTypeEntityList, aProjectFilesEntityList, aProjectFileEntity);
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

