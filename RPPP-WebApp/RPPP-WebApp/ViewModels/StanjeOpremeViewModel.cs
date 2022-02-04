using RPPP_WebApp.Models;
using System.Collections.Generic;


namespace RPPP_WebApp.ViewModels
{
    public class StanjeOpremeViewModel
    {
        public List<Stanje> Stanja { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}