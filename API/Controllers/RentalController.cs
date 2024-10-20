using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
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
        private readonly IMailService _mailService;

        public RentalController(ICarRepository carRepository,
            IRentRepository rentRepository, IClientRepository clientRepository,
            CarContext Context, IMapper mapper, IMailService mailService)
        {
            _carRepository = carRepository;
            _rentRepository = rentRepository;
            _clientRepository = clientRepository;
            _Context = Context;
            _mapper = mapper;
            _mailService = mailService;
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

                // Send email notification
                var emailSubject = "Car Rental Confirmation";
                var emailBody = $"Dear {clientDto.FirstName} {clientDto.LastName},<br/><br/>" +
                                $"Your Rent for the car {car.Model} has been confirmed.<br/>" +
                                $"Rent Start Date: {rentalStartDate:MMMM dd, yyyy}<br/>" +
                                $"Rent End Date: {rentalEndDate:MMMM dd, yyyy}<br/><br/>" +
                                $"Thank you for choosing us!<br/><br/>" +
                                $"Best regards,<br/>" +
                                $"Your Car Rental Team";
                var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\EmailTemplate.html";
                var str = new StreamReader(filePath);
                var mailText = str.ReadToEnd();
                str.Close();
                mailText = mailText.Replace("[Type]", "Rental")
                                   .Replace("[Header]", "Rental is Confirmed")
                                   .Replace("[FirstName]", clientDto.FirstName)
                                   .Replace("[LastName]", clientDto.LastName)
                                   .Replace("[CarModel]", $"{car.Brand.Name} {car.Make.Name} {car.ModelVariant}")
                                   .Replace("[RentalStartDate]", rentalStartDate.ToString("MMMM dd, yyyy"))
                                   .Replace("[RentalEndDate]", rentalEndDate.ToString("MMMM dd, yyyy"))
                                   .Replace("[Body]", emailBody);  // If "Body" is a fallback

                // Send the email
                await _mailService.SendEmailAsync(clientDto.Email, emailSubject, mailText);
            }
            return Ok(new RentalResponseDTO() { IsAllowed = true , Message = string.Empty});
        }
    }
}