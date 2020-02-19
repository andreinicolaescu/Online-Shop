using Project_plus_Auth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectDaw.Models
{
    public class Product
    {       

        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Numele produslui este obligatoriu")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Pretul produslui este obligatoriu")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "Descrierea produslui este obligatorie")]
        public string ProductDescription { get; set; }

        //public int ProductRating { get; set; }

        public bool ProductAprove { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public HttpPostedFileBase Image { get; set; }

        public string ImagePath { get; set; }

        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public List<string> Comments { get; set; }

        //rating

        [StringLength(5)]
        public string Rating { get; set; }

        //

        public int orderQuantity { get; set; }

        public class ProductDBContext : DbContext
        {
            public ProductDBContext() : base("DBConnectionString") { }
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<ShoppingList> ShoppingLists { get; set; }
            public DbSet<Comment> CommentLists { get; set; }
            public DbSet<StarRating> Ratings { get; set; }
        }

    }
}