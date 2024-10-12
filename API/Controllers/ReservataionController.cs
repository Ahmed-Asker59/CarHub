using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public ReservataionController
            (CarContext context,ICarRepository repository,
            IClientRepository clientRepository,IReservationRepository reservationRepository)
        {
            _context = context;
            _carRepository = repository;
            _clientRepository = clientRepository;
            _reservationRepository = reservationRepository;
        }


        [HttpPost("reserve")]
        public async Task<ActionResult> ReserveCar([FromBody]Client client,[FromQuery] int cardId)
        {
            if (ModelState.IsValid)
            {
                var car = await _carRepository.GetCarByIdAsync(cardId);
                if (car == null)
                {
                    return NotFound("Car is not Found");
                }
                if (car.IsDeleted || !car.IsAvailableForReserve)
                {
                    return BadRequest("Car is either not available for reservation or does not exist");
                }

                var _client = await _clientRepository.GetClientByIdAsync(client.Id);

                if (_client != null)
                {
                    var activeReservation = _context.Reservations.SingleOrDefault(r => r.ClientId == _client.Id);
                    if (activeReservation.EndDate > DateTime.Now)
                    {
                        return BadRequest("Client already has an active reservation.");
                    }

                }
                else
                {
                    await _clientRepository.AddClientAsync(_client);
                }
               
                await _reservationRepository.CreateReservationAsync(_client.Id,car.Id);

            }

            return Ok("Reservation Completed Successfully");
        }
    }
}
