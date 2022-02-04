using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class TimZaOdrzavanjeSort
    {
        public static IQueryable<TimZaOdrzavanje> ApplySort(this IQueryable<TimZaOdrzavanje> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<TimZaOdrzavanje, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = t => t.NazivTima;
                    break;
                case 2:
                    orderSelector = t => t.DatumOsnivanja;
                    break;
                case 3:
                    orderSelector = t => t.IdPodrucjeRadaNavigation.VrstaPodrucjaRada;
                    break;
                case 4:
                    orderSelector = t => t.Satnica;
                    break;
                case 5:
                    orderSelector = t => t.Radnik.Count;
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
