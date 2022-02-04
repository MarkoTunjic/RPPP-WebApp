using RPPP_WebApp.Models;
using System;
using System.Linq;

namespace RPPP_WebApp.Extensions.Selectors
{
    public static class LogsSort
    {
        public static IQueryable<SystemLogging> ApplySort(this IQueryable<SystemLogging> query, int sort, bool ascending)
        {
            System.Linq.Expressions.Expression<Func<SystemLogging, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = l => l.EnteredDate;
                    break;
                case 2:
                    orderSelector = l => l.LogDate;
                    break;
                case 3:
                    orderSelector = l => l.LogLevel;
                    break;
                case 4:
                    orderSelector = l => l.LogLogger;
                    break;
                case 5:
                    orderSelector = l => l.LogMessage;
                    break;
                case 6:
                    orderSelector = l => l.LogException;
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
