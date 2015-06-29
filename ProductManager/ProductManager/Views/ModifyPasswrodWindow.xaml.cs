﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.ViewModel;

namespace ProductManager.Views
{
    public partial class ModifyPasswrodWindow : ChildWindow
    {
        public ModifyPasswrodWindow()
        {
            InitializeComponent();
            this.DataContext = new ModifyPasswordWindowViewModel(this);
        }

//         private void OKButton_Click(object sender, RoutedEventArgs e)
//         {
//             this.DialogResult = true;
//         }

//         private void CancelButton_Click(object sender, RoutedEventArgs e)
//         {
//             this.DialogResult = false;
//         }
    }
}
