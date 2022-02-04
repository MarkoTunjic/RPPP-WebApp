using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class RadniNalogSort
    {
        public static IQueryable<RadniNalog> ApplySort(this IQueryable<RadniNalog> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<RadniNalog, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = r => r.IdKvarNavigation.Opis;
                    break;
                case 2:
                    orderSelector = r => r.IdLokacijaNavigation.Naziv;
                    break;
                case 3:
                    orderSelector = r => r.IdStatusNavigation.NazivStatusa;
                    break;
                case 4:
                    orderSelector = r => r.IdStupanjPrioritetaNavigation.StupanjPrioriteta;
                    break;
                case 5:
                    orderSelector = r => r.PocetakRada;
                    break;
                case 6:
                    orderSelector = r => r.RadniList.Count;
                    break;
                case 7:
                    orderSelector = r => r.Sla;
                    break;
                case 8:
                    orderSelector = r => r.TipRada;
                    break;
                case 9:
                    orderSelector = r => r.TragKvara;
                    break;
                case 10:
                    orderSelector = r => r.Trajanje;
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
