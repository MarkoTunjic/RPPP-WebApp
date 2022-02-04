using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class PodsustaviSort
    {
        public static IQueryable<Podsustav> ApplySort(this IQueryable<Podsustav> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Podsustav, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = p => p.Naziv;
                    break; 
                case 2:
                    orderSelector = p => p.Opis;
                    break;
                case 3:
                    orderSelector = p => p.Osjetljivost;
                    break;
                case 4:
                    orderSelector = p => p.IdLokacijaNavigation.Naziv;
                    break;
                case 5:
                    orderSelector = p => p.IdKriticnostNavigation.StupanjKriticnosti;
                    break;
                case 6:
                    orderSelector = p => p.UcestalostOdrzavanja;
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
    //"Naziv", "Opis", "Osjetljivost", "Lokacija", "Stupanj kritičnosti", "Učestalost održavanja"
    
}
