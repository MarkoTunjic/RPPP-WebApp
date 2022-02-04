using System.Collections.Generic;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class RangViewModel
    {
        public List<Rang> Rangovi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}