using ProiectDaw.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_plus_Auth.Models
{
    public class Comment
    {

        [Key]
        public int CommentId { get; set; }

        public int CommentProductId { get; set; }

        [Required(ErrorMessage = "Textul este obligatoriu")]
        public string CommentTxt { get; set; }

    }
}