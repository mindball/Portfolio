﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarTrade.Data;
using CarTrade.Data.Enums;
using CarTrade.Services.InsurancePolicy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Services.InsurancePolicy
{
    public class InsurancesPoliciesService : IInsurancesPoliciesService
    {
        private readonly CarDbContext db;
        private IMapper mapper;

        public InsurancesPoliciesService(CarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPolicyAsync(int vehicleId, InsurancePolicyFormServiceModel newPolicy)
        { 
            if (!this.CompareStartEndDate(newPolicy.StartDate, newPolicy.EndDate))
            {
                throw new ArgumentException("Start date must be small than end date" +
                    "or the date must not be less than one year back");
            }
            
            if(!await ExistInsuranceCompany(newPolicy.InsuranceCompanyId))
            {
                throw new ArgumentException("Missing Insurance company");
            }

            if(await ExistTypeOfInsurancePolicyOnVehicle(
                vehicleId, 
                newPolicy.TypeInsurance, 
                newPolicy.Expired))
            {
                throw new ArgumentException("This policy exist and it is active");
            }

            var newInsurancePolicy =
                this.mapper
                .Map<InsurancePolicyFormServiceModel, Data.Models.InsurancePolicy>(newPolicy, opt =>
                        opt.ConfigureMap()
                        .ForMember(p => p.Id, opt => opt.Ignore())
                        .ForMember(p => p.InsuanceCompany, opt => opt.Ignore())
                        .ForMember(p => p.Vehicle, opt => opt.Ignore())
                        .ForMember(p => p.VehicleId, opt => opt.Ignore()));

            newInsurancePolicy.VehicleId = vehicleId;

            await this.db.InsurancePolicies.AddAsync(newInsurancePolicy);
            await this.db.SaveChangesAsync();
        }

        public async Task EditPolicyAsync(int insuranceId, InsurancePolicyFormServiceModel insurancePolicyModel)
        {
            var existInsurancePolicy = await this.db.InsurancePolicies.FindAsync(insuranceId);
           
            if(existInsurancePolicy == null)
            {
                throw new ArgumentException("Missing policy");
            }

            if (!this.CompareStartEndDate(insurancePolicyModel.StartDate, insurancePolicyModel.EndDate))
            {
                throw new ArgumentException("Start date must be small than end date");
            }


            //TODO: This map doesnt work db return 0 row affected
            //var vehicleId = existInsurancePolicy.VehicleId;
            //existInsurancePolicy =
            //      this.mapper
            //      .Map<InsurancePolicyFormServiceModel, Data.Models.InsurancePolicy>(insurancePolicyModel, opt =>
            //              opt.ConfigureMap()
            //              .ForMember(p => p.Id, opt => opt.Ignore())
            //              .ForMember(p => p.InsuanceCompany, opt => opt.Ignore())
            //              .ForMember(p => p.Vehicle, opt => opt.Ignore())
            //              .ForMember(p => p.VehicleId, opt => opt.Ignore())
            //              );

            // existInsurancePolicy.VehicleId = vehicleId;

            existInsurancePolicy.TypeInsurance = insurancePolicyModel.TypeInsurance;
            existInsurancePolicy.StartDate = insurancePolicyModel.StartDate;
            existInsurancePolicy.EndDate = insurancePolicyModel.EndDate;
            existInsurancePolicy.Expired = insurancePolicyModel.Expired;
            existInsurancePolicy.InsuranceCompanyId = insurancePolicyModel.InsuranceCompanyId;

            var result = await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<InsurancePolicyListingServiceModel>> GetAllInsuranceByVehicleId(int vehicleId)
            => await this.db.InsurancePolicies
            .Where(v => v.VehicleId == vehicleId)
            .ProjectTo<InsurancePolicyListingServiceModel>()
            .ToListAsync();

        public async Task<TModel> GetByIdAsync<TModel>(int insuranceId) where TModel : class
        {
            var existInsurancePolicy =
                await this.db.InsurancePolicies
                .Where(ip => ip.Id == insuranceId)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();
                

            //TODO: make custom exception handler on controllers
            if (existInsurancePolicy == null)
            {
                return null;
                throw new ArgumentException("Missing policy");
            } 

            return existInsurancePolicy;
        }

        private bool CompareStartEndDate(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return false;
            }

            if(startDate <= DateTime.UtcNow.AddYears(-1))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ExistInsuranceCompany(int insuranceCompanyId)
        {
            var insuranceCompany = await db.InsuranceCompanies.FindAsync(insuranceCompanyId);
            if (insuranceCompany == null)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ExistTypeOfInsurancePolicyOnVehicle(int vehicleId, TypeInsurance insuranceType, bool? expire)
            => await this.db.InsurancePolicies
            .AnyAsync(i => 
            i.VehicleId == vehicleId 
            && i.TypeInsurance == insuranceType 
            && i.Expired == false);
    }
}
