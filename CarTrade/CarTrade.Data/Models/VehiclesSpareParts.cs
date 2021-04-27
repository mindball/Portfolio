using System;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Data.Models
{
    public class VehiclesSpareParts
    {

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }

        public int SparePartId { get; set; }

        public virtual SparePart SpareParts { get; set; }
    }
}
