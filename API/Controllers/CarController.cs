using API.DTO;
using API.Helper;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace API.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public CarController(ICarRepository carRepository, IMapper mapper, CarContext carContext)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars([FromQuery] CarSpecParams carParams)
        {
            
            var carsWithSpecifiactions = await _carRepository.GetCarsWithSpecificationsAsync(carParams);


            // Get updated filter options based on filtered cars
            var filterOptions = new FilterOptionsResolver(carsWithSpecifiactions,_mapper).GenerateOptions();

            
            var paginationList = new PaginationList
            {
                PageSize = carParams.PageSize,
                PageIndex = carParams.PageIndex,
                Count = await _carRepository.CountAsync(),
                Data = _mapper.Map<IReadOnlyList<CarDTO>>(carsWithSpecifiactions),
                FilterOptions = filterOptions
                
            };

            
            return Ok(paginationList);
           
        }

         



        [HttpGet("{id}")]
        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            if(car is null)
                return NotFound();

            var carMapped = _mapper.Map<CarDTO>(car);

            carMapped.IsReserved = await _carRepository.IsReservedAsync(id);


           

            return Ok(carMapped);

        }


        [HttpGet("makes")]
        public async Task<ActionResult<List<MakeDTO>>> GetMakes([FromQuery] int? brandId, [FromQuery] int? makeId, [FromQuery] int? modelId)
        {


           var makes = await _carRepository.GetMakesAsync(brandId, makeId, modelId);

           var makesMapped = _mapper.Map<IReadOnlyList<MakeDTO>>(makes);

            return Ok(makesMapped);
            
        }

        [HttpGet("models")]
        public async Task<ActionResult<List<ModelDTO>>> GetModels([FromQuery] int brandId ,[FromQuery] int? makeId, [FromQuery] int? modelId )
        {


            var models = await _carRepository.GetModelsAsync(brandId, makeId, modelId);

            var modelsMapped = _mapper.Map<IReadOnlyList<ModelDTO>>(models);

            return Ok(modelsMapped);

        }


        [HttpGet("modelvariants")]
       public async Task<ActionResult<List<ModelVariantDTO>>> GetModelVariants([FromQuery] int? brandId, [FromQuery] int? makeId, [FromQuery] int? modelId)
        {


           var modelVariants = await _carRepository.GetModelVariantsAsync(brandId, makeId, modelId);

           var modelsVariantsMapped = _mapper.Map<IReadOnlyList<ModelVariantDTO>>(modelVariants);

           return Ok(modelsVariantsMapped);

        }


    }
}
