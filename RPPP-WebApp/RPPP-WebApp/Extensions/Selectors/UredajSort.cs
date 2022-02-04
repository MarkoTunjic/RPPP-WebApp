using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class UredajSort
    {
        public static IQueryable<Uredaj> ApplySort(this IQueryable<Uredaj> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Uredaj, object>> orderSelector = null;
            switch(sort)
            {
                case 1:
                    orderSelector = t => t.Naziv;
                    break;
                case 2:
                    orderSelector = t => t.Proizvodac;
                    break;
                case 3:
                    orderSelector = t => t.GodinaProizvodnje;
                    break;
                case 4:
                    orderSelector = t => t.IdOprema;
                    break;
                case 5:
                    orderSelector = t => t.IdStanjeNavigation.TipStanja;
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
