using ProiectDaw.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project_plus_Auth.Models
{
    public class StarRating
    {
       [Key]
       public int RateId { get; set; } 

       public int Rate { get; set; }

       public int ProductId { get; set; }

       public string UserId { get; set; }
    }
}