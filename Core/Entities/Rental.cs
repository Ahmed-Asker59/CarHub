using Core.Entities.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Rental : OrderBaseEntity
    {
        public Rental()
        {
            EndDate = DateTime.Now.AddDays((double)RentalDays);

        }
        public int RentalDays { get; set; }

        public DateTime? ActualReturnDate { get; set; }
        public int DelayInDays { 
            get{
                var delay = 0;
               
                if(ActualReturnDate.HasValue && ActualReturnDate > EndDate)
                    delay = (int)(ActualReturnDate.Value - EndDate).TotalDays;

                else if (!ActualReturnDate.HasValue && DateTime.Today > EndDate)
                     delay = (int)(DateTime.Today - EndDate).TotalDays;

                return delay;

             }
        }
        //public decimal LateFeePerDay
        //{
        //    get { return CalcLateFeePerDay(); }
        //    set { value = CalcLateFeePerDay(); }
        //}

        public decimal RentalPrice { get; set; }
        public decimal TotalRentalPrice
        {
            get { return CalcTotalRentalPrice(); }           
        }

        //private decimal CalcLateFeePerDay()
        //{
        //    return Car.Price * CarServicesPrices.LateFeeRatioPerDay;
        //}
        private decimal CalcTotalRentalPrice()
        {
            decimal totalPrice = RentalPrice;

            if (ActualReturnDate.HasValue && ActualReturnDate > EndDate)
            {
                TimeSpan ReturnDelay = ActualReturnDate.Value - EndDate;

                int daysLate = ReturnDelay.Days;
                if (daysLate > 0)
                {
                    totalPrice += (daysLate * CarServicesPrices.LateFeeRatioPerDay);
                }
            }

            return totalPrice;

        }
    }
}