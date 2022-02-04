using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class RadniNalogViewModel
    {
        public int Id { get; set; }
        [Display(Name = "SLA: ", Prompt = "Odredite Service Level Agreement (SLA)")]
        public DateTime Sla { get; set; }
        [Display(Name = "Trajanje: ", Prompt = "Unesite trajanje rada u satima ili ostavite prazno ukoliko se rad još izvodi")]
        public int Trajanje { get; set; }
        [Display(Name = "Tip rada:", Prompt = "Odaberite tip rada")]
        public string TipRada { get; set; }
        [Display(Name = "Trag kvara: ", Prompt = "Unesite trag kvara")]
        public string TragKvara { get; set; }
        [Display(Name = "Pocetak rada: ", Prompt = "Unesite početak rada")]
        public DateTime PocetakRada { get; set; }
        [Display(Name = "Kontrolor: ", Prompt = "Unesite ime kontrolora koji je otvorio nalog")]
        public string Kontrolor { get; set; }
        [Display(Name = "Lokacija: ", Prompt = "Odaberite lokaciju")]
        public string Lokacija { get; set; }
        [Display(Name = "Kvar: ", Prompt = "Unesite razlog kvara")]
        public string Kvar { get; set; }
        [Display(Name = "Status: ", Prompt = "Odaberite status naloga")]
        public string Status { get; set; }
        [Display(Name = "Stupanj prioriteta: ", Prompt = "Odaberite prioritet naloga")]
        public string StupanjPrioriteta { get; set; }
        [Display(Name = "Voditelj Smjene: ", Prompt = "Odaberite voditelja smjene")]
        public string VoditeljSmjene { get; set; }
        [Display(Name = "Stupanj prioriteta: ", Prompt = "Odaberite prioritet naloga")]
        public int IdStupanjPrioriteta { get; set; }
        [Display(Name = "Kontrolor: ", Prompt = "Unesite ime kontrolora koji je otvorio nalog")]
        public int IdKontrolor { get; set; }
        public int IdVoditeljSmjene { get; set; }
        [Display(Name = "Lokacija: ", Prompt = "Odaberite lokaciju")]
        public int IdLokacija { get; set; }
        [Display(Name = "Kvar: ", Prompt = "Unesite razlog kvara")]
        public int IdKvar { get; set; }
        [Display(Name = "Status: ", Prompt = "Odaberite status naloga")]
        public int IdStatus { get; set; }
        public virtual List<RadniListViewModel> RadniListovi { get; set; }

        public RadniNalogViewModel()
        {
            this.RadniListovi = new List<RadniListViewModel>();
        }
    }
}
