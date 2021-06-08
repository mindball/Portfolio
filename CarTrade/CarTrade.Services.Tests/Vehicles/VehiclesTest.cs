using CarTrade.Services.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using static CarTrade.Common.DataConstants;

namespace CarTrade.Services.Tests.Vehicles
{
    [Collection("Database collection")]
    public class VehiclesTest
    {
        DatabaseFixture fixture;
        private IVehicleService vehicleService;

        public VehiclesTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.vehicleService = new VehicleService(this.fixture.Context, this.fixture.Mapper);
        }


    }
}
