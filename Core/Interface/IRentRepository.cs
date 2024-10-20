﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IRentRepository
    {
        Task<bool> RentCar(int clientId, int carId, int rentDays);
        Task<IReadOnlyList<Client>> GetClientsToAlert();
        Task<decimal> CalcLateFeePerDay(int carId);
    }
}