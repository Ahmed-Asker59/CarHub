using Core.Entities;
using Core.Entities.Consts;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ICarRepository
    {
        Task<Car?> GetCarByIdAsync(int id);
        Task<IReadOnlyList<Car>> GetCarsAsync();

        Task<int> CountAsync();
        Task<IReadOnlyList<Car>> GetCarsWithSpecificationsAsync(CarSpecParams carParams);
        Task<IReadOnlyList<Make>> GetMakesAsync(int? brandId, int? makeId, int? modelId);
   
        Task<IReadOnlyList<Model>> GetModelsAsync(int? brandId, int? makeId, int? modelId);
        Task<IReadOnlyList<ModelVariant>> GetModelVariantsAsync(int? brandId, int? makeId, int? modelId);
        Task<IReadOnlyList<CarCondition>> GetCarConditionsAsync(int? brandId, int? makeId, int? modelId);


        Task<bool> IsReservedAsync(int carId);
        Task<bool> IsRentedAsync(int id);
    }
}
