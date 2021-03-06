// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.Models
{
    public partial class RadniList
    {
        [Display(Name = "Početak rada: ", Prompt = "Unesite datum početka rada")]
        public DateTime PocetakRada { get; set; }
        [Display(Name = "Trajanje rada: ", Prompt = "Unesite trajanje rada u satima ili 0 ukoliko se rad još izvodi")]
        public int TrajanjeRada { get; set; }
        [Display(Name = "Opis rada: ", Prompt = "Unesite opis rada")]
        public string OpisRada { get; set; }
        public int Id { get; set; }
        [Display(Name = "Naziv uređaja: ", Prompt = "Odaberite povezani uređaj")]
        public int IdUredaj { get; set; }
        [Display(Name = "Naziv tima za održavanje: ", Prompt = "Unesite naziv tima za održavanje")]
        public int IdTimZaOdrzavanje { get; set; }
        [Display(Name = "Radni nalog: ", Prompt = "Unesite povezani radni nalog")]
        public int IdRadniNalog { get; set; }
        [Display(Name = "Status: ", Prompt = "Odaberite status radnog lista")]
        public int IdStatus { get; set; }

        public virtual RadniNalog IdRadniNalogNavigation { get; set; }
        public virtual Status IdStatusNavigation { get; set; }
        public virtual TimZaOdrzavanje IdTimZaOdrzavanjeNavigation { get; set; }
        public virtual Uredaj IdUredajNavigation { get; set; }
    }
}