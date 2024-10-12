using Core.Entities;
using Core.Entities.Consts;

public class Reservation : OrderBaseEntity
{
    public Reservation()
    {
        ReservationFee = Car.Price * CarServicesPrices.ReservationRatio;
        EndDate = DateTime.Now.AddDays(3);
    }

    public decimal ReservationFee { get; set; }

}
