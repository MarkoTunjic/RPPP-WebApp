using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class RadniListViewModel // : Controller
    {
        public DateTime PocetakRada { get; set; }

        public int TrajanjeRada { get; set; }

        public string OpisRada { get; set; }

        public string NazivUredaja { get; set; }
        public int IdUredaj { get; set; }
        public int IdTimZaOdrzavanje { get; set; }
        public int IdStatus { get; set; }
        public string TimZaOdrzavanje { get; set; }

        public string Status { get; set; }
        public int Id { get; set; }
        public int TrajanjeRN { get; set; }
    }
}
