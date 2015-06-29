using System;
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

namespace ProductManager.Controls
{
    public partial class NotifyWindow : ChildWindow
    {

        private String ContextNotify { get; set; }
        //private String Title { get; set; }

        public NotifyWindow(String aTitle, String aContextNotify)
        {
            InitializeComponent();
            ContextNotify = aContextNotify;
            this.Title = aTitle;
            //Title = aTitle;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyContext.Text = ContextNotify;
            this.Title = Title;
        }
    }
}

