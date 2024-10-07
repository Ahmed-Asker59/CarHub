using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;

namespace Infrastructure.Data
{
    public class CarRepository : ICarRepository
    {
        private readonly CarContext _context;
        private readonly IMapper _mapper;

        public CarRepository(CarContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        async Task<IReadOnlyList<Car>> ICarRepository.GetCarsAsync()
        {
            return await _context.Cars
                         .Include(c => c.Brand)
                         .Include(c => c.Make)
                         .Include(c => c.Model).ToListAsync();
        }

        async Task<Car> ICarRepository.GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task<IReadOnlyList<Car>> GetCarsWithSpecificationsAsync(CarSpecParams carParams)
        {


            IQueryable<Car> query = _context.Cars
               .Include(c => c.Brand)
               .Include(c => c.Make)
               .Include(c => c.Model);


           
            query = query.Where(c =>
             (string.IsNullOrEmpty(carParams.SearchValue) || c.Brand.Name.ToLower().Contains(carParams.SearchValue)
              || c.Brand.Name.Contains(carParams.SearchValue) || c.Model.Name.ToLower().Contains(carParams.SearchValue)
             || c.Make.Name.Contains(carParams.SearchValue))

              && (!carParams.makeId.HasValue || c.MakeId == carParams.makeId)
            && (!carParams.modelId.HasValue || c.ModelId == carParams.modelId)
            && (!carParams.modelVariant.HasValue || c.ModelVariant == carParams.modelVariant)
            && (!carParams.carCondition.HasValue || c.CarCondition == carParams.carCondition)
            && (!carParams.priceFrom.HasValue || c.Price >= carParams.priceFrom)
            && (!carParams.priceTo.HasValue || c.Price <= carParams.priceTo)
            && (!carParams.fuel.HasValue || c.Fuel == carParams.fuel)
            && (!carParams.yearFrom.HasValue || c.ManufactureYear >= carParams.yearFrom)
            && (!carParams.yearTo.HasValue || c.ManufactureYear <= carParams.yearTo)
            && (carParams.color == null || c.Color == carParams.color)
            );

            query = query.OrderBy($"{carParams.sortBy} {carParams.sortDirection}");

            var cars = await query.Skip(carParams.PageSize * (carParams.PageIndex-1))
                                  .Take(carParams.PageSize)
                                  .ToListAsync();

           
            return cars;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Cars.CountAsync();
        }

        
    }
}
