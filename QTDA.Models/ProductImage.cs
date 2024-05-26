using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTDA.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
 		[ValidateNever]
		public string ImageUrl { get; set; }    
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [NotMapped]
		[ValidateNever]
		public string productTitle { get; set; }
    }
}
