using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class StanjeSort
    {
        public static IQueryable<Stanje> ApplySort(this IQueryable<Stanje> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Stanje, object>> orderSelector = null;
            switch(sort)
            {
                case 1:
                    orderSelector = s => s.TipStanja;
                    break;
            }
            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }
            return query;
        }
    }
}
