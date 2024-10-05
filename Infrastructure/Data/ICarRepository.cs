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
    public class CarRepository : ICarRepository
    {
        private readonly CarContext _context;

        public CarRepository(CarContext context)
        {
            _context = context;

        }
        async Task<IReadOnlyList<Car>> ICarRepository.GetCarsAsync()
        {
            return await _context.Cars
                         .Include(c => c.Brand)
                         .Include(c => c.Make)
                         .Include(c => c.Model).ToListAsync();
        }

        async Task<Car> ICarRepository.GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

      
    }
}
