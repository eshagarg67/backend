using System;
using System.Collections.Generic;

namespace Training_Project.Models
{
    public partial class Users
    {
        public Users()
        {
            CategoriesCreatedByNavigation = new HashSet<Categories>();
            CategoriesModifiedByNavigation = new HashSet<Categories>();
            ProductsCreatedByNavigation = new HashSet<Products>();
            ProductsModifiedByNavigation = new HashSet<Products>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string UserImage { get; set; }
        public bool? IsAdmin { get; set; }

        public ICollection<Categories> CategoriesCreatedByNavigation { get; set; }
        public ICollection<Categories> CategoriesModifiedByNavigation { get; set; }
        public ICollection<Products> ProductsCreatedByNavigation { get; set; }
        public ICollection<Products> ProductsModifiedByNavigation { get; set; }
    }
}
