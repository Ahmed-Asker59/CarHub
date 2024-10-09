using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly CarContext _carContext;
        private readonly IMapper _mapper;

        public CarController(ICarRepository carRepository, IMapper mapper, CarContext carContext)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _carContext = carContext;
        }


        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars([FromQuery] CarSpecParams carParams)
        {


            var carsWithSpecifiactions = await _carRepository.GetCarsWithSpecificationsAsync(carParams);

            var paginationList = new PaginationList
            {
                PageSize = carParams.PageSize,
                PageIndex = carParams.PageIndex,
                Count = await _carRepository.CountAsync(),
                Data = _mapper.Map<IReadOnlyList<CarDTO>>(carsWithSpecifiactions)
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
