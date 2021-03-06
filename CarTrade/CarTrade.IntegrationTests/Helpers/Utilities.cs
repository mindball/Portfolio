using CarTrade.Data;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarTrade.Web.Infrastructure.Mapping;
using CarTrade.Data.Models;
using CarTrade.Data.Enums;
using CarTrade.Services.Tests.Enums;
using System.Threading.Tasks;

namespace CarTrade.IntegrationTests.Helpers
{
    public static class Utilities
    {
        public static async Task InitializeDbForTests(CarDbContext db)
        {
            AutoMapper.Mapper.Initialize(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            await FillBranches(db);
            await FillBrands(db);
            await FillEmployeers(db);
            await FillInsuranceCompany(db);
            await FillVehicles(db);
            await FillInsurancePolicies(db);
            await FillVignettes(db);
        }


        public static async Task ReinitializeDbForTests(CarDbContext db)
        {
            //db.Messages.RemoveRange(db.Messages);
            await InitializeDbForTests(db);
        }

        private static async Task FillBranches(CarDbContext context)
        {
            var firstBranch = new Branch { Id = 1, Town = "Стара Загора", Address = "бул. Никола Петков 55" };
            var secondBranch = new Branch { Id = 2, Town = "София", Address = "бул. Христо Ботев 98" };
            var thirdBranch = new Branch { Id = 3, Town = "Бургас", Address = "ж.к. Славейков, бл. 165" };
            var fourthBranch = new Branch { Id = 4, Town = "Пловдив", Address = "бул. Кукленско шосе № 3 А" };
            var fifttBranch = new Branch { Id = 5, Town = "Русе", Address = "ул. Потсдам 2" };

            await context.Branches.AddRangeAsync(firstBranch, secondBranch,
                thirdBranch, fourthBranch, fifttBranch);

            await context.SaveChangesAsync();
        }

        private static async Task FillBrands(CarDbContext context)
        {
            var firstBrand = new Brand { Id = 1, Name = "Volkswagen" };
            var secondBrand = new Brand { Id = 2, Name = "Audi" };

            await context.Brands.AddRangeAsync(firstBrand, secondBrand);

            await context.SaveChangesAsync();
        }

        private static async Task FillEmployeers(CarDbContext context)
        {
            var firstEmployeer = new Employer { Id = 1, Name = "Автохит Трейд ООД" };
            var secondEmployeer = new Employer { Id = 2, Name = "Автохит 2000" };

            await context.Companies.AddRangeAsync(firstEmployeer, secondEmployeer);

            await context.SaveChangesAsync();
        }

        private static async Task FillInsuranceCompany(CarDbContext context)
        {
            List<InsuranceCompany> insuranceCompanies = new List<InsuranceCompany>()
            {
                new InsuranceCompany { Id = 1, Name = "ЗД Бул инс АД" },
                new InsuranceCompany { Id = 2, Name = "ЗК Български имоти АД" },
                new InsuranceCompany { Id = 3, Name = "ЗК Уника АД" },
                new InsuranceCompany { Id = 4, Name = "ЗК Лев инс АД" },
                new InsuranceCompany { Id = 5, Name = "Дженерали застраховане АД" }
            };

            await context.InsuranceCompanies.AddRangeAsync(insuranceCompanies);
            await context.SaveChangesAsync();
        }

        private static async Task FillVehicles(CarDbContext context)
        {
            List<Vehicle> vehicles = new List<Vehicle>()
            {
                //Active
                //Vignette
                //Insurance expire
                //Inspection
                //OilChange after 1000 km
                new Vehicle
                {
                    Id = 1,
                    Model = "Up",
                    PlateNumber = "CT1234BM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 112000,
                    EndOilChange = 113000,
                    Vin = "VWVZZZ1KZ1P324444",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                },
                //Active
                //Vignette
                //Insurance expire
                //Inspection
                //OilChange expire 999
                new Vehicle
                {
                    Id = 2,
                    Model = "Up",
                    PlateNumber = "CTTTTTBM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(6),
                    TravelledDistance = 180000,
                    EndOilChange = 180999,
                    Vin = "VWVZZZ1KZ1P111222",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.QuarterOfYearDays),
                    BranchId = 2,
                    BrandId = 1,
                    OwnerId = 1,
                },                
                //Vignette
                //Insurance expire
                //Inspection expire monthly
                //OilChange
                new Vehicle
                {
                    Id = 3,
                    Model = "Up",
                    PlateNumber = "CT7777AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(3),
                    TravelledDistance = 18000,
                    EndOilChange = 28000,
                    Vin = "VWVZZZ1KZ1P999888",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.MonthlyDays - 1),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //Vignette
                //InspectionSafetyCheck expire today
                //Insurance                
                //OilChange
                new Vehicle
                {
                    Id = 4,
                    Model = "Up",
                    PlateNumber = "CT9876AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(8),
                    TravelledDistance = 198000,
                    EndOilChange = 208000,
                    Vin = "VWVZZZ1KZ1P333333",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow,
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                },
               //Vignette
                //InspectionSafetyCheck expired
                //Insurance expire
                //OilChange
                new Vehicle
                {
                    Id = 5,
                    Model = "Up",
                    PlateNumber = "CT9876AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(8),
                    TravelledDistance = 198000,
                    EndOilChange = 200000,
                    Vin = "VWVZZZ1KZ1P333333",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1
                },
                 //Vignette
                //InspectionSafetyCheck expired
                //Insurance                
                //OilChange 100 km to oil change                
                new Vehicle
                {
                    Id = 6,
                    Model = "TT",
                    PlateNumber = "CT9876AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 198100,
                    Vin = "VWVZZZ1KZ1P98744",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly),
                    BranchId = 4,
                    BrandId = 2,
                    OwnerId = 2,
                },                
                 //Vignette expire
                //InspectionSafetyCheck expiring
                //Insurance                
                //OilChange expire
                new Vehicle
                {
                    Id = 7,
                    Model = "Up",
                    PlateNumber = "CT1111AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(7),
                    TravelledDistance = 198000,
                    EndOilChange = 198000,
                    Vin = "VWVZZZ1KZ1P447788",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.WeeklyDays),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                },                
                //Vignette expire
                //InspectionSafetyCheck
                //Insurance expire
                //OilChange expire 10 km to oil change
                new Vehicle
                {
                    Id = 8,
                    Model = "Up",
                    PlateNumber = "CT5555AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(2),
                    TravelledDistance = 198000,
                    EndOilChange = 198010,
                    Vin = "VWVZZZ1KZ1P357159",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                },                
                //Vignette expire
                //InspectionSafetyCheck
                //Insurance                
                //OilChange exceed expire TravelledDistance
                new Vehicle
                {
                    Id = 9,
                    Model = "Up",
                    PlateNumber = "CT75665AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 197000,
                    Vin = "VWVZZZ1KZ1P357159",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                },                
                //Vignette expire
                //InspectionSafetyCheck expiring
                //Insurance expire   
                //OilChange expire
                new Vehicle
                {
                    Id = 10,
                    Model = "Up",
                    PlateNumber = "CT75665AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 198000,
                    Vin = "VWVZZZ1KZ1P357159",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.MonthlyDays - 1),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                }, 
                //Vignette  never use vignette
                //InspectionSafetyCheck  never use inspection
                //Insurance never use insurance
                //OilChange never use
                new Vehicle
                {
                    Id = 11,
                    Model = "Passat",
                    PlateNumber = "CT72235AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 0,
                    EndOilChange = 999999,
                    Vin = "WVZWWWAAAAAWASDAS22",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.MaxValue,
                    BranchId = 5,
                    BrandId = 1,
                    OwnerId = 2,
                },
                //vignette expire after 30 days
                //Insurance
                //Inspection
                //OilChange
                new Vehicle
                {
                    Id = 12,
                    Model = "TT",
                    PlateNumber = "CT0987AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 208000,
                    Vin = "WZWZASAAASDASDASDASDA",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.Dayly),
                    BranchId = 5,
                    BrandId = 2,
                    OwnerId = 2,
                },                
                //Vignette expire after 20 days
                //Insurance
                //Inspection
                //OilChange
                new Vehicle
                {
                    Id = 13,
                    Model = "expire 20 days",
                    PlateNumber = "expire 20 days",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 208000,
                    Vin = "expire 20 days",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 5,
                    BrandId = 2,
                    OwnerId = 2,
                },
                 //Vignette expire after 7 days
                 //Insurance
                //Inspection
                //OilChange
                new Vehicle
                {
                    Id = 14,
                    Model = "expire 7 days",
                    PlateNumber = "expire 7 days",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 208000,
                    Vin = "expire 7 days",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 5,
                    BrandId = 2,
                    OwnerId = 2,
                },
                 //Vignette expire after 1 days
                 //Insurance
                //Inspection
                //OilChange
                new Vehicle
                {
                    Id = 15,
                    Model = "expire 1 day",
                    PlateNumber = "expire 1 day",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 208000,
                    Vin = "expire 1 day",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYearDays),
                    BranchId = 5,
                    BrandId = 2,
                    OwnerId = 2,
                },
            };

            await context.Vehicles.AddRangeAsync(vehicles);
            await context.SaveChangesAsync();
        }

        private static async Task FillInsurancePolicies(CarDbContext context)
        {
            List<InsurancePolicy> insurancePolicies = new List<InsurancePolicy>()
            {
                //ExpiredFullCascoInsurance
                new InsurancePolicy {
                    Id = 1,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    InsuranceCompanyId = 1,
                    VehicleId = 1
                },
                //Active FullCasco insurance
                new InsurancePolicy {
                    Id = 2,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays * 2 + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays * 2)),
                    Expired = false,
                    InsuranceCompanyId = 1,
                    VehicleId = 1
                },
                //Expired ThirdPartyLiability Insurance but not asign
                 new InsurancePolicy {
                     Id = 3,
                     TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 1
                },
                 //Active ThirdPartyLiability insurance
                new InsurancePolicy {
                    Id = 4,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 1
                },
                //Active FullCasco insurance
                new InsurancePolicy {
                    Id = 5,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 2
                },               
                //Start now FullCasco insurance
                new InsurancePolicy {
                    Id = 6,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.YearDays),
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 3
                },
                //Start now ThirdPartyLiability insurance
                new InsurancePolicy {
                    Id = 7,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.YearDays),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 3
                },
                //Expiring today
                new InsurancePolicy {
                    Id = 8,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 5
                },
                 //Expired after 30 days
                new InsurancePolicy {
                    Id = 9,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 1)),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 2
                },
                 //Expired after 20 days
                new InsurancePolicy {
                    Id = 10,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + 11)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 11)),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 5
                },
                 //Expired after 7 days
                new InsurancePolicy {
                    Id = 11,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.WeeklyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.WeeklyDays)),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 3
                },
                 //Expired after 1 days
                new InsurancePolicy {
                    Id = 12,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                     StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.Dayly + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.Dayly)),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 8
                }
            };
                        
            await context.InsurancePolicies.AddRangeAsync(insurancePolicies);
            await context.SaveChangesAsync();
        }

        private static async Task FillVignettes(CarDbContext context)
        {
            List<Vignette> vignettes = new List<Vignette>()
            {
                 //Old time Expired Vignette with asign
                new Vignette
                {
                    Id = 18,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays) + (int)TimesPeriod.YearDays),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 1
                },
                //Expired Vignette with asign
                new Vignette
                {
                    Id = 1,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 1
                },
                //Active Vignette
                new Vignette
                {
                    Id = 2,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays * 2 + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays * 2)),
                    Expired = false,
                    VehicleId = 1
                },
                //Expired Vignette with asign
                 new Vignette
                 {
                    Id = 3,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 2
                },
                 //Active Vignette
                new Vignette
                {
                    Id = 4,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays * 3 + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays * 3)),
                    Expired = false,
                    VehicleId = 2
                },
                //Active Vignette
                new Vignette {
                    Id = 5,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.MonthlyDays)),
                    Expired = false,
                    VehicleId = 3
                },
                //Active Vignette
                new Vignette
                {
                    Id = 6,
                    StartDate =  DateTime.UtcNow.AddDays(-((int)TimesPeriod.WeeklyDays * 3)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.WeeklyDays * 3 + (int)TimesPeriod.YearDays)),
                    Expired = false,
                    VehicleId = 4
                },
                //Active Start today Vignette
                new Vignette
                {
                    Id = 7,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 5
                },
                //Active Start Vignette
                new Vignette
                {
                    Id = 8,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays((int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 6
                },
                //Expiring Today Vignette
                new Vignette
                {
                    Id = 9,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 7
                },
                //Expiring Today Vignette
                new Vignette
                {
                    Id = 10,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 8
                },
                //Expire but not asign
                new Vignette
                {
                    Id = 11,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 9
                },
                //Expired with asign
                new Vignette
                {
                    Id = 12,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = true,
                    VehicleId = 10
                },
                 //Expired but not asign
                new Vignette
                {
                    Id = 13,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.YearDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(-(int)TimesPeriod.YearDays),
                    Expired = false,
                    VehicleId = 10
                },                
                 //Expired after 30 days
                new Vignette
                {
                    Id = 14,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 1)),
                    Expired = false,
                    VehicleId = 12
                },
                //Expired after 20 days
                new Vignette
                {
                    Id = 15,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.MonthlyDays + 11)),
                    EndDate = DateTime.UtcNow.AddDays((((int)TimesPeriod.MonthlyDays) - 11)),
                    Expired = false,
                    VehicleId = 13
                },
                //Expired after 7 days
                new Vignette
                {
                    Id = 16,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.WeeklyDays + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.WeeklyDays)),
                    Expired = false,
                    VehicleId = 14
                },
                //Expired after 1 day
                new Vignette
                {
                    Id = 17,
                    StartDate = DateTime.UtcNow.AddDays(-((int)TimesPeriod.Dayly + (int)TimesPeriod.YearDays)),
                    EndDate = DateTime.UtcNow.AddDays(((int)TimesPeriod.Dayly)),
                    Expired = false,
                    VehicleId = 15
                }
            };

            await context.Vignettes.AddRangeAsync(vignettes);
            await context.SaveChangesAsync();
        }


    }
}
