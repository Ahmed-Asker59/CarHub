using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IReservationRepository
    {
        Task<bool> CreateReservationAsync(int CarId,int ClientId);
        Task<IReadOnlyList<Client>> GetClientsToAlert();
        Task<bool> SaveAsync();
        Task<IReadOnlyList<Reservation>> GetReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<bool> EndReservationAsync(Reservation reservation);
        Task<bool> CancelReservationAsync(Reservation reservation);
    }
}
