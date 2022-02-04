using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class TipOpremeSort
    {
        public static IQueryable<TipOpreme> ApplySort(this IQueryable<TipOpreme> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<TipOpreme, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = t => t.TipOpreme1;
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
