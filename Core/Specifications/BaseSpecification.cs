using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class BaseSpecification:ISpecification
    {
        public BaseSpecification(Expression<Func<Car, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<Car, bool>> Criteria { get; }
        public List<Expression<Func<Car, object>>> Includes { get; } = new List<Expression<Func<Car, object>>>();

        public Expression<Func<Car, object>> OrderBy { get; private set; }

        public Expression<Func<Car, bool>> OrderByDescending { get; private set; }

        protected void AddIncludes(Expression<Func<Car, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
 


    }
}
