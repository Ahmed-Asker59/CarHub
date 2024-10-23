using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IRentRepository _rentalRepository;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public ClientController(IClientRepository clientRepository, ICarRepository _carRepository, IRentRepository rentalRepository, IMapper autoMapper)
        {
            _clientRepository = clientRepository;
            _rentalRepository = rentalRepository;
            _mapper = autoMapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string nationalId)
        {

            var client = await _clientRepository.SearchClientAsync(nationalId);


            var mappedClient = _mapper.Map<ClientDTO>(client);

            return Ok(mappedClient);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client is null) return NotFound();

            var mappedClient = _mapper.Map<ClientDTO>(client);

            return Ok(mappedClient);
        }

        [HttpGet("{clientId}/reservations")]
        public async Task<ActionResult<List<ReservationDTO>>> Reservations(int clientId)
        {
            var client = await _clientRepository.GetClientByIdAsync(clientId);

            if (client is null) return NotFound();

            var reservations = await _clientRepository.GetReservationsAsync(clientId);

            var mappedReservations = _mapper.Map<List<ReservationDTO>>(reservations);

            


            return Ok(mappedReservations);

        }


        [HttpGet("{clientId}/rentals")]
        public async Task<ActionResult<List<RentalDTO>>> Rentals(int clientId)
        {
            var client = await _clientRepository.GetClientByIdAsync(clientId);

            if (client is null) return NotFound();

            var rentals = await _clientRepository.GetRentalsAsync(clientId);

            var mappedRentals = _mapper.Map<List<RentalDTO>>(rentals);

           foreach(var rental in mappedRentals)
            {
                var latePerDay = await _rentalRepository.CalcLateFeePerDay(rental.CarId);

                rental.LateFee = latePerDay * rental.DelayInDays;
                rental.TotalRentalPrice = rental.LateFee + rental.RentalPrice;
            }

            return Ok(mappedRentals);

        }

    


    }


}