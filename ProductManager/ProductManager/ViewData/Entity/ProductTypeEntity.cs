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
    public class ProductTypeEntity : NotifyPropertyChanged
    {
        private int productTypeID;
        public int ProductTypeID
        {
            get
            {
                return productTypeID;
            }
            set
            {
                if (productTypeID != value)
                {
                    productTypeID = value;
                    UpdateChanged("ProductTypeID");
                }
            }
        }

        private String productTypeName;
        public String ProductTypeName
        {
            get
            {
                return productTypeName;
            }
            set
            {
                if (productTypeName != value)
                {
                    productTypeName = value;
                    UpdateChanged("ProductTypeName");
                }
            }
        }

        public void Update()
        {
            this.productTypeID = ProductType.product_type_id;
            this.productTypeName = ProductType.product_type_name;
        }

        public void DUpdate()
        {
            ProductType.product_type_id = this.productTypeID;
            ProductType.product_type_name = this.productTypeName;
        }

        public void RaisALL()
        {
            UpdateChanged("ProductTypeID");
            UpdateChanged("ProductTypeName");
        }

        public ProductManager.Web.Model.product_type ProductType { get; set; }
    }
}
