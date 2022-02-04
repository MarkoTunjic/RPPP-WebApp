using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class KriticnostSort
    {
        public static IQueryable<Kriticnost> ApplySort(this IQueryable<Kriticnost> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Kriticnost, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.StupanjKriticnosti;
                    break;
            }
            if (orderSelector != null)
            {
                query = ascending ?
                    query.OrderBy(orderSelector) :
                    query.OrderByDescending(orderSelector);
            }

            return query;
        }
    }
}
