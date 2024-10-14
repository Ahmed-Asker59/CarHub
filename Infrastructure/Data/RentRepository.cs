using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class RentRepository : IRentRepository
    {
        private readonly CarContext _context;

        public RentRepository(CarContext context)
        {
            _context = context;
        }
        public async Task<bool> RentCar(int clientId, int carId, int rentalDays)
        {
            var rental = new Rental
            {
                CarId = carId,
                ClientId = clientId,
                RentalDays = rentalDays,
                RentalPrice = _context.Cars.SingleOrDefault(c=>c.Id==carId).Price * rentalDays * CarServicesPrices.RentalRatio,
            };
            await _context.Rentals.AddAsync(rental);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}