using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class PlanoviOdrzavanjaViewModel
    {
        public IEnumerable<PlanOdrzavanjaViewModel> PlanoviOdrzavanja { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
