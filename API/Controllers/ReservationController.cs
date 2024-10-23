using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly CarContext _context;
        private readonly ICarRepository _carRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IMailService _emailService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;

        public ReservationController
            (CarContext context, ICarRepository repository,
            IClientRepository clientRepository, IReservationRepository reservationRepository,
            IMapper mapper, IMailService emailService, IEmailBodyBuilder emailBodyBuilder)
        {
            _context = context;
            _carRepository = repository;
            _clientRepository = clientRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _emailService = emailService;
            _emailBodyBuilder = emailBodyBuilder;
        }



        [HttpGet("allowedtoreserve/{nationalId}")]
        public async Task<ActionResult<ReserveResponseDTO>> IsAllowedToReserve(string nationalId)
        {
            var client = await _clientRepository.GetClientByNationalIdAsync(nationalId);

            if (client is null)
            {
                return new ReserveResponseDTO()
                {
                    IsAllowed = true,
                    Message = "Client did not reserve before"
                };
            }

            var lastReservation = client.Reservations.LastOrDefault();
            if (lastReservation is not null && lastReservation.EndDate > DateTime.Now)
            {
                return new ReserveResponseDTO()
                {
                    IsAllowed = false,
                    Message = "Client already has an active reservation."
                };
            }

            return new ReserveResponseDTO()
            {
                IsAllowed = true,
                Message = "Client reservation has already ended."
            };


        }


        [HttpPost("reserve")]
        public async Task<ActionResult<ReserveResponseDTO>> ReserveCar([FromBody] ClientDTO clientDTO, [FromQuery] int carId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car is null)
            {
                return NotFound("Car is not Found");
            }
            if (car.IsDeleted || !car.IsAvailableForReserve)
            {
                return BadRequest("Car is either not available for reservation or does not exist");
            }
            var isReserved = await _carRepository.IsReservedAsync(carId);
            if (isReserved)
                return new ReserveResponseDTO() { IsAllowed = false, Message = "Car is already reserved" };

            var client = await _clientRepository.GetClientByNationalIdAsync(clientDTO.NationalId);

            if (client is not null)
            {
                //confirm order
                await _reservationRepository.CreateReservationAsync(car.Id, client.Id);
            }
            else
            {
                var clientToAdd = _mapper.Map<Client>(clientDTO);
                var clientId = await _clientRepository.AddClientAsync(clientToAdd);

                await _reservationRepository.CreateReservationAsync(car.Id, clientId);


            }
            // Calculate reservation dates
            var reservationStartDate = DateTime.Now;
            var reservationEndDate = reservationStartDate.AddDays((int)CarServicesConfigurations.RegularReservationDays);

            var placeholders = new Dictionary<string, string>()
            {
            {"FirstName", $"{clientDTO.FirstName}" },
            {"CarModel", $"{car.Brand.Name} {car.Make.Name} {car.ModelVariant}" },
            { "ReserveStartDate", reservationStartDate.ToString("MMMM dd, yyyy") },
            {"ReserveEndDate", reservationEndDate.ToString("MMMM dd, yyyy") },

            };


            // Send email notification
            var emailSubject = "Car Reservation Confirmation";

            var body = _emailBodyBuilder.GenerateEmailBody("Reserve", placeholders);

            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(clientDTO.Email, emailSubject, body));

            return Ok(new ReserveResponseDTO() { IsAllowed = true, Message = string.Empty });
        }


        [HttpPost("{id}/end")]
        public async Task<IActionResult> EndReservation(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);

            if (reservation is null) return NotFound();

            reservation.EndDate = DateTime.Now;
            await _reservationRepository.EndReservationAsync(reservation);

            return Ok(new { message = "Reservation ended successfully", endDate = reservation.EndDate });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);


            if (reservation is null) return NotFound();

            var client = await _clientRepository.GetClientByIdAsync(reservation.ClientId);

            if(client is null) return NotFound();
             
            var canceled =  await _reservationRepository.CancelReservationAsync(reservation);

            if (canceled)
            {

                // Send email notification
            var emailSubject = "Cancelation Alert!";
            var placeholders = new Dictionary<string, string>()
            {
            {"FirstName", $"{client.FirstName}" },
            {"OrderType", $"Reservation" },
            {"Car", $"{reservation.Car.Brand.Name} {reservation.Car.Model.Name} {reservation.Car.ModelVariant}" },
    
            };

                var body =  _emailBodyBuilder.GenerateEmailBody("CancelOrder", placeholders);
                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(client.Email, emailSubject, body));
            }
                

            return Ok(new { message = "Reservation has been canceled successfully" });
        }




    }



}