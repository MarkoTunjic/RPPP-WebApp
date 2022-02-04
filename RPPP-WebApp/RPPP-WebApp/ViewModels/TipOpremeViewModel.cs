using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class TipOpremeViewModel
    {
        public List<TipOpreme> Tipovi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}