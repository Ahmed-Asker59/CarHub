using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IRentRepository _rentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly CarContext _Context;
        private readonly IMapper _mapper;

        public RentalController(ICarRepository carRepository,
            IRentRepository rentRepository, IClientRepository clientRepository,
            CarContext Context, IMapper mapper)
        {
            _carRepository = carRepository;
            _rentRepository = rentRepository;
            _clientRepository = clientRepository;
            _Context = Context;
            _mapper = mapper;
        }
        [HttpGet("isallowedtorent/{nationalId}")]
        public async Task<ActionResult> IsAllowedToRent(string nationalId)
        {
            var client = await _clientRepository.GetClientByNationalIdAsync(nationalId);
            if (client != null)
            {

                var currentRentals = client.Rentals.Where(c => !c.ActualReturnDate.HasValue).Count();
                var allowdRentals = (int)CarServicesConfigurations.MaxAllowedCarsForRentals - currentRentals;
                if (allowdRentals == 0)
                {
                    return BadRequest(
                        new
                        {
                            allowed = false,
                            message = "Maximum Allowed Rental Times are Exceded"
                        }
                    );
                }
                else
                {
                    return Ok(new
                    {
                        allowed = true,
                        message = "You can rent"
                    });
                }

            }
            return Ok(new
            {
                allowed = true,
                message = "Allowed to rent , client did not rent before"
            });
        }

        [HttpPost("rent")]
        public async Task<ActionResult> RentCar([FromBody]ClientDTO clientDto,[FromQuery] int carId, [FromQuery] int rentalDays)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Data Entered is not valid!");
            }
            else
            {
                var car = await _carRepository.GetCarByIdAsync(carId);
                if (car == null)
                {
                    return NotFound("Car is not found");
                }
                if (car.IsDeleted || !car.IsAvailableForRental)
                {
                    return BadRequest("Car is doesnot exist or is not available for rental");
                }
                var client = await _clientRepository.GetClientByNationalIdAsync(clientDto.NationalId);
                if (client != null)
                {
                    var currentRentals = client.Rentals.Where(c => !c.ActualReturnDate.HasValue).Count();
                    var allowdRentals = (int)CarServicesConfigurations.MaxAllowedCarsForRentals - currentRentals;
                    if (allowdRentals == 0)
                    {
                        return BadRequest(
                            new
                            {
                                allowed = false,
                                message = "Maximum Allowed Rental Times are Exceded"
                            }
                        );
                    }
                    await _rentRepository.RentCar(client.Id, carId, rentalDays);

                }
                else
                {
                    var clientToAdd = _mapper.Map<Client>(clientDto);
                    var clientId = await _clientRepository.AddClientAsync(clientToAdd);
                    await _rentRepository.RentCar(clientId, carId, rentalDays);

                }
                
            }
            return Ok("Rental is completed Successfully");
        }
    }
}