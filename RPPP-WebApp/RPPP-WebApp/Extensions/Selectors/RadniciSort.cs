using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class RadniciSort
    {
        public static IQueryable<Radnik> ApplySort(this IQueryable<Radnik> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Radnik, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = t => t.Ime;
                    break;
                case 2:
                    orderSelector = t => t.Prezime;
                    break;
                case 3:
                    orderSelector = t => t.IdStrucnaSpremaNavigation.RazinaStrucneSpreme;
                    break;
                case 4:
                    orderSelector = t => t.Certifikat;
                    break;
                case 5:
                    orderSelector = t => t.IstekCertifikata;
                    break;
                case 6:
                    orderSelector = t => t.Dezuran;
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
