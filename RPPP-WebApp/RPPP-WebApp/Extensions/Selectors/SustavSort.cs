using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class SustavSort
    {
        public static IQueryable<Sustav> ApplySort(this IQueryable<Sustav> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<Sustav, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.Opis;
                    break;          
                case 2:             
                    orderSelector = s => s.Osjetljivost;
                    break;          
                case 3:             
                    orderSelector = s => s.IdKriticnostNavigation.StupanjKriticnosti;
                    break;          
                case 4:             
                    orderSelector = s => s.IdVrstaSustavaNavigation.NazivVrsteSustava;
                    break;
                case 5:
                    orderSelector = s => s.Podsustav.Count;
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
