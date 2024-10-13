using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly CarContext _context;
    private readonly ICarRepository _carRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;

    public ReservationController
        (CarContext context, ICarRepository repository,
        IClientRepository clientRepository, IReservationRepository reservationRepository, IMapper mapper)
    {
        _context = context;
        _carRepository = repository;
        _clientRepository = clientRepository;
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }



    [HttpPost("allowedtoreserve")]
    public async Task<ActionResult> IsAllowedTReserve(string nationalId)
    {
        var client = await _clientRepository.GetClientByNationalIdAsync(nationalId);

        if (client is null)
        {
            return Ok(new
            {
                allowed = true,
                message = "Client did not reserve before"
            });
        }

        var lastReservation = client.Reservations.LastOrDefault();
        if (lastReservation is not null && lastReservation.EndDate > DateTime.Now)
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
            message = "Last Reservation Has Already ended"
        });

    }


    [HttpPost("reserve")]
    public async Task<ActionResult> ReserveCar([FromBody] ClientDTO clientDTO, [FromQuery] int carId)
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