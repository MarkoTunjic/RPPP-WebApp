using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class OpremaSort
    {
        public static IQueryable<Oprema> ApplySort(this IQueryable<Oprema> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Oprema, object>> orderSelector = null;
            switch(sort)
            {
                case 1:
                    orderSelector = t => t.Redundantnost;
                    break;
                case 2:
                    orderSelector = t => t.Budzet;
                    break;
                case 3:
                    orderSelector = t => t.DatumPustanjaUPogon;
                    break;
                case 4:
                    orderSelector = t => t.IdTipOpremeNavigation.TipOpreme1;
                    break;
                case 5:
                    orderSelector = t => t.IdPodsustavNavigation.Naziv;
                    break;
                case 6:
                    orderSelector = t => t.Uredaj.Count;
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
