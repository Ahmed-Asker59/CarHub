using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepo _carRepo;
        private readonly IMapper _mapper;

        public CarController(ICarRepo carRepo, IMapper mapper)
        {
            _carRepo = carRepo;
            _mapper = mapper;

        }
        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars()
        {
            var Cars = await _carRepo.GetCarAsync();

            return Ok(_mapper.Map<IReadOnlyList<Car>, IReadOnlyList<CarDTO>>(Cars));


            //return Cars.Select(Car => new CarDTO()
            //{
            //    BrandId = Car.BrandId,
            //    Brand = Car.Brand,
            //    Make = Car.Make,
            //    Model = Car.Model,
            //    ModelVariant = Car.ModelVariant,
            //    Color = Car.Color,
            //    Transmission = Car.Transmission,
            //    Fuel = Car.Fuel,
            //    CarCondition = Car.CarCondition,
            //    Price = Car.Price,
            //    ManufactureYear = Car.ManufactureYear,
            //    IsAvailableForRental = Car.IsAvailableForRental,
            //    IsAvailableForReserve = Car.IsAvailableForReserve,
            //    ImagePath = Car.ImagePath


            //}).ToList();






        }

        [HttpGet("{id}")]

        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var Car = await _carRepo.GetCarByIdAsync(id);

            return _mapper.Map<Car, CarDTO>(Car);
            //return new CarDTO()
            //{
            //    BrandId=Car.BrandId,
            //    Brand=Car.Brand,
            //    Make=Car.Make,
            //    Model=Car.Model,
            //    ModelVariant=Car.ModelVariant,
            //    Color=Car.Color,
            //    Transmission=Car.Transmission,          
            //    Fuel=Car.Fuel,
            //    CarCondition=Car.CarCondition,
            //    Price=Car.Price,
            //    ManufactureYear=Car.ManufactureYear,
            //    IsAvailableForRental=Car.IsAvailableForRental,
            //    IsAvailableForReserve=Car.IsAvailableForReserve,
            //    ImagePath=Car.ImagePath
            //} ;











        }

    }
}
