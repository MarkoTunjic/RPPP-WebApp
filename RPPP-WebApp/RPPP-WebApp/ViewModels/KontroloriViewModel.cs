using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RPPP_WebApp.ViewModels
{
    public class KontroloriViewModel : Controller
    {
        public IEnumerable<KontrolorViewModel> Kontrolori { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}