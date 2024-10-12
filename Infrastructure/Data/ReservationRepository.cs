using Core.Entities.Consts;
using Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly CarContext _context;

        public ReservationRepository(CarContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateReservationAsync(int CarId,int ClientId)
        {
            var reservation = new Reservation();   
            reservation.CarId = CarId;
            reservation.ClientId = ClientId;
            reservation.ReservationFee =  _context.Cars.SingleOrDefault(s => s.Id == CarId).Price * CarServicesPrices.ReservationRatio;
            await _context.Reservations.AddAsync(reservation);
            
            return await SaveAsync();
        }

        public  async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved >0? true:false;
        }

      
    }
}
