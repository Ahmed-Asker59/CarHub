using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Core.Specifications;
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

        public async Task<IReadOnlyList<Rental>> GetClientRentals(int clientId)
        {
            return new List<Rental>();
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

        public async Task<IReadOnlyList<Client>> GetClientsToAlert(){

            var clients = await _context.Clients
             .Select(c => new Client
             {
                 Id = c.Id,
                 FirstName = c.FirstName,
                 Email = c.Email,

                 // Include only the rentals expiring tomorrow
                 Rentals = c.Rentals.Where(r => r.EndDate.Date == DateTime.Today.AddDays(1).Date)
                 .Select(r => new Rental
                 {
                     EndDate = r.EndDate,
                     Car = r.Car
                 }).ToList()
             })
           .Where(c => c.Rentals.Any()) // Ensure only clients with expiring rentals are returned
          .ToListAsync();

            return clients;


        }

        public async Task<decimal> CalcLateFeePerDay(int carId)
        {
            var fee =  _context.Cars.SingleOrDefault(c => c.Id == carId)
                               .Price * CarServicesPrices.LateFeeRatioPerDay;

            return fee;
        }

        public async Task<int> GetDelaysInDaysAsync(int rentalId)
        {
            var delay = 0;

            var rental = await _context.Rentals.FindAsync(rentalId);
            if (rental.ActualReturnDate.HasValue && rental.ActualReturnDate > rental.EndDate)
                delay = (int)(rental.ActualReturnDate.Value - rental.EndDate).TotalDays;

            else if (!rental.ActualReturnDate.HasValue && DateTime.Today > rental.EndDate)
                delay = (int)(DateTime.Today - rental.EndDate).TotalDays;

            return delay;
        }

        public async Task<Rental?> GetRentalByIdAsync(int id)
        {
            return await _context.Rentals.FindAsync(id);
        }

        public async Task<bool> ReturnRentalAsync(Rental rental)
        {
            rental.ActualReturnDate = DateTime.Today;

            return await SaveAsync();

        }

        public async Task<bool> CancelRentalAsync(Rental rental)
        {
             _context.Rentals.Remove(rental);


            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

    }
}