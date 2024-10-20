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
    }
}