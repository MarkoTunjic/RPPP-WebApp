using System;
using System.Linq;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class KontroloriSort
    {
        public static IQueryable<Kontrolor> ApplySort(this IQueryable<Kontrolor> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Kontrolor, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.Ime;
                    break;
                case 2:
                    orderSelector = s => s.Prezime;
                    break;
                case 3:
                    orderSelector = s => s.Oib;
                    break;
                case 4:
                    orderSelector = s => s.DatumZaposlenja;
                    break;
                case 5:
                    orderSelector = s => s.ZaposlenDo;
                    break;
                case 6:
                    orderSelector = s => s.Lozinka;
                    break;
                case 7:
                    orderSelector = s => s.KorisnickoIme;
                    break;
                case 8:
                    orderSelector = s => s.IdRangNavigation.ImeRanga;
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