using CarTrade.Data;
using CarTrade.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarTrade.Data.Enums;
using System;
using CarTrade.Services.Tests.Enums;
using AutoMapper;
using CarTrade.Web.Infrastructure.Mapping;

namespace CarTrade.Services.Tests
{
    public class DatabaseFixture : IDisposable
    {

        /* 
         * Setting up and seeding the database
         * This sample recreates the database for each test. This works well for SQLite and 
         * EF in-memory database testing but can involve significant overhead with other 
         * database systems, including SQL Server.
         * */

        public DatabaseFixture()
        {
            var dbOptions = new DbContextOptionsBuilder<CarDbContext>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            this.Context = new CarDbContext(dbOptions);
            //Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
            this.MapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            this.FillBranches();
            this.FillBrands();
            this.FillEmployeers();
            this.FillInsuranceCompany();
            this.FillVehicles();
            this.FillInsurancePolicies();
            this.FillVignettes();
        }

        public List<InsurancePolicy> AllInsurance { get; set; }

        public MapperConfiguration MapperConfig { get; set; }

        public CarDbContext Context { get; set; }

        //Tip

        //The creation and seeding code does not need to be async.Making it async 
        //will complicate the code and will not improve performance or throughput of tests.


        private void FillBranches()
        {
            var firstBranch = new Branch { Id = 1, Town = "Стара Загора", Address = "бул. Никола Петков 55" };
            var secondBranch = new Branch { Id = 2, Town = "София", Address = "бул. Христо Ботев 98" };
            var thirdBranch = new Branch { Id = 3, Town = "Бургас", Address = "ж.к. Славейков, бл. 165" };
            var fourthBranch = new Branch { Id = 4, Town = "Пловдив", Address = "бул. Кукленско шосе № 3 А" };
            var fifttBranch = new Branch { Id = 5, Town = "Русе", Address = "ул. Потсдам 2" };

            this.Context.Branches.AddRange(firstBranch, secondBranch,
                thirdBranch, fourthBranch, fifttBranch);

            this.Context.SaveChanges();
        }

        private void FillBrands()
        {
            var firstBrand = new Brand { Id = 1, Name = "Volkswagen" };
            var secondBrand = new Brand { Id = 2, Name = "Audi" };

            this.Context.Brands.AddRange(firstBrand, secondBrand);

            this.Context.SaveChanges();
        }

        private void FillEmployeers()
        {
            var firstEmployeer = new Employer { Id = 1, Name = "Автохит Трейд ООД" };
            var secondEmployeer = new Employer { Id = 2, Name = "Автохит 2000" };

            this.Context.Companies.AddRange(firstEmployeer, secondEmployeer);

            this.Context.SaveChanges();
        }

        private void FillInsuranceCompany()
        {
            List<InsuranceCompany> insuranceCompanies = new List<InsuranceCompany>()
            {
                new InsuranceCompany { Id = 1, Name = "ЗД Бул инс АД" },
                new InsuranceCompany { Id = 2, Name = "ЗК Български имоти АД" },
                new InsuranceCompany { Id = 3, Name = "ЗК Уника АД" },
                new InsuranceCompany { Id = 4, Name = "ЗК Лев инс АД" },
                new InsuranceCompany { Id = 5, Name = "Дженерали застраховане АД" }
            };

            this.Context.InsuranceCompanies.AddRange(insuranceCompanies);
            this.Context.SaveChanges();
        }

