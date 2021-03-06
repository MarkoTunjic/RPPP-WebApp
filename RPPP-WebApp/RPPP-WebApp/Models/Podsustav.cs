// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Podsustav
    {
        public Podsustav()
        {
            Funkcije = new HashSet<Funkcije>();
            NadzirePodsustav = new HashSet<NadzirePodsustav>();
            Oprema = new HashSet<Oprema>();
            PlanOdrzavanja = new HashSet<PlanOdrzavanja>();
        }

        public int UcestalostOdrzavanja { get; set; }
        public int Osjetljivost { get; set; }
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int IdSustav { get; set; }
        public int IdLokacija { get; set; }
        public int IdKriticnost { get; set; }

        public virtual Kriticnost IdKriticnostNavigation { get; set; }
        public virtual Lokacija IdLokacijaNavigation { get; set; }
        public virtual Sustav IdSustavNavigation { get; set; }
        public virtual ICollection<Funkcije> Funkcije { get; set; }
        public virtual ICollection<NadzirePodsustav> NadzirePodsustav { get; set; }
        public virtual ICollection<Oprema> Oprema { get; set; }
        public virtual ICollection<PlanOdrzavanja> PlanOdrzavanja { get; set; }
    }
}