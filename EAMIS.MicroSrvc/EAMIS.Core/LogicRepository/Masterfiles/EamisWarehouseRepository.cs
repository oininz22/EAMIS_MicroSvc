﻿using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisWarehouseRepository : IEamisWarehouseRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisWarehouseRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisWarehouseDTO> Delete(EamisWarehouseDTO item)
        {
            EAMISWAREHOUSE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISWAREHOUSE MapToEntity(EamisWarehouseDTO item)
        {
            if (item == null) return new EAMISWAREHOUSE();
            return new EAMISWAREHOUSE
            {
                ID = item.Id,
                WAREHOUSE_DESCRIPTION = item.Warehouse_Description,
                STREET_NAME = item.Street_Name,
                BARANGAY_CODE = item.Barangay_Code,
                REGION_CODE = item.Region_Code,
                MUNICIPALITY_CODE = item.Municipality_Code,
                PROVINCE_CODE = item.Province_Code
            };
        }

        public async Task<EamisWarehouseDTO> Insert(EamisWarehouseDTO item)
        {
            EAMISWAREHOUSE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisWarehouseDTO>> List(EamisWarehouseDTO filter, PageConfig config)
        {
            IQueryable<EAMISWAREHOUSE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisWarehouseDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisWarehouseDTO> QueryToDTO(IQueryable<EAMISWAREHOUSE> query)
        {
            return query.Select(x => new EamisWarehouseDTO
            {
                Id = x.ID,
                Warehouse_Description = x.WAREHOUSE_DESCRIPTION,
                Street_Name = x.STREET_NAME,
                Barangay_Code = x.BARANGAY_CODE,
                Region_Code = x.REGION_CODE,
                Municipality_Code = x.MUNICIPALITY_CODE,
                Province_Code = x.PROVINCE_CODE,
                Barangay = new EamisBarangayDTO
                {
                    BrgyCode = x.BARANGAY.BRGY_CODE,
                    BrgyDescription = x.BARANGAY.BRGY_DESCRIPTION.ToUpper(),
                    Municipality = new EamisMunicipalityDTO
                    {
                        CityMunicipalityCode = x.BARANGAY.MUNICIPALITY.MUNICIPALITY_CODE,
                        CityMunicipalityDescription = x.BARANGAY.MUNICIPALITY.CITY_MUNICIPALITY_DESCRIPTION
                    },
                    Province = new EamisProvinceDTO
                    {
                        ProvinceCode = x.BARANGAY.PROVINCE.PROVINCE_CODE,
                        ProvinceDescription = x.BARANGAY.PROVINCE.PROVINCE_DESCRITION
                    },
                    Region = new EamisRegionDTO
                    {
                        RegionCode = x.BARANGAY.REGION.REGION_CODE,
                        RegionDescription = x.BARANGAY.REGION.REGION_DESCRIPTION
                    }
                }
                //RegionDTO = new EamisRegionDTO
                //{
                //    RegionCode = x.REGION.REGION_CODE,
                //}
            });
        }

        private IQueryable<EAMISWAREHOUSE> PagedQuery(IQueryable<EAMISWAREHOUSE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISWAREHOUSE> FilteredEntities(EamisWarehouseDTO filter, IQueryable<EAMISWAREHOUSE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISWAREHOUSE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Warehouse_Description)) predicate = (strict)
                     ? predicate.And(x => x.WAREHOUSE_DESCRIPTION.ToLower() == filter.Warehouse_Description.ToLower())
                     : predicate.And(x => x.WAREHOUSE_DESCRIPTION.Contains(filter.Warehouse_Description.ToLower()));
            if (!string.IsNullOrEmpty(filter.Street_Name)) predicate = (strict)
                     ? predicate.And(x => x.STREET_NAME.ToLower() == filter.Street_Name.ToLower())
                     : predicate.And(x => x.STREET_NAME.Contains(filter.Street_Name.ToLower()));
            if (filter.Region_Code != null && filter.Region_Code != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.Region_Code);
            if (filter.Municipality_Code != null && filter.Municipality_Code != 0)
                predicate = predicate.And(x => x.MUNICIPALITY_CODE == filter.Municipality_Code);
            if (filter.Province_Code != null && filter.Province_Code != 0)
                predicate = predicate.And(x => x.PROVINCE_CODE == filter.Province_Code);
            if (filter.Barangay_Code != null && filter.Barangay_Code != 0)
                predicate = predicate.And(x => x.BARANGAY_CODE == filter.Barangay_Code);
            var query = custom_query ?? _ctx.EAMIS_WAREHOUSE;
            return query.Where(predicate);
        }

        public async Task<EamisWarehouseDTO> Update(EamisWarehouseDTO item)
        {
            EAMISWAREHOUSE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
