using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepo;
        private readonly IMapper _mapper;

        public CarController(ICarRepository carRepo, IMapper mapper)
        {
            _carRepo = carRepo;
            _mapper = mapper;

        }
        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars()
        {
            var Cars = await _carRepo.GetCarsAsync();

            return Ok(_mapper.Map<IReadOnlyList<Car>, IReadOnlyList<CarDTO>>(Cars));

        }





        [HttpGet("{id}")]
        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var Car = await _carRepo.GetCarByIdAsync(id);

            return _mapper.Map<Car, CarDTO>(Car);
         


        }

    }
}
