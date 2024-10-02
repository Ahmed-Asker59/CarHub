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
    public class CarRepo : ICarRepo
    {
        private readonly CarContext _context;

        public CarRepo(CarContext context)
        {
            _context = context;

        }
        async Task<IReadOnlyList<Car>> ICarRepo.GetCarAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        async Task<Car> ICarRepo.GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }


    }
}
