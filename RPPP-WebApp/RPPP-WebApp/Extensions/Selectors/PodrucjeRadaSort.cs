using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PodrucjeRadaSort
    {
        public static IQueryable<PodrucjeRada> ApplySort(this IQueryable<PodrucjeRada> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<PodrucjeRada, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = p => p.VrstaPodrucjaRada;
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
