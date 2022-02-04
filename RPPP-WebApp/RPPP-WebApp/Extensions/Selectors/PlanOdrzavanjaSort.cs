using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PlanOdrzavanjaSort
    {
        public static IQueryable<PlanOdrzavanja> ApplySort(this IQueryable<PlanOdrzavanja> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<PlanOdrzavanja, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = p => p.IdTimZaOdrzavanjeNavigation.NazivTima;
                    break;
                case 2:
                    orderSelector = p => p.IdPodsustavNavigation.Naziv;
                    break;
                case 3:
                    orderSelector = p => p.DatumOdrzavanja;
                    break;
                case 4:
                    orderSelector = p => p.RazinaStrucnosti;
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
