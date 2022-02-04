using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class RadniListSort
    {
        public static IQueryable<RadniList> ApplySort(this IQueryable<RadniList> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<RadniList, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = r => r.IdRadniNalogNavigation.TipRada;
                    break;
                case 2:
                    orderSelector = r => r.IdTimZaOdrzavanjeNavigation.NazivTima;
                    break;
                case 3:
                    orderSelector = r => r.IdStatusNavigation.NazivStatusa;
                    break;
                case 4:
                    orderSelector = r => r.IdUredajNavigation.Naziv;
                    break;
                case 5:
                    orderSelector = r => r.PocetakRada;
                    break;
                case 6:
                    orderSelector = r => r.TrajanjeRada;
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
