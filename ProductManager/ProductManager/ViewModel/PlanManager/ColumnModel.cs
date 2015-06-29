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
using System.Collections.ObjectModel;

namespace ProductManager.ViewModel.PlanManager
{
    public class ColumnModel
    {
        public ColumnModel()
        {
            {
                //model0
                ObservableCollection<string> columnModel = new ObservableCollection<string>();
                columnModel.Add("序号");
                columnModel.Add("部件");
                columnModel.Add("任务描述");
                columnModel.Add("权重(分值)");
                columnModel.Add("实际得分");
                columnModel.Add("计划完成时间");
                columnModel.Add("第一次调整");
                columnModel.Add("第二次调整");
                columnModel.Add("实际完成时间");
                columnModel.Add("责任单位");
                columnModel.Add("备注");
                columnModelCollectionList.Add(columnModel);
                if (columnModel.Count < minSize)
                {
                    minSize = columnModel.Count;
                }
            }

            {
                //model1
                ObservableCollection<string> columnModel = new ObservableCollection<string>();
                columnModel.Add("序号");
                columnModel.Add("部件");
                columnModel.Add("任务描述");
                columnModel.Add("权重(分值)");
                columnModel.Add("实际得分");
                columnModel.Add("计划下订单时间");
                columnModel.Add("计划到货时间");
                columnModel.Add("第一次调整");
                columnModel.Add("第二次调整");
                columnModel.Add("实际到货时间");
                columnModel.Add("责任单位");
                columnModel.Add("备注");
                columnModelCollectionList.Add(columnModel);
                if (columnModel.Count < minSize)
                {
                    minSize = columnModel.Count;
                }
            }

            {
                //model2
                ObservableCollection<string> columnModel = new ObservableCollection<string>();
                columnModel.Add("序号");
                columnModel.Add("部件");
                columnModel.Add("任务描述");
                columnModel.Add("权重(分值)");
                columnModel.Add("实际得分");
                columnModel.Add("计划到货时间");
                columnModel.Add("第一次调整");
                columnModel.Add("第二次调整");
                columnModel.Add("实际到货时间");
                columnModel.Add("责任单位");
                columnModel.Add("备注");
                columnModelCollectionList.Add(columnModel);
                if (columnModel.Count < minSize)
                {
                    minSize = columnModel.Count;
                }
            }
        }

        private int minSize = int.MaxValue;
        public int MinSize
        {
            get
            {
                return minSize;
            }
        }

        private ObservableCollection<ObservableCollection<string>> columnModelCollectionList = new ObservableCollection<ObservableCollection<string>>();
        public ObservableCollection<ObservableCollection<string>> List
        {
            get
            {
                return columnModelCollectionList;
            }
        }
    }
}
