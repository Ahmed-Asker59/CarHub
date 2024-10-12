//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Core.Entities
//{
//    public class Rental : OrderBaseEntity
//    {
//        public bool IsReturned { get; set; }

//        public DateTime? ActualReturnedDate { get; set; }
//        public decimal LateFeePerDay
//        {
//            get { return CalcLateFeePerDay(); }
//        }

//        public decimal RentalPrice
//        {
//            get { return CalcRentalPrice(); }
//        }

//        public decimal TotalRentalPrice
//        {
//            get { return CalcTotalRentalPrice(); }
//        }




//        private decimal CalcLateFeePerDay()
//        {
//            return RentalPrice * Convert.ToDecimal(.1);
//        }
//        private decimal CalcRentalPrice()
//        {
//            return Car.Price * Convert.ToDecimal(.03);
//        }
//        private decimal CalcTotalRentalPrice()
//        {
//            decimal totalPrice = RentalPrice;



//            if (IsReturned && ActualReturnedDate > EndDate)
//            {
//                TimeSpan ReturnDelay = ActualReturnedDate.Value - EndDate;

//                int daysLate = ReturnDelay.Days;
//                if (daysLate > 0)
//                {
//                    totalPrice += daysLate * LateFeePerDay;
//                }
//            }

//            return totalPrice;

//        }
//    }
//}