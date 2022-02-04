using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class PlanOdrzavanjaViewModel
    {

        public string NazivTima { get; set; }

        public string NazivPodsustava { get; set; }

        public DateTime DatumOdrzavanja { get; set; }

        public int RazinaStrucnosti { get; set; }
        public int Id { get; set; }
    }
}
