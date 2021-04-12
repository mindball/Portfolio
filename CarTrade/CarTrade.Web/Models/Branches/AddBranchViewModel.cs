using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Models.Branches
{
    public class AddBranchViewModel
    {
        [Required]
        public string Town { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
