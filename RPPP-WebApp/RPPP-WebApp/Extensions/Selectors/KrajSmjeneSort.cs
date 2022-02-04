using System;
using System.Linq;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class KrajSmjeneSort
    {
        public static IQueryable<KrajSmjene> ApplySort(this IQueryable<KrajSmjene> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<KrajSmjene, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = k => k.VrijemeKrajaSmjene;
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