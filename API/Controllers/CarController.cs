using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public CarController(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;

        }


        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars(
        int? makeId, int? modelId,
        ModelVariant? modelVariant,
        CarCondition? carCondition,
        int? priceFrom,int? priceTo,
        FuelType? fuel,
        int? yearFrom,int? yearTo,
        int? mileageFrom,int? mileageTo,
        string? color,
        string sortBy = "make",
        int pageNumber = 0,
        int pageSize = 10) 
        {
            var Cars = await _carRepository.GetCarsAsync();
            IQueryable query = Cars.AsQueryable();
            PaginationList paginationList ;
            if(makeId != null)
            {
                Cars.Where(car => car.MakeId == makeId.Value);
            }
            if (modelId != null)
            {
                Cars.Where(car => car.ModelId == modelId.Value);
            }
            if (modelVariant != null) { 
            Cars.Where(car=>car.ModelVariant == modelVariant.Value);
            }
            if (carCondition != null) { 
            Cars.Where(car=>car.CarCondition== carCondition.Value);
            }
            if (priceFrom != null) { 
            Cars.Where(car=>car.Price>= priceFrom.Value);
            }
            if (priceTo != null)
            {
                Cars.Where(car => car.Price <= priceTo.Value);
            }
            if(fuel != null)
            {
                Cars.Where(car=>car.Fuel== fuel.Value);
            }
            if (mileageFrom != null)
            {
                Cars.Where(car => car.Mileage >= mileageFrom.Value);
            }
            if (mileageTo != null)
            {
                Cars.Where(car => car.Mileage <= mileageTo.Value);
            }
            if (yearFrom != null) {
                Cars.Where(car => car.ManufactureYear >= yearFrom.Value);
            }
            if (yearTo != null)
            {
                Cars.Where(car => car.ManufactureYear <= yearTo.Value);
            }
            if(color != null)
            {
                Cars.Where(car => car.Color == color);
            }

            switch ( sortBy.ToLower()) 
            {
               case ("make"): Cars.OrderBy(car => car.Make.Name); break;
                case ("model"): Cars.OrderBy(car => car.Model.Name); break;
                case ("brand"): Cars.OrderBy(car => car.Brand.Name); break;
                default: Cars.OrderBy(car => car.Make.Name);break;
            }
           
            var paginatedCars = Cars.Skip(pageNumber).Take(pageSize).ToList();
           
             paginationList = new PaginationList
            {
                PageSize = pageSize,
                PageIndex = pageNumber,
                Count = Cars.Count(),
                 Data = _mapper.Map<IReadOnlyList<CarDTO>>(paginatedCars)
             };
            return Ok(paginationList);
           
        }

         



        [HttpGet("{id}")]
        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var Car = await _carRepository.GetCarByIdAsync(id);

            return _mapper.Map<Car, CarDTO>(Car);

        }

    }
}
