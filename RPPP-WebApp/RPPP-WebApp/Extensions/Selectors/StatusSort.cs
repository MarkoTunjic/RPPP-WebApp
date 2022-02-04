
using RPPP_WebApp.Models;
using System;

using System.Linq;


namespace RPPP_WebApp.Extensions.Selectors
{
    public static class StatusSort
    {
        public static IQueryable<Status> ApplySort(this IQueryable<Status> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Status, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.NazivStatusa;
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
