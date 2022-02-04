using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class RadniNaloziViewModel
    {
        public List<RadniNalogViewModel> RadniNalozi { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
