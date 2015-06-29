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

namespace ProductManager.ViewData.Entity
{
    public class ProductPartTimeEntity : NotifyPropertyChanged
    {
        private int productPartTimeID;
        public int ProductPartTimeID
        {
            get
            {
                return productPartTimeID;
            }
            set
            {
                if (productPartTimeID != value)
                {
                    productPartTimeID = value;
                    UpdateChanged("ProductPartTimeID");
                }
            }
        }

        private String productID;
        public String ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                if (productID != value)
                {
                    productID = value;
                    UpdateChanged("ProductID");
                }
            }
        }

        private int productPartID;
        public int ProductPartID
        {
            get
            {
                return productPartID;
            }
            set
            {
                if (productPartID != value)
                {
                    productPartID = value;
                    UpdateChanged("ProductPartID");
                }
            }
        }

        private String productPartName;
        public String ProductPartName
        {
            get
            {
                return productPartName;
            }
            set
            {
                if (productPartName != value)
                {
                    productPartName = value;
                    UpdateChanged("ProductPartName");
                }
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    UpdateChanged("StartTime");
                }
            }
        }

        public String StartTimeString
        {
            get
            {
                if (startTime != DateTime.MinValue)
                {
                    return startTime.ToShortDateString();
                } 
                else
                {
                    return "";
                }
            }
        }

        private DateTime finishTime;
        public DateTime FinishTime
        {
            get
            {
                return finishTime;
            }
            set
            {
                if (finishTime != value)
                {
                    finishTime = value;
                    UpdateChanged("FinishTime");
                }
            }
        }

        public String FinishTimeString
        {
            get
            {
                if (finishTime != DateTime.MinValue)
                {
                    return finishTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String descript1;
        public String Descript1
        {
            get
            {
                return descript1;
            }
            set
            {
                if (descript1 != value)
                {
                    descript1 = value;
                    UpdateChanged("Descript1");
                }
            }
        }

        private String descript2;
        public String Descript2
        {
            get
            {
                return descript2;
            }
            set
            {
                if (descript2 != value)
                {
                    descript2 = value;
                    UpdateChanged("Descript2");
                }
            }
        }

        public ProductManager.Web.Model.product_part_time ProductPartTime { get; set; }

        public void Update()
        {
            this.productPartTimeID = ProductPartTime.product_part_time_id;
            this.productPartID = ProductPartTime.product_part_id.GetValueOrDefault(-1);
            this.productID = ProductPartTime.product_id;
            this.startTime = ProductPartTime.start_time.GetValueOrDefault();
            this.finishTime = ProductPartTime.finish_time.GetValueOrDefault();
            this.descript1 = ProductPartTime.descript1;
            this.descript2 = ProductPartTime.descript2;

        }

        public void DUpdate()
        {
            //ProductPartTime.product_part_time_id = this.productPartTimeID;
            ProductPartTime.product_part_id = this.productPartID;
            ProductPartTime.product_id = this.productID;
            ProductPartTime.start_time = this.startTime;
            ProductPartTime.finish_time = this.finishTime;
            ProductPartTime.descript1 = this.descript1;
            ProductPartTime.descript2 = this.descript2;
        }

        public void RaisALL()
        {
            UpdateChanged("productPartTimeID");
            UpdateChanged("productPartID");
            UpdateChanged("ProductPartName");
            UpdateChanged("ProductID");
            UpdateChanged("StartTime");
            UpdateChanged("StartTimeString");
            UpdateChanged("FinishTime");
            UpdateChanged("FinishTimeString");
            UpdateChanged("Descript1");
            UpdateChanged("Descript2");
        }
    }
}
