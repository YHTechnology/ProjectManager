using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PDFTron.SilverDox.Controls;
using PDFTron.SilverDox.IO;


namespace ProductManager.Controls
{
    public partial class ReviewWindow : ChildWindow
    {
        private String fileUrl;

        public ReviewWindow(String aFileUrl)
        {
            InitializeComponent();
            fileUrl = aFileUrl;
        }

        public void LoadDocument()
        {
            try
            {
                Busy.IsBusy = true;
                Uri documentUri = new Uri(fileUrl);
                HttpPartRetriever myHttpPartRetriever = new HttpPartRetriever(documentUri);
                this.MyDocumentViewer.LoadAsync(myHttpPartRetriever, OnLoadAsyncCallback);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        public void OnLoadAsyncCallback(Exception ex)
        {
            if (ex != null)
            {
                //An error has occurred
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                MessageBox.Show("无法预览文件" + ex.Message);
                this.DialogResult = false;
            }

            MyDocumentViewer.SetFitMode(DocumentViewer.FitModes.Panel, DocumentViewer.FitModes.None);
            Busy.IsBusy = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.CurrentPageNumber -= 1;
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.CurrentPageNumber += 1;
        }

        private void ChildWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadDocument();
        }

        private void TrunLeftButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.RotateCounterClockwise();
        }

        private void TrunRightButton_Click(object sender, RoutedEventArgs e)
        {
            MyDocumentViewer.RotateClockwise();
        }

        private void ZoonInButton_Click(object sender, RoutedEventArgs e)
        {
            double lZoon = MyDocumentViewer.Zoom * 1.1;
            MyDocumentViewer.Zoom = lZoon;
        }

        private void ZoonOutButton_Click(object sender, RoutedEventArgs e)
        {
            double lZoon = MyDocumentViewer.Zoom / 1.1;
            MyDocumentViewer.Zoom = lZoon;
        }

    }
}

