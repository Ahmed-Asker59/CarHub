using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;


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

        async Task<Car?> ICarRepository.GetCarByIdAsync(int id)
        {
            return await _context.Cars
                                 .Where(c => c.Id == id)
                                 .Include(c => c.Brand)
                                 .Include(c => c.Make)
                                 .Include( c => c.Model)
                                 .FirstOrDefaultAsync();
                                  
        }

        public static string GetEnumStringValue(ModelVariant modelVariant)
        {
            return modelVariant.ToString().ToLower();
        }
        public async Task<IReadOnlyList<Car>> GetCarsWithSpecificationsAsync(CarSpecParams carParams)
        {


            IQueryable<Car> query = _context.Cars
               .Include(c => c.Brand)
               .Include(c => c.Make)
               .Include(c => c.Model);



            query = query.Where(c =>
             (string.IsNullOrEmpty(carParams.SearchValue) || c.Brand.Name.ToLower().Contains(carParams.SearchValue)
              || c.Model.Name.ToLower().Contains(carParams.SearchValue)
             || c.Make.Name.Contains(carParams.SearchValue)
              || c.Color.ToLower().Contains(carParams.SearchValue)  
              )
            && (!carParams.makeId.HasValue || c.MakeId == carParams.makeId)
            && (!carParams.modelId.HasValue || c.ModelId == carParams.modelId)
            && (!carParams.modelVariant.HasValue || c.ModelVariant == carParams.modelVariant)
            && (!carParams.carCondition.HasValue || c.CarCondition == carParams.carCondition)
            && (!carParams.transmission.HasValue || c.Transmission == carParams.transmission)
            && (!carParams.priceFrom.HasValue || c.Price >= carParams.priceFrom)
            && (!carParams.priceTo.HasValue || c.Price <= carParams.priceTo)
            && (!carParams.fuel.HasValue || c.Fuel == carParams.fuel)
            && (!carParams.yearFrom.HasValue || c.ManufactureYear >= carParams.yearFrom)
            && (!carParams.yearTo.HasValue || c.ManufactureYear <= carParams.yearTo)
            && (!carParams.mileageFrom.HasValue || c.Mileage >= carParams.yearTo)
            && (!carParams.mileageTo.HasValue || c.Mileage <= carParams.yearTo)
            && (carParams.color == null || c.Color == carParams.color)
            );

            if (carParams.sortBy.ToLower() == "name")
                carParams.sortBy = SortByOptions.Name;

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

        public async Task<IReadOnlyList<Make>> GetMakesAsync(int? brandId, int? makeId, int? modelId)
        {
           var makes = await _context.Makes
                           .Where(m => ( !brandId.HasValue || m.BrandId == brandId)
                                  && (!makeId.HasValue || m.Id == makeId)
                                  && (!modelId.HasValue || m.Models.Any(model => model.Id == modelId))
                                  )
                           .ToListAsync();

            return makes;
        }

        public async Task<IReadOnlyList<Model>> GetModelsAsync(int? brandId, int? makeId, int? modelId)
        {
          var models = await _context.Models.Where(m => (!brandId.HasValue || m.Make.BrandId == brandId.Value)
                                        && (!makeId.HasValue || m.MakeId == makeId)
                                        && (!modelId.HasValue || m.Id == modelId))
                                     .ToListAsync();

            return models;
        }

        public async Task<IReadOnlyList<ModelVariant>> GetModelVariantsAsync(int? brandId, int? makeId, int? modelId)
        {
            var modelVariants = await _context.Cars
                                       .Where(c => (!brandId.HasValue || c.BrandId == brandId)
                                        && (!makeId.HasValue || c.MakeId == makeId)
                                        && (!modelId.HasValue || c.ModelId == modelId))
                                       .Select(c => c.ModelVariant)
                                        .ToListAsync();

            return modelVariants;
        }

        public async Task<IReadOnlyList<CarCondition>> GetCarConditionsAsync(int? brandId, int? makeId, int? modelId)
        {
            var carConditions = await _context.Cars
                                       .Where(c => (!brandId.HasValue || c.BrandId == brandId)
                                        && (!makeId.HasValue || c.MakeId == makeId)
                                        && (!modelId.HasValue || c.ModelId == modelId))
                                       .Select(c => c.CarCondition)
                                        .ToListAsync();

            return carConditions;

        }

        public async Task<bool> IsReservedAsync(int carId)
        {
            return await _context.Reservations.AnyAsync(r=>r.CarId == carId && r.EndDate > DateTime.Now);
        }

        public async Task<bool> IsRentedAsync(int id)
        {
            return await _context.Rentals.AnyAsync(r => r.CarId == id
                        && !r.ActualReturnDate.HasValue);
        }

        public async Task<string> GetCarNameAsync(int carId)
        {
            var name = await _context.Cars
                           .Where(c => c.Id == carId)
                           .Include(c => c.Brand)
                           .Include(c => c.Make)
                           .Include(c => c.Model)
                           .Select(c => c.Brand.Name + " " + c.Model.Name + " " + c.ModelVariant.ToString())
                           .FirstOrDefaultAsync();


            return name;
        }
    }

}
