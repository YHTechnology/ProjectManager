using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Windows.Data.DomainServices;

namespace ProductManager.ViewData.Entity
{
    public class ProductEntity : NotifyPropertyChanged
    {
        private String productID;

        [Required(ErrorMessage = "序列号必填")]
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
                    RemoveError("ProductID");
                    CheckProductID();
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
                    RemoveError("ManufactureNumber");
                    CheckManufactureNumber();
                }
            }
        }

        private String projectName;
        public String ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                if (projectName != value)
                {
                    projectName = value;
                    UpdateChanged("ProjectName");
                }
            }
        }


        private Nullable<DateTime> productIDCreateData;
        public Nullable<DateTime> ProductIDCreateData
        {
            get
            {
                return productIDCreateData;
            }
            set
            {
                if (productIDCreateData != value)
                {
                    productIDCreateData = value;
                    UpdateChanged("ProductIDCreateData");
                    UpdateChanged("ProductIDCreateDataString");
                }
            }
        }

        public String ProductIDCreateDataString
        {
            get
            {
                if (productIDCreateData.HasValue)
                {
                    return productIDCreateData.Value.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

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

        private String productTypeString;
        public String ProductTypeString
        {
            get
            {
                return productTypeString;
            }
            set
            {
                if (productTypeString != value)
                {
                    productTypeString = value;
                    UpdateChanged("ProductTypeString");
                }
            }
        }

        private String productDescript1;
        public String ProductDescript1
        {
            get
            {
                return productDescript1;
            }
            set
            {
                if (productDescript1 != value)
                {
                    productDescript1 = value;
                    UpdateChanged("ProductDescript1");
                }
            }
        }

        private String productDescript2;
        public String ProductDescript2
        {
            get
            {
                return productDescript2;
            }
            set
            {
                if (productDescript2 != value)
                {
                    productDescript2 = value;
                    UpdateChanged("ProductDescript2");
                }
            }
        }

        private DateTime productAriveTime;
        public DateTime ProductAriveTime
        {
            get
            {
                return productAriveTime;
            }
            set
            {
                if (productAriveTime != value)
                {
                    productAriveTime = value;
                    UpdateChanged("ProductAriveTime");
                    UpdateChanged("ProductAriveTimeString");
                }
            }
        }

        public String ProductAriveTimeString
        {
            get
            {
                if (productAriveTime != DateTime.MinValue)
                {
                    return productAriveTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private DateTime productCompleteTime;
        public DateTime ProductCompleteTime
        {
            get
            {
                return productCompleteTime;
            }
            set
            {
                if (productCompleteTime != value)
                {
                    productCompleteTime = value;
                    UpdateChanged("ProductCompleteTime");
                    UpdateChanged("ProductCompleteTimeString");
                }
            }
        }

        public String ProductCompleteTimeString
        {
            get
            {
                if (productCompleteTime != DateTime.MinValue)
                {
                    return productCompleteTime.ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        private String productPartTimeInfo;
        public String ProductPartTimeInfo
        {
            get
            {
                return productPartTimeInfo;
            }
            set
            {
                if (productPartTimeInfo != value)
                {
                    productPartTimeInfo = value;
                    UpdateChanged("ProductPartTimeInfo");
                }
            }
        }

        private String productOutputNumber;
        public String ProductOutputNumber
        {
            get
            {
                return productOutputNumber;
            }
            set
            {
                if (productOutputNumber != value)
                {
                    productOutputNumber = value;
                    UpdateChanged("ProductOutputNumber");
                }
            }
        }

        public void Update()
        {
            this.productID = Product.product_id;
            this.manufactureNumber = Product.manufacture_number;
            this.productName = Product.product_name;
            this.productIDCreateData = Product.product_id_createdate;
            this.productTypeID = Product.product_type_id.GetValueOrDefault(-1);
            this.productDescript1 = Product.product_descript1;
            this.productDescript2 = Product.product_descript2;
            this.productAriveTime = Product.product_arive_time.GetValueOrDefault();
            this.productCompleteTime = Product.product_complete_time.GetValueOrDefault();
            this.productOutputNumber = Product.product_outputnumber;
            using (productPartTimeView.DeferRefresh())
            {
                productPartTimeView.MoveToFirstPage();
            }
        }

        public void DUpdate()
        {
            Product.product_id = this.productID;
            Product.manufacture_number = this.manufactureNumber;
            Product.product_name = this.productName;
            Product.product_id_createdate = this.productIDCreateData;
            Product.product_type_id = this.productTypeID;
            Product.product_descript1 = this.productDescript1;
            Product.product_descript2 = this.productDescript2;
            Product.product_arive_time = this.productAriveTime;
            Product.product_complete_time = this.productCompleteTime;
            Product.product_outputnumber = this.productOutputNumber;
        }

        public void RaisALL()
        {
            UpdateChanged("ProductID");
            UpdateChanged("ProductName");
            UpdateChanged("ManufactureNumber");
            UpdateChanged("ProductIDCreateData");
            UpdateChanged("ProductIDCreateDataString");
            UpdateChanged("ProductTypeID");
            UpdateChanged("ProductTypeString");
            UpdateChanged("ProductDescript1");
            UpdateChanged("ProductDescript2");
            UpdateChanged("ProductAriveTime");
            UpdateChanged("ProductAriveTimeString");
            UpdateChanged("ProductCompleteTime");
            UpdateChanged("ProductCompleteTimeString");
        }

        public ProductManager.Web.Model.product Product { get; set; }
        public Dictionary<String, ProductEntity> ProductEntityDictionary { get; set; }
        public Dictionary<String, ProjectEntity> ProjectEntityDictionary { get; set; }

        private void CheckProductID()
        {
            if (string.IsNullOrWhiteSpace(this.productID)
                || string.IsNullOrWhiteSpace(this.ProductID))
            {
                return;
            }

            ProductEntity lProductEntity;
            if (ProductEntityDictionary.TryGetValue(this.ProductID, out lProductEntity))
            {
                this.AddError("ProductID", "序列号不能重复");
            }
            else
            {
                this.RemoveError("ProductID");
            }
        }

        private void CheckManufactureNumber()
        {
            if (string.IsNullOrWhiteSpace(this.manufactureNumber)
                || string.IsNullOrWhiteSpace(this.ManufactureNumber))
            {
                return;
            }

            ProjectEntity lProjectEntity;
            if (!ProjectEntityDictionary.TryGetValue(this.ManufactureNumber, out lProjectEntity))
            {
                this.AddError("ManufactureNumber", "生产令号不在库中");
            }
            else
            {
                this.RemoveError("ManufactureNumber");
            }
        }

        public Dictionary<int, ProductPartTypeEntity> ProductPartTypeEntityDictionary { get; set; }

        private ProductManager.Web.Service.ProductDomainContext ProductDomainContext = new ProductManager.Web.Service.ProductDomainContext();

        private DomainCollectionView<ProductManager.Web.Model.product_part_time> productPartTimeView;
        private DomainCollectionViewLoader<ProductManager.Web.Model.product_part_time> productPartTimeLoader;
        private EntityList<ProductManager.Web.Model.product_part_time> productPartTimeSource;

        public ProductEntity()
        {
            this.productPartTimeSource = new EntityList<ProductManager.Web.Model.product_part_time>(this.ProductDomainContext.product_part_times);
            this.productPartTimeLoader = new DomainCollectionViewLoader<ProductManager.Web.Model.product_part_time>(
                this.LoadProductPartTime,
                this.LoadOperationProductPartTimeCompleted
                );
            this.productPartTimeView = new DomainCollectionView<ProductManager.Web.Model.product_part_time>(this.productPartTimeLoader, this.productPartTimeSource);
        }

        private LoadOperation<ProductManager.Web.Model.product_part_time> LoadProductPartTime()
        {
            EntityQuery<ProductManager.Web.Model.product_part_time> lQuery = this.ProductDomainContext.GetProduct_part_timeQuery();
            lQuery = lQuery.Where(e => e.product_id == productID);
            return this.ProductDomainContext.Load(lQuery.SortAndPageBy(this.productPartTimeView));
        }

        private void LoadOperationProductPartTimeCompleted(LoadOperation<ProductManager.Web.Model.product_part_time> aLoadOperation)
        {
            if (ProductPartTypeEntityDictionary != null)
            {
                String lProductPartTimeInfo = "";
                foreach (ProductManager.Web.Model.product_part_time product_part_time in aLoadOperation.Entities)
                {
                    String lProductPartTimeInfoTemp = "";
                    ProductPartTimeEntity productPartTimeEntity = new ProductPartTimeEntity();
                    productPartTimeEntity.ProductPartTime = product_part_time;
                    productPartTimeEntity.Update();
                    ProductPartTypeEntity lproductPartTypeEntity;
                    if (ProductPartTypeEntityDictionary.TryGetValue(productPartTimeEntity.ProductPartID, out lproductPartTypeEntity))
                    {
                        lProductPartTimeInfoTemp += "[阶段：" + lproductPartTypeEntity.ProductPartTypeName + "(" + productPartTimeEntity.StartTimeString + "-" + productPartTimeEntity.FinishTimeString + ")]";
                        lProductPartTimeInfo += lProductPartTimeInfoTemp;
                    }
                }
                ProductPartTimeInfo = lProductPartTimeInfo;
            }
        }
    }
}
