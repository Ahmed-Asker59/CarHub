using Core.Entities;
using Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ClientRepository : IClientRepository
    {
        private readonly CarContext _context;

        public ClientRepository(CarContext context)
        {
            _context = context;
        }
        public async Task<int> AddClientAsync(Client client)
        {
            var newClient = new Client()
            {
                NationalId = client.NationalId,
                Address = client.Address,
                Email = client.Email,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Phone = client.Phone,
            };
            await _context.Clients.AddAsync(newClient);
            await _context.SaveChangesAsync();
            return newClient.Id;
        }

        public async Task<Client> GetClientByIdAsync(int Id)
        {
            var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == Id);
            return client;
        }
        public Task<Client?> GetClientByNationalIdAsync(string nationalId)
        {
            return _context.Clients.Where(c => c.NationalId == nationalId)
                       .Include(c => c.Reservations)
                       .Include(c=>c.Rentals)
                       .FirstOrDefaultAsync();

        }

        public async Task<string> GetEmailAsync(int clientId)
        {
            var client =  await _context.Clients.Where(c => c.Id == clientId)                                  
                                   .FirstOrDefaultAsync();
            if(client is not null)
                return client.Email;

            return string.Empty;
                                   
        }

        public async Task<IReadOnlyList<Reservation>> GetReservationsAsync(int clientId)
        {
            var reservations = await _context.Reservations
                                    .Where(r => r.ClientId == clientId)
                                    .Include(r => r.Car)
                                    .Include(r => r.Car.Model)
                                    .Include(r => r.Car.Brand)
                                    .OrderByDescending(r => r.Id).ToListAsync();

            return reservations;
        }

        public async Task<IReadOnlyList<Rental>> GetRentalsAsync(int clientId)
        {
            var rentals = await _context.Rentals
                                    .Where(r => r.ClientId == clientId)
                                    .Include(r => r.Car)
                                    .Include(r => r.Car.Model)
                                    .Include(r => r.Car.Brand)
                                    .OrderByDescending(r => r.Id).ToListAsync();

            return rentals;
        }

        public async Task<Client?> SearchClientAsync(string nationalId)
        {
            var client = await _context.Clients.Where(c => c.NationalId == nationalId)
                                            .FirstOrDefaultAsync();                                
                            
            return client;
        }

        
    }
}
