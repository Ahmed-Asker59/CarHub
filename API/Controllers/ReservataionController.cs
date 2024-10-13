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
        public async Task<ActionResult> IsAllowedTReserve(ClientDTO clientDto)
        {
            var client = await _clientRepository.GetClientByNationalIdAsync(clientDto.NationalId);

            if(client is null)
            {
                return Ok("Client did not reserve before");
            }

            var lastReservation = client.Reservations.LastOrDefault();
            if (lastReservation is not null && lastReservation.EndDate < DateTime.Now)
            {
                return BadRequest("Client already has an active reservation.");
            }

            return Ok("Last Reservation Has Already ended");

        }


        [HttpPost("reserve")]
        public async Task<ActionResult> ReserveCar([FromBody] ClientDTO clientDTO, [FromQuery] int cardId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var car = await _carRepository.GetCarByIdAsync(cardId);
            if (car is null)
            {
                return NotFound("Car is not Found");
            }
            if (car.IsDeleted || !car.IsAvailableForReserve)
            {
                return BadRequest("Car is either not available for reservation or does not exist");
            }

            var client = await _clientRepository.GetClientByNationalIdAsync(clientDTO.NationalId);

            if (client is not null)
            {
                 await _reservationRepository.CreateReservationAsync(client.Id, car.Id);    
            }
            else
            {
                var clientToAdd = _mapper.Map<Client>(clientDTO);
                var clientId = await _clientRepository.AddClientAsync(clientToAdd);

                await _reservationRepository.CreateReservationAsync(clientId, car.Id);
            }

            return Ok("Reservation Completed Successfully");
        }
    }
}
