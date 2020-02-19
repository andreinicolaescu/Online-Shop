using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProiectDaw.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Descrierea categoriei este obligatorie")]
        public string CategoryDescription { get; set; }

        [NotMapped]
        public HttpPostedFileBase Image { get; set; }

        public string ImagePath { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}