﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public interface ISpecification
    {
        Expression<Func<Car, bool>> Criteria { get; }
        List<Expression<Func<Car, object>>> Includes { get; }
       

    }
}
