using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Models
{
    public abstract class BasicDetailViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
