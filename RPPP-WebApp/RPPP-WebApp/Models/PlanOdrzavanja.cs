// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    public partial class PlanOdrzavanja
    {
        public PlanOdrzavanja()
        {
            PlaniranZa = new HashSet<PlaniranZa>();
        }
        [Display(Name = "Razina stručnosti: ", Prompt = "Unesite razinu stručnosti")]
        public int RazinaStrucnosti { get; set; }
        [Display(Name = "Datum održavanja: ", Prompt = "Unesite predviđeni datum održavanja")]
        public DateTime DatumOdrzavanja { get; set; }
        public int Id { get; set; }
        [Display(Name = "Naziv podsustava: ", Prompt = "Unesite naziv podsustava")]
        public int IdPodsustav { get; set; }
        [Display(Name = "Naziv tima: ", Prompt = "Unesite naziv tima")]
        public int IdTimZaOdrzavanje { get; set; }

        public virtual Podsustav IdPodsustavNavigation { get; set; }
        public virtual TimZaOdrzavanje IdTimZaOdrzavanjeNavigation { get; set; }
        public virtual ICollection<PlaniranZa> PlaniranZa { get; set; }
    }
}