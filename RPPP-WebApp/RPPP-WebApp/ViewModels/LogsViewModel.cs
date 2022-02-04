using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class LogsViewModel
    {
        public IEnumerable<LogViewModel> Logs { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
