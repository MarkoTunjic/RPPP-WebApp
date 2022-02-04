using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class KriticnostViewModel
    {
        public List<Kriticnost> Kriticnosti { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
