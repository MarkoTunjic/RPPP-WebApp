using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
    public class OpremaViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Redundantnost", Prompt = "Unesite redundantnost opreme")]
        public int Redundantnost { get; set; }

        [Display(Name = "Budzet", Prompt = "Unesite budzet opreme")]
        public double Budzet { get; set; }

        [Display(Name = "Datum pustanja u pogon", Prompt = "Unesite datum pustanja opreme u pogon")]
        public DateTime DatumPustanjaUPogon { get; set; }

        [Display(Name = "Tip opreme", Prompt = "Unesite tip opreme")]
        public int IdOpreme { get; set; }
        public string TipOpreme { get; set; }

        [Display(Name = "Naziv podsustava", Prompt = "Unesite naziv podsustava")]
        public int IdPodsustava { get; set; }
        public string PodsustavNaziv { get; set; }

        public ICollection<UredajViewModel> Uredaji { get; set; }

        public OpremaViewModel()
        {
            this.Uredaji = new List<UredajViewModel>();
        }
    }
}