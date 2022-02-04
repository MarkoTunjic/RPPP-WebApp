using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class PrioritetViewModel : Controller
    {
        public List<Prioritet> Prioriteti { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
