using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class FunkcijeSort
    {
        public static IQueryable<Funkcije> ApplySort(this IQueryable<Funkcije> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Funkcije, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = f => f.Naziv;
                    break;

                case 2:
                    orderSelector = f => f.Kategorija;
                    break;

                case 3:
                    orderSelector = f => f.IdPodsustavNavigation.Naziv;
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
