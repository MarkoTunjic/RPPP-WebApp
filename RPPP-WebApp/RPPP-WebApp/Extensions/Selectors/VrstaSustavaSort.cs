using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class VrstaSustavaSort
    {
        public static IQueryable<VrstaSustava> ApplySort(this IQueryable<VrstaSustava> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<VrstaSustava, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = s => s.NazivVrsteSustava;
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
