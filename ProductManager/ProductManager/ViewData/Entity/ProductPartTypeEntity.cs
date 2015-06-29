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
    public class ProductPartTypeEntity : NotifyPropertyChanged
    {
        private int productPartTypeID;
        public int ProductPartTypeID
        {
            get
            {
                return productPartTypeID;
            }
            set
            {
                if (productPartTypeID != value)
                {
                    productPartTypeID = value;
                    UpdateChanged("ProductPartTypeID");
                }
            }
        }

        private String productPartTypeName;
        public String ProductPartTypeName
        {
            get
            {
                return productPartTypeName;
            }
            set
            {
                if (productPartTypeName != value)
                {
                    productPartTypeName = value;
                    UpdateChanged("ProductPartTypeName");
                }
            }
        }

        public ProductManager.Web.Model.product_part_type ProductPartType { get; set; }

        public void Update()
        {
            this.productPartTypeID = ProductPartType.product_part_type_id;
            this.productPartTypeName = ProductPartType.product_part_type_name;
        }

        public void DUpdate()
        {
            ProductPartType.product_part_type_id = this.productPartTypeID;
            ProductPartType.product_part_type_name = this.productPartTypeName;
        }

        public void RaisALL()
        {
            UpdateChanged("ProductPartTypeID");
            UpdateChanged("ProductPartTypeName");
        }
    }
}
