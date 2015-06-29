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
using ProductManager.ViewModel.PlanManager;
using System.Collections.ObjectModel;
using ProductManager.Controls;
using ProductManager.ViewData.Entity;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListStatisticsWindow : ChildWindow
    {
        public PlanListViewModel planListViewModel;
        public PlanExtraEntity planExtraEntity;

        public PlanListStatisticsWindow(string aTitle, PlanListViewModel aPlanListViewModel, PlanExtraEntity aPlanExtraEntity)
        {
            InitializeComponent();
            this.Title = aTitle;
            this.planListViewModel = aPlanListViewModel;
            this.planExtraEntity = aPlanExtraEntity;

            if (null == aPlanExtraEntity)
            {
                this.planListTabControl.Margin = new Thickness(2, 2, 2, 40);
                this.planListTabControl.SetValue(Canvas.ZIndexProperty, 3);
                this.planExtraGrid.SetValue(Canvas.ZIndexProperty, 1);
            }
            else
            {
                this.planExtraGrid.DataContext = planExtraEntity;
            }

            AddTabItem(planListViewModel.Title, planListViewModel);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void AddTabItem(string header, PlanListViewModel aPlanListViewModel)
        {
            if (string.IsNullOrEmpty(header) || null == aPlanListViewModel)
            {
                return;
            }

            var list = this.planListTabControl.Items.Where(w => ((TabItem)w).Header.ToString() == header);
            if (list.Count() > 0)
            {
                ((TabItem)list.First()).Visibility = Visibility.Visible;

                this.planListTabControl.SelectedItem = list.First();
            }
            else
            {
                TabItem tabItem = new TabItem();

                tabItem.Header = header;

                PlanListStatisticsDataGrid planListDataGrid = new PlanListStatisticsDataGrid(aPlanListViewModel, null != planExtraEntity);

                var tabContent = planListDataGrid as UserControl;

                tabItem.Content = tabContent;
                this.planListTabControl.Items.Add(tabItem);
                this.planListTabControl.SelectedItem = tabItem;
            }
        }
    }
}

