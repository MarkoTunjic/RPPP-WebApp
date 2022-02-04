using System.Collections.Generic;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class KrajSmjeneViewModel
    {
        public List<KrajSmjene> KrajeviSmjena { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}