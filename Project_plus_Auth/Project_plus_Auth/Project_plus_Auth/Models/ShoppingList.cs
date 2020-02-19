using ProiectDaw.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_plus_Auth.Models
{
    public class ShoppingList
    {
        [Key]
        public string ShoppingListId { get; set;}

        public virtual List <Product> List { get; set; }

        
    }
}