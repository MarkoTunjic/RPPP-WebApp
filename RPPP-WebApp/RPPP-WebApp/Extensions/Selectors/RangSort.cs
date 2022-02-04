using System;
using System.Linq;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class RangSort
    {
            public static IQueryable<Rang> ApplySort(this IQueryable<Rang> query, int sort, bool ascending)
            {
                System.Linq.Expressions.Expression<Func<Rang, object>> orderSelector = null;
                switch (sort)
                {
                    case 1:
                        orderSelector = r => r.ImeRanga;
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