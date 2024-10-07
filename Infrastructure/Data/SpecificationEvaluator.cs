using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator
    {
        public static IQueryable<Car> GetQuery(IQueryable<Car> inputQuery, ISpecification specification)
        {
            var query = inputQuery;

            
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }
           
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
