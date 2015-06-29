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
using System.Collections.ObjectModel;

using ProductManager.ViewModel.PlanManager;

namespace ProductManager.Views.PlanManager
{
    public partial class PlanListEvaluateResultChart : UserControl
    {
        public PlanListEvaluateResultChart(Dictionary<string, int> aPlanEvaluateResultDictionary)
        {
            InitializeComponent();
            this.EvaluateResult.DataContext = aPlanEvaluateResultDictionary;
        }
    }
}
