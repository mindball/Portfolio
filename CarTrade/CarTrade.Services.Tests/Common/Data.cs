using CarTrade.Services.Tests.Enums;
using CarTrade.Services.Vehicles.Models;
using System;
using System.Collections.Generic;

namespace CarTrade.Services.Tests.Common
{
    public abstract class Data
    {
        public static IEnumerable<object[]> ExpireDates => new[]
        {
            new object[] { DateTime.UtcNow.AddYears(-(int)TimesPeriod.YearDays)},
            new object[] { DateTime.UtcNow.AddMonths(-(int)TimesPeriod.MonthlyDays) },
            new object[] { DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly)},
            new object[] { DateTime.UtcNow.AddHours(-(int)TimesPeriod.Hour)},
            new object[] { DateTime.UtcNow.AddMinutes(-(int)TimesPeriod.Minute),},
            new object[] { DateTime.UtcNow.AddSeconds(-(int)TimesPeriod.Second)},
            new object[] { DateTime.UtcNow.AddMilliseconds(-(int)TimesPeriod.Milisecond)}
        };

        public static IEnumerable<object[]> StartDateBiggerThanEndDateData => new[]
        {
            new object[] {DateTime.UtcNow, DateTime.UtcNow.AddYears(-(int)TimesPeriod.YearDays)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMonths(-(int)TimesPeriod.MonthlyDays) },
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddHours(-(int)TimesPeriod.Hour)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMinutes(-(int)TimesPeriod.Minute),},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddSeconds(-(int)TimesPeriod.Second)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMilliseconds(-(int)TimesPeriod.Milisecond)}
        };
                
        public static IEnumerable<object[]> ValidateVehicleFormServiceModel => new[]
       { 
            //Exist plate number
            new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = "CT1234BM",
                        Vin = "VWVZZZ1KZ1P324444",
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = 1,
                }},
            new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = null,
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = 1,
                }},
            //Exist vin         
            new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = "VWVZZZ1KZ1P324444",
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = 1,
                }},
            new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = null,
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = 1,
                }},
            //not exist branch
             new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 0,
                        BrandId = 1,
                        OwnerId = 1,
                }},
              //not exist branch
             new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = int.MaxValue,
                        BrandId = 1,
                        OwnerId = 1,
                }},
             //not exist brand
              new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 1,
                        BrandId = 0,
                        OwnerId = 1,
                }},
              //not exist branch
              new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 1,
                        BrandId =int.MaxValue,
                        OwnerId = 1,
                }},
              //not exist owner
              new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = 0,
                }},
              new object[] {
                new VehicleFormServiceModel() {
                        PlateNumber = Guid.NewGuid().ToString(),
                        Vin = Guid.NewGuid().ToString(),
                        BranchId = 1,
                        BrandId = 1,
                        OwnerId = int.MaxValue,
                }}
        };
    }
}
