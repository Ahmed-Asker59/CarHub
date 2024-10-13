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
    public class ReservataionController : ControllerBase
    {
        private readonly CarContext _context;
        private readonly ICarRepository _carRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;

        public ReservataionController
            (CarContext context,ICarRepository repository,
            IClientRepository clientRepository,IReservationRepository reservationRepository,IMapper mapper)
        {
            _context = context;
            _carRepository = repository;
            _clientRepository = clientRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }


        [HttpPost("allowedtoreserve")]
        public async Task<ActionResult> IsAllowedToReserve(ClientDTO clientDto)
        {
            var client = await _clientRepository.GetClientByNationalIdAsync(clientDto.NationalId);

            if (client == null)
            {
                var newClient = _mapper.Map<Client>(clientDto); 
                var newClientId = await _clientRepository.AddClientAsync(newClient); 

                return Ok(new
                {
                    allowed = true,
                    message = "Client did not exist before, but is now created. You can proceed to reserve.",
                    nationalId = newClient.NationalId 
                });
            }

            
            var lastReservation = client.Reservations.LastOrDefault();
            if (lastReservation != null && lastReservation.EndDate > DateTime.Now)
            {
                return BadRequest(new
                {
                    allowed = false,
                    message = "Client already has an active reservation."
                });
            }
        
            return Ok(new
            {
                allowed = true,
                message = "Last reservation has ended. You can reserve now.",
                nationalId = clientDto.NationalId 
            });
        }


        [HttpPost("reserve")]        
        public async Task<ActionResult> ReserveCar([FromQuery] string nationalId, [FromQuery] int carId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
            {
                return NotFound("Car not found.");
            }
            if (car.IsDeleted || !car.IsAvailableForReserve)
            {
                return BadRequest("Car is either not available for reservation or does not exist.");
            }

            
            var client = await _clientRepository.GetClientByNationalIdAsync(nationalId);

           
            var lastReservation = client.Reservations.LastOrDefault();
            if (lastReservation != null && lastReservation.EndDate > DateTime.Now)
            {
                return BadRequest("Client already has an active reservation.");
            }
            
            await _reservationRepository.CreateReservationAsync(client.Id, car.Id);
            return Ok("Reservation completed successfully.");
        }
    }
}
