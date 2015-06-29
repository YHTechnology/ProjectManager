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
using System.Windows.Printing;
using System.Windows.Shapes;

namespace ProductManager.Controls
{
    public partial class PointWindow : ChildWindow
    {
        private ReportParame m_ReportParame = null;
        private List<Grid> m_PageList = new List<Grid>();
        private String m_Name;

        public PointWindow(ReportParame aReportParame, string aName)
        {
            m_ReportParame = aReportParame;
            m_Name = aName;
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument lPoint = new PrintDocument();
            lPoint.PrintPage += new EventHandler<PrintPageEventArgs>(report_PrintPage);
            lPoint.Print("Report");

        }

        void report_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (m_PageList.Count > 0)
            {
                e.PageVisual = m_PageList.First();
                m_PageList.RemoveAt(0);
                if (m_PageList.Count > 0)
                {
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int lCount = 0;
            Grid lPage = null;
            int lPageNumber = 1;

            int lTotalPage = m_ReportParame.Values.Count / 28 + 1;

            foreach (List<String> lValues in m_ReportParame.Values)
            {
                if (lCount == 0)
                {
                    Border lPageBorder = new Border();
                    lPageBorder.Margin = new System.Windows.Thickness(15, 15, 15, 0);
                    lPageBorder.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                    lPageBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    lPageBorder.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

                    Grid lPagePointer = new Grid();

                    Grid lPageContion = new Grid();
                    lPageContion.HorizontalAlignment = HorizontalAlignment.Center;
                    
                    lPageContion.Margin = new System.Windows.Thickness(35, 30, 35, 0);
                    lPageBorder.Child = lPagePointer;
                    
                    //lPageContion.HorizontalAlignment = HorizontalAlignment.Stretch;
                    
                    lPagePointer.Children.Add(lPageContion);

                    RowDefinition lHeader = new RowDefinition();
                    lPageContion.RowDefinitions.Add(lHeader);
                    RowDefinition lContext = new RowDefinition();
                    //lContext.MinHeight = 650;
                    lPageContion.RowDefinitions.Add(lContext);
                    RowDefinition lFooder = new RowDefinition();
                    lPageContion.RowDefinitions.Add(lFooder);

                    Border lHeaderBorder = new Border();
                    lHeaderBorder.BorderThickness = new System.Windows.Thickness(0, 0, 0, 1);
                    lHeaderBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                    StackPanel lStackPanel = new StackPanel();

                    lStackPanel.Orientation = Orientation.Vertical;

                    TextBlock lHeaderText = new TextBlock();
                    lHeaderText.Text = m_ReportParame.ReportTitle;
                    lHeaderText.HorizontalAlignment = HorizontalAlignment.Center;
                    lHeaderText.FontSize = 21;
                    //lHeaderText.SetValue(TextBlock.FontWeightProperty, "Bold");
                    lHeaderText.Margin = new System.Windows.Thickness(0, 10, 0, 15);
                    lHeaderText.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    //lHeaderBorder.Child = lHeaderText;

                    lStackPanel.Children.Add(lHeaderText);

                    TextBlock lHeaderTextName = new TextBlock();
                    lHeaderTextName.Text = "制表人：" + m_Name + "      日期：" + DateTime.Now.ToShortDateString();
                    lHeaderTextName.HorizontalAlignment = HorizontalAlignment.Center;
                    //lHeaderTextName.FontSize = 21;
                    lHeaderTextName.Margin = new System.Windows.Thickness(0, 10, 0, 10);
                    lHeaderTextName.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                    lStackPanel.Children.Add(lHeaderTextName);

                    Border lFooderBorder = new Border();
                    lFooderBorder.BorderThickness = new System.Windows.Thickness(0, 1, 0, 0);
                    lFooderBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                    TextBlock lFooderText = new TextBlock();
                    lFooderText.Text = "- 第 " + lPageNumber + " 页，共 " + lTotalPage + " 页 -";
                    lFooderText.Margin = new System.Windows.Thickness(0, 10, 0, 10);
                    lFooderText.HorizontalAlignment = HorizontalAlignment.Center;
                    lFooderText.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    //lFooderBorder.Child = lFooderText;

                    lPageContion.Children.Add(lStackPanel);
                    lHeaderText.SetValue(Grid.RowProperty, 0);
                    lPageContion.Children.Add(lFooderText);
                    lFooderText.SetValue(Grid.RowProperty, 2);

                    Border lPageContextBorder = new Border();
                    lPageContextBorder.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                    lPageContextBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    lPageContextBorder.VerticalAlignment = VerticalAlignment.Top;
                    //lPageContextBorder.Margin = new System.Windows.Thickness(0, 5, 0, 0);

                    lPage = new Grid();
                    lPage.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                    lPage.VerticalAlignment = VerticalAlignment.Top;

                    //for (int i = 0; i < m_ReportParame.Title.Count(); i++)
                    foreach (int lWidth in m_ReportParame.TitleWidth)
                    {
                        ColumnDefinition lColumnDefinition = new ColumnDefinition();
                        lColumnDefinition.MinWidth = lWidth;
                        lPage.ColumnDefinitions.Add(lColumnDefinition);
                    }
                    //{
                    //    ColumnDefinition lColumnDefinition = new ColumnDefinition();
                    //    lPage.ColumnDefinitions.Add(lColumnDefinition);
                    //}


                    lPageContextBorder.Child = lPage;
                    lPageContion.Children.Add(lPageContextBorder);
                    lPageContextBorder.SetValue(Grid.RowProperty, 1);

                    //define header
                    RowDefinition lHeaderRow = new RowDefinition();
                    lPage.RowDefinitions.Add(lHeaderRow);

                    int lIndex = 0;
                    foreach (String lName in m_ReportParame.Title)
                    {
                        Border lHeaderTextBorder0 = new Border();
                        if (lIndex < m_ReportParame.Title.Count() - 1)
                        {
                            lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(0, 0, 1, 0);
                        }
                        else
                        {
                            lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(0, 0, 0, 0);
                        }
                        //lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                        lHeaderTextBorder0.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                        lHeaderTextBorder0.Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                        TextBlock lHeaderText0 = new TextBlock();
                        lHeaderText0.Text = lName;
                        lHeaderText0.FontSize = 14;
                        lHeaderText0.HorizontalAlignment = HorizontalAlignment.Center;

                        lHeaderText0.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                        lHeaderText0.Margin = new System.Windows.Thickness(5, 2, 0, 2);
                        lHeaderTextBorder0.Child = lHeaderText0;
                        lPage.Children.Add(lHeaderTextBorder0);
                        lHeaderTextBorder0.SetValue(Grid.RowProperty, 0);
                        lHeaderTextBorder0.SetValue(Grid.ColumnProperty, lIndex);
                        lIndex++;
                    }

                    PintConter.Children.Add(lPageBorder);

                    lPageNumber++;

                    m_PageList.Add(lPagePointer);
                }
                lCount++;

                RowDefinition lValueRow = new RowDefinition();
                lPage.RowDefinitions.Add(lValueRow);

                int lColIndex = 0;
                foreach (String lName in lValues)
                {
                    Border lHeaderTextBorder0 = new Border();
                    if (lColIndex < lValues.Count() - 1)
                    {
                        lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(0, 1, 1, 0);
                    }
                    else
                    {
                        lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(0, 1, 0, 0);
                    }

                    //lHeaderTextBorder0.BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
                    lHeaderTextBorder0.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    TextBlock lHeaderText0 = new TextBlock();
                    lHeaderText0.Text = lName;
                    lHeaderText0.Margin = new System.Windows.Thickness(5, 2, 0, 2);
                    lHeaderText0.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    lHeaderTextBorder0.Child = lHeaderText0;
                    lPage.Children.Add(lHeaderTextBorder0);
                    lHeaderTextBorder0.SetValue(Grid.RowProperty, lCount);
                    lHeaderTextBorder0.SetValue(Grid.ColumnProperty, lColIndex);
                    lColIndex++;
                }

                if (lCount == 28)
                {
                    lCount = 0;
                }
            }
        }
    }

    public class ReportParame
    {
        public String ReportTitle { get; set; }
        public int FieldsCount { get; set; }
        public List<String> Title { get; set; }
        public List<int> TitleWidth { get; set; }
        public List<List<String>> Values { get; set; }
    }
}

