using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IRentRepository _rentRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;

        public ReportController(IRentRepository rentRepository,
            IReservationRepository reservationRepository,
            IMapper mapper)
        {
            _rentRepository = rentRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        [HttpGet("rentals")]
        public async Task<ActionResult<IReadOnlyList<RentalDTO>>> GetRentals([FromQuery] DateTime? dateFrom=null, [FromQuery] DateTime? dateTo=null)
        {
            var Rentals = await _rentRepository.GetRentalsAsync(dateFrom,dateTo);

            var RentalsMapped = _mapper.Map<IReadOnlyList<RentalDTO>>(Rentals);

            if (RentalsMapped == null)
            {
                return NotFound("No Rentals Exists");
            }

            return Ok(RentalsMapped);
        }

        [HttpGet("delayedrentals")]
        public async Task<ActionResult<IReadOnlyList<DelayedRentalDTO>>> GetDelayedRentals()
        {
            var DelayedRentals = await _rentRepository.GetDelayedRentalsAsync();

            var DelayedRentalsMapped = _mapper.Map<IReadOnlyList<DelayedRentalDTO>>(DelayedRentals);

            if (DelayedRentalsMapped == null)
            {
                return NotFound("No Delayed Rentals Exists");
            }

            foreach (var rental in DelayedRentalsMapped)
            {
                var latePerDay = await _rentRepository.CalcLateFeePerDay(rental.CarId);

                rental.LateFee = latePerDay * rental.DelayInDays;
                rental.RentalPrice = rental.LateFee + rental.RentalPrice;
            }

            return Ok(DelayedRentalsMapped);

        }

        [HttpGet("reservations")]
        public async Task<ActionResult<IReadOnlyList<ReservationDTO>>> GetReservations()
        {
            var Reservations = await _reservationRepository.GetReservationsAsync();

            var ReservationsMapped = _mapper.Map<IReadOnlyList<ReservationDTO>>(Reservations);

            if (ReservationsMapped == null)
            {
                return NotFound("No Reservations Exists");
            }

            return Ok(ReservationsMapped);
        }

    }
}
