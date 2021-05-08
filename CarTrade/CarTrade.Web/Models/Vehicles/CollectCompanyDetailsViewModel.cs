using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.Models.Vehicles
{
    public class CollectCompanyDetailsViewModel
    {
        public IEnumerable<SelectListItem> Branches { get; set; }

        public IEnumerable<SelectListItem> Brands { get; set; }

        public IEnumerable<SelectListItem> Employers { get; set; }
    }
}
