using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class TimoviZaOdrzavanjeViewModel
    {
        public List<TimZaOdrzavanjeViewModel> Timovi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
