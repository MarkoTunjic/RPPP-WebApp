using System.Collections.Generic;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class OpremaListViewModel
    {
        public List<OpremaViewModel> OpremaList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}