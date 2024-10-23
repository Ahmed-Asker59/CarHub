using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IMailService _emailService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;

        public RentalController(ICarRepository carRepository,
            IRentRepository rentRepository, IClientRepository clientRepository,
            CarContext Context, IMapper mapper, IMailService mailService, IEmailBodyBuilder emailBodyBuilder)
        {
            _carRepository = carRepository;
            _rentRepository = rentRepository;
            _clientRepository = clientRepository;
            _Context = Context;
            _mapper = mapper;
            _emailService = mailService;
            _emailBodyBuilder = emailBodyBuilder;
        }


 
        [HttpGet("isallowedtorent/{nationalId}")]
        public async Task<ActionResult<RentalResponseDTO>> IsAllowedToRent(string nationalId)
        {
            var client = await _clientRepository.GetClientByNationalIdAsync(nationalId);
            if (client != null)
            {

                var currentRentals = client.Rentals.Where(c => !c.ActualReturnDate.HasValue).Count();
                var allowdRentals = (int)CarServicesConfigurations.MaxAllowedCarsForRentals - currentRentals;
                if (allowdRentals == 0)
                {
                    return
                        new RentalResponseDTO
                        {
                            IsAllowed = false,
                            Message = "Maximum Allowed Rental Times is Exceded"
                        };
                    
                }
                else
                {
                    return new RentalResponseDTO
                    {
                        IsAllowed = true,
                        Message = "You can rent"
                    };
                }

            }
            return new RentalResponseDTO
            {
                IsAllowed = true,
                Message = "Allowed to rent , client did not rent before"
            };
        }

        [HttpPost("rent")]
        public async Task<ActionResult<RentalResponseDTO>> RentCar([FromBody]ClientDTO clientDto,[FromQuery] int carId, [FromQuery] int rentalDays)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

                var isRented = await _carRepository.IsRentedAsync(carId);
                if (isRented)
                    return new RentalResponseDTO() { IsAllowed = false, Message = "Car is already rented" };
                var client = await _clientRepository.GetClientByNationalIdAsync(clientDto.NationalId);
                if (client != null)
                {
                    var currentRentals = client.Rentals.Where(c => !c.ActualReturnDate.HasValue).Count();
                    var allowdRentals = (int)CarServicesConfigurations.MaxAllowedCarsForRentals - currentRentals;
                    if (allowdRentals == 0)
                    {
                        return
                            new RentalResponseDTO
                            {
                                IsAllowed = false,
                                Message = "Maximum Allowed Rental Times are Exceded"
                            };
                        
                    }
                    await _rentRepository.RentCar(client.Id, carId, rentalDays);
                    
                }
                else
                {
                    var clientToAdd = _mapper.Map<Client>(clientDto);
                    var clientId = await _clientRepository.AddClientAsync(clientToAdd);
                    await _rentRepository.RentCar(clientId, carId, rentalDays);

                }
                // Calculate start and end dates for the rental
                var rentalStartDate = DateTime.Now;
                var rentalEndDate = rentalStartDate.AddDays(rentalDays);
                var lateFeePerDay = await _rentRepository.CalcLateFeePerDay(carId);

                

                // Send email notification
                var emailSubject = "Car Rental Confirmation";
                var placeholders = new Dictionary<string, string>()
            {
            {"FirstName", $"{clientDto.FirstName}" },
            {"CarModel", $"{car.Brand.Name} {car.Make.Name} {car.ModelVariant}" },
            { "RentalStartDate", rentalStartDate.ToString("MMMM dd, yyyy") },
            {"RentalEndDate", rentalEndDate.ToString("MMMM dd, yyyy") },
             {"PenaltyAmount",$"{lateFeePerDay.ToString("C")}" }
            };

             var body = _emailBodyBuilder.GenerateEmailBody("Rent",placeholders);
                // Send the email
                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(clientDto.Email, emailSubject, body));

            }
            return Ok(new RentalResponseDTO() { IsAllowed = true , Message = string.Empty});
        }


        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnRental(int id)
        {
            var rental = await _rentRepository.GetRentalByIdAsync(id);

            if (rental is null) return NotFound();

            rental.EndDate = DateTime.Now;
            await _rentRepository.ReturnRentalAsync(rental);

            return Ok(new { message = "Rental returned successfully", endDate = rental.EndDate });
        }


        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelRental(int id)
        {
            var rental = await _rentRepository.GetRentalByIdAsync(id);

            if (rental is null) return NotFound();

            var client = await _clientRepository.GetClientByIdAsync(rental.ClientId);

            if (client is null) return NotFound();


            var canceled =  await _rentRepository.CancelRentalAsync(rental);

            if (canceled)
            {

                // Send email notification
                var emailSubject = "Cancelation Alert!";
                var placeholders = new Dictionary<string, string>()
            {
            {"FirstName", $"{client.FirstName}" },
            {"OrderType", $"Rental" },
            {"Car", $"{rental.Car.Brand.Name} {rental.Car.Model.Name} {rental.Car.ModelVariant}" },

            };

                var body = _emailBodyBuilder.GenerateEmailBody("CancelOrder", placeholders);
                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(client.Email, emailSubject, body));
            }


            return Ok(new { message = "Rental has been canceled successfully"});
        }



    }
}