        private void FillVehicles()
        {
            List<Vehicle> vehicles = new List<Vehicle>()
            {
                //Active
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
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYear),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //Active
                new Vehicle
                {
                    Id = 2,
                    Model = "Up",
                    PlateNumber = "CTTTTTBM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(6),
                    TravelledDistance = 182000,
                    EndOilChange = 190000,
                    Vin = "VWVZZZ1KZ1P111222",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.QuarterOfYear),
                    BranchId = 2,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //Expire InspectionSafetyCheck montly
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
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.Monthly),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
               //Expire InspectionSafetyCheck today
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
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
               //Expired InspectionSafetyCheck
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
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //Expired InspectionSafetyCheck and 1000 km to oil change
                new Vehicle
                {
                    Id = 6,
                    Model = "TT",
                    PlateNumber = "CT9876AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 199000,
                    Vin = "VWVZZZ1KZ1P98744",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly),
                    BranchId = 2,
                    BrandId = 2,
                    OwnerId = 2,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //Expired InspectionSafetyCheck and expired km to oil change
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
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //expire 1000 km to oil change
                new Vehicle
                {
                    Id = 8,
                    Model = "Up",
                    PlateNumber = "CT5555AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(2),
                    TravelledDistance = 198000,
                    EndOilChange = 198000,
                    Vin = "VWVZZZ1KZ1P357159",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYear),
                    BranchId = 1,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //expired km to oil change
                new Vehicle
                {
                    Id = 9,
                    Model = "Up",
                    PlateNumber = "CT75665AM",
                    YearОfМanufacture = DateTime.UtcNow.AddYears(5),
                    TravelledDistance = 198000,
                    EndOilChange = 199000,
                    Vin = "VWVZZZ1KZ1P357159",
                    Status = VehicleStatus.OnMotion,
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays((int)TimesPeriod.HalfYear),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                    //List<InsurancePolicy>
                    //List<Vignette>
                },
                //expired all
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
                    InspectionSafetyCheck = DateTime.UtcNow.AddDays(-(int)TimesPeriod.HalfYear),
                    BranchId = 3,
                    BrandId = 1,
                    OwnerId = 1,
                },
            };

            this.Context.Vehicles.AddRange(vehicles);
            this.Context.SaveChanges();
        }

        private void FillInsurancePolicies()
        {
            List<InsurancePolicy> insurancePolicies = new List<InsurancePolicy>()
            {
                //ExpiredFullCascoInsurance
                new InsurancePolicy { Id = 1,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.Parse("2020-03-21 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2021-03-21 00:00:00.0000000"),
                    Expired = true,
                    InsuranceCompanyId = 1,
                    VehicleId = 1
                },
                //Active FullCasco insurance
                new InsurancePolicy { Id = 2,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.Parse("2021-03-21 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-03-21 00:00:00.0000000"),
                    Expired = false,
                    InsuranceCompanyId = 1,
                    VehicleId = 1
                },
                //Expired ThirdPartyLiability Insurance
                 new InsurancePolicy { Id = 3,
                     TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.Parse("2020-01-01 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2021-01-01 00:00:00.0000000"),
                    Expired = true,
                    InsuranceCompanyId = 2,
                    VehicleId = 1
                },
                 //Active ThirdPartyLiability insurance
                new InsurancePolicy { Id = 4,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.Parse("2021-01-01 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 1
                },
                //Active FullCasco insurance
                new InsurancePolicy { Id = 5,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.Parse("2021-05-21 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-05-21 00:00:00.0000000"),
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 2
                },
                //Active ThirdPartyLiability insurance
                new InsurancePolicy { Id = 6,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.Parse("2021-06-01 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-06-21 00:00:00.0000000"),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 2
                },
                //Start now FullCasco insurance
                new InsurancePolicy { Id = 7,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.Year),
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 3
                },
                //Start now ThirdPartyLiability insurance
                new InsurancePolicy { Id = 8,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.Year),
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId =3
                },
                //Expiring today
                new InsurancePolicy { Id = 9,
                    TypeInsurance = TypeInsurance.FullCasco,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    InsuranceCompanyId = 3,
                    VehicleId = 5
                },
                new InsurancePolicy { Id = 10,
                    TypeInsurance = TypeInsurance.ThirdPartyLiability,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    InsuranceCompanyId = 2,
                    VehicleId = 5
                }
            };

            this.Context.InsurancePolicies.AddRange(insurancePolicies);
            this.Context.SaveChanges();
        }

        //TODO: make readonly collection
        public async Task<IList<InsurancePolicy>> AllInsurancePoliciesAsync()
            => await this.Context.InsurancePolicies.ToListAsync();

        private void FillVignettes()
        {
            List<Vignette> vignettes = new List<Vignette>()
            {
                //Expired Vignette
                new Vignette
                {
                    Id = 1,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year + (int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year),
                    Expired = true,
                    VehicleId = 1
                },
                //Active Vignette
                new Vignette
                {
                    Id = 2,
                    StartDate = DateTime.Parse("2021-03-21 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-03-21 00:00:00.0000000"),
                    Expired = false,
                    VehicleId = 1
                },
                //Expired Vignette
                 new Vignette
                 {
                    Id = 3,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year + (int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year),
                    Expired = true,
                    VehicleId = 2
                },
                 //Active Vignette
                new Vignette
                {
                    Id = 4,
                    StartDate = DateTime.Parse("2021-01-01 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-01-01 00:00:00.0000000"),
                    Expired = false,
                    VehicleId =2
                },
                //Active Vignette
                new Vignette {
                    Id = 5,
                    StartDate = DateTime.Parse("2021-05-21 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-05-21 00:00:00.0000000"),
                    Expired = false,
                    VehicleId = 3
                },
                //Active Vignette
                new Vignette
                {
                    Id = 6,
                    StartDate = DateTime.Parse("2021-06-01 00:00:00.0000000"),
                    EndDate = DateTime.Parse("2022-06-21 00:00:00.0000000"),
                    Expired = false,
                    VehicleId = 4
                },
                //Start today Vignette
                new Vignette
                {
                    Id = 7,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.Year),
                    Expired = false,
                    VehicleId = 5
                },
                //Start Vignette
                new Vignette
                {
                    Id = 8,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears((int)TimesPeriod.Year),
                    Expired = false,
                    VehicleId = 6
                },
                //Expiring Today Vignette
                new Vignette
                {
                    Id = 9,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 7
                },
                //Expiring Today Vignette
                new Vignette
                {
                    Id = 10,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow,
                    Expired = false,
                    VehicleId = 8
                },
                //Expire but not asign
                new Vignette
                {
                    Id = 11,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year + (int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year),
                    Expired = false,
                    VehicleId = 9
                },
                 //Expire but not asign
                new Vignette
                {
                    Id = 12,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year + (int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year),
                    Expired = false,
                    VehicleId = 9
                },
                //Expired with asign
                new Vignette
                {
                    Id = 13,
                    StartDate = DateTime.UtcNow.AddYears(-((int)TimesPeriod.Year + (int)TimesPeriod.Year)),
                    EndDate = DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year),
                    Expired = true,
                    VehicleId = 10
                },
            };

            this.Context.Vignettes.AddRange(vignettes);
            this.Context.SaveChanges();
        }

        public void Dispose()
        {
        }
    }
}
