using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class SmjenaViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Početak smjene: ", Prompt = "Unesite vrijeme početka smjene")]
        public string PocetakSmjene { get; set; }

        [Display(Name = "Platni faktor: ", Prompt = "Unesite platni faktor")]
        public double PlatniFaktor { get; set; }

        [Display(Name = "Kraj smjene: ", Prompt = "Unesite vrijeme kraja smjene")]
        public string VrijemeKrajaSmjene { get; set; }

        [Display(Name = "Vrijeme kraja smjene: ", Prompt = "Unesite ID kraja smjene")]
        public int IdKrajSmjene { get; set; }

        public List<KontrolorViewModel> Kontrolori { get; set; }
            
        public SmjenaViewModel()
        {
            this.Kontrolori = new List<KontrolorViewModel>();
        }
    }
}