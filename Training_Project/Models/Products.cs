using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Training_Project.Models
{
    public partial class Products
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public int Price { get; set; }
        public int QuantityInStock { get; set; }
        public string ModelNumber { get; set; }
        public bool IsDiscounted { get; set; }
        public int? DiscountPercent { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        internal Categories Category { get; set; }
        internal Users CreatedByNavigation { get; set; }
        internal Users ModifiedByNavigation { get; set; }
    }


    public class ProductListInfoByCategory
    {
        public List<Products> products { get; set; }
        public string categoryName { get; set; }
        public int categoryId { get; set; }
    }

    public class ProductDetail: Products
    {

        public string categoryName { get; set; }
        public ProductDetail(Products prd)
        {
            foreach (PropertyInfo prop in prd.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(prd, null), null);
        }
    }
}
