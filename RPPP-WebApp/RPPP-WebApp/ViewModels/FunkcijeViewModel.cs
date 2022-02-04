using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class FunkcijeViewModel
    {
        public List<FunkcijaViewModel> Funkcije { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
