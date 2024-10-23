using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IRentRepository
    {
        Task<Rental?> GetRentalByIdAsync(int id);
        Task<bool> ReturnRentalAsync(Rental rental);
        Task<bool> RentCar(int clientId, int carId, int rentDays);

        Task<IReadOnlyList<Rental>> GetRentalsAsync(DateTime? From,DateTime? To);
        Task<IReadOnlyList<Rental>> GetDelayedRentalsAsync();
        Task<decimal> CalcLateFeePerDay(int carId);
        Task<int> GetDelaysInDaysAsync(int rentalId);
        Task<IReadOnlyList<Client>> GetClientsToAlert();
        Task<bool> CancelRentalAsync(Rental rental);
         Task<bool> SaveAsync();





    }

}