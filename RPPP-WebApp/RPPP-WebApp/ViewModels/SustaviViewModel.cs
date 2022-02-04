using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class SustaviViewModel
    {
        public List<SustavViewModel> Sustavi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
