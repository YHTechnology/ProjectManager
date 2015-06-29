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
    public class ProjectConfigEntity : NotifyPropertyChanged
    {
        /*
        private int productID;
        public int ProductID
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

        private String productNumber;
        private String ProductNumber
        {
            get
            {
                return productNumber;
            }
            set
            {
                if (productNumber != value)
                {
                    productNumber = value;
                    UpdateChanged("ProductNumber");
                }
            }

        }

        private String manufactureNumber;
        public String ManufactureNumber
        {
            get
            {
                return manufactureNumber;
            }
            set
            {
                if (manufactureNumber != value)
                {
                    manufactureNumber = value;
                    UpdateChanged("ManufactureNumber");
                }
            }
        }

        private String productName;
        public String ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                if (productName != value)
                {
                    productName = value;
                    UpdateChanged("ProductName");
                }
            }
        }

        private String productDescribe;
        public String ProductDescribe
        {
            get
            {
                return productDescribe;
            }
            set
            {
                if (productDescribe != value)
                {
                    productDescribe = value;
                    UpdateChanged("ProductDescribe");
                }
            }
        }

        private String productMainConfig;
        public String ProductMainConfig
        {
            get
            {
                return productMainConfig;
            }
            set
            {
                if (productMainConfig != value)
                {
                    productMainConfig = value;
                    UpdateChanged("ProductMainConfig");
                }
            }
        }

        private String productTechRequire;
        public String ProductTechRequire
        {
            get
            {
                return productTechRequire;
            }
            set
            {
                if (productTechRequire != value)
                {
                    productTechRequire = value;
                    UpdateChanged("ProductTechRequire");
                }
            }
        }

        private int productCount;
        public int ProductCount
        {
            get
            {
                return productCount;
            }
            set
            {
                if (productCount != value)
                {
                    productCount = value;
                    UpdateChanged("ProductCount");
                }
            }
        }

        private String productMadeIn;
        public String ProductMadeIn
        {
            get
            {
                return productMadeIn;
            }
            set
            {
                if (productMadeIn != value)
                {
                    productMadeIn = value;
                    UpdateChanged("ProductMadeIn");
                }
            }
        }

        public void Update()
        {
            this.productID = Product.product_id;
            this.productNumber = Product.product_number;
            this.manufactureNumber = Product.manufacture_number;
            this.productName = Product.product_name;
            this.productDescribe = Product.product_describe;
            this.productMainConfig = Product.product_mainconfig;
            this.productTechRequire = Product.product_techrequire;
            this.productCount = Product.product_count.GetValueOrDefault(0);
            this.productMadeIn = Product.product_madein;
        }

        public void DUpate()
        {
            Product.product_id = this.productID;
            Product.product_number = this.productNumber;
            Product.manufacture_number = this.manufactureNumber;
            Product.product_name = this.productName;
            Product.product_describe = this.productDescribe;
            Product.product_mainconfig = this.productMainConfig;
            Product.product_techrequire = this.productTechRequire;
            Product.product_count = this.productCount;
            this.productMadeIn = Product.product_madein;
        }

        public void RaisALL()
        {
            UpdateChanged("ProductID");
            UpdateChanged("ProductNumber");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ProductName");
            UpdateChanged("ProductDescribe");
            UpdateChanged("ProductMainConfig");
            UpdateChanged("ProductTechRequire");
            UpdateChanged("ProductCount");
            UpdateChanged("ProductMadeIn");
        }

        public ProductManager.Web.Model.product Product { get; set; }
        */
    }
}
