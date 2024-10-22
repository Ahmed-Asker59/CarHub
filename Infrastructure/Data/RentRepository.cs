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

        public async Task<IReadOnlyList<Rental>> GetRentalsAsync(DateTime? from = null, DateTime? to = null)
        {

            var startDate = from ?? DateTime.MinValue; // Default to a very early date if not provided
            var endDate = to ?? DateTime.MaxValue;     // Default to a very late date if not provided

            var query = _context.Rentals
                         .Include(r => r.Client)
                         .Include(r => r.Car)
                         .Include(c => c.Car.Brand)
                         .Include(c => c.Car.Make)
                         .Include(c => c.Car.Model)
                         .AsNoTracking();

            // Filter by the provided date range
            query = query.Where(r => r.StartDate >= startDate && r.EndDate <= endDate);

            return await query.ToListAsync();
        }
        public async Task<IReadOnlyList<Rental>> GetDelayedRentalsAsync()
        {
            var DelayedRentals = await _context.Rentals
               .Include(r => r.Client).Include(r => r.Car).Include(c => c.Car.Brand)
                .Include(c => c.Car.Make)
                .Include(c => c.Car.Model).Where(r=>r.ActualReturnDate > r.EndDate).AsNoTracking().ToListAsync();
            return DelayedRentals;
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