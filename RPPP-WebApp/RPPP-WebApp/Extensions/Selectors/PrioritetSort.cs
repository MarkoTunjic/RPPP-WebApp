using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PrioritetSort
    {
        public static IQueryable<Prioritet> ApplySort(this IQueryable<Prioritet> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Prioritet, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = p => p.StupanjPrioriteta;
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
