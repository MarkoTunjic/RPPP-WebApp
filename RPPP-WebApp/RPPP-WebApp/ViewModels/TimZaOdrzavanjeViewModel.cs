using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class TimZaOdrzavanjeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Datum osnivanja: ", Prompt = "Unesite datum osnivanja")]
        public DateTime DatumOsnivanja { get; set; }

        [Display(Name = "Naziv osnivanja: ", Prompt = "Odaberite naziv tima")]
        public string NazivTima { get; set; }

        [Display(Name = "Satnica tima: ", Prompt = "Unesite satnicu tima")]
        public int Satnica { get; set; }

        [Display(Name = "Vrsta podrucja rada: ", Prompt = "Odaberite vrstu podrucja rada")]
        public int IdPodrucjaRada { get; set; }
        public string VrstaPodrucjaRada { get; set; }


        public ICollection<RadnikViewModel> Radnici { get; set; }

        public TimZaOdrzavanjeViewModel()
        {
            this.Radnici = new List<RadnikViewModel>();
        }
    }
}
