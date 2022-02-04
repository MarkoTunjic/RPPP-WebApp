using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class RadniListoviViewModel : Controller
    {
        public IEnumerable<RadniListViewModel> RadniListovi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
