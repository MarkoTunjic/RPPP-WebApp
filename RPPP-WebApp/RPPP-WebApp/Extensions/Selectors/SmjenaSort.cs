using System;
using System.Linq;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class SmjenaSort
    {
        public static IQueryable<Smjena> ApplySort(this IQueryable<Smjena> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Smjena, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.PocetakSmjene;
                    break;
                case 2:
                    orderSelector = s => s.PlatniFaktor;
                    break;
                case 3:
                    orderSelector = s => s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene;
                    break;
                case 4:
                    orderSelector = s => s.Kontrolor.Count;
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