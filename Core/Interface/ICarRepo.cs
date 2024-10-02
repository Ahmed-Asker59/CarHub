using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ICarRepo
    {
        Task<Car> GetCarByIdAsync(int id);
        Task<IReadOnlyList<Car>> GetCarAsync();




    }
}
