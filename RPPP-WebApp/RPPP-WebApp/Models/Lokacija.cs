﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Lokacija
    {
        public Lokacija()
        {
            PlaniranZa = new HashSet<PlaniranZa>();
            Podsustav = new HashSet<Podsustav>();
            RadniNalog = new HashSet<RadniNalog>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }
        public string PostanskiBroj { get; set; }
        public string KontaktTelefon { get; set; }

        public virtual ICollection<PlaniranZa> PlaniranZa { get; set; }
        public virtual ICollection<Podsustav> Podsustav { get; set; }
        public virtual ICollection<RadniNalog> RadniNalog { get; set; }
    }
}