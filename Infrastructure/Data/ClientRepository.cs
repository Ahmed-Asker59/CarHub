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
        public async Task<Client> AddClientAsync(Client client)
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
            return newClient;
        }

        public async Task<Client> GetClientByIdAsync(int Id)
        {
            var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == Id);
            return client;
        }
    }
}
