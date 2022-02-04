using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class PodrucjeRadaViewModel
    {
        public List<PodrucjeRada> Podrucja { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
