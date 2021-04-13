using CarTrade.Common.Mapping;
using CarTrade.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Services.Insurance.Models
{
    public class InsuranceListingServiceModel : IMapFrom<Data.Models.Insurance>
    {
        public string CompanyName { get; set; }

        
        public TypeInsurance TypeInsurance { get; set; }

        
        public DateTime StartDate { get; set; }

        
        public DateTime EndDate { get; set; }
    }
}
