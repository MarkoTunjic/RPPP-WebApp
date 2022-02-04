using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class StrucnaSpremaSort
    {
        public static IQueryable<StrucnaSprema> ApplySort(this IQueryable<StrucnaSprema> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<StrucnaSprema, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.RazinaStrucneSpreme;
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
