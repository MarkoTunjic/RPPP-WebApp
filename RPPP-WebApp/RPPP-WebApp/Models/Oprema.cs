﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Oprema
    {
        public Oprema()
        {
            Uredaj = new HashSet<Uredaj>();
        }

        public int Redundantnost { get; set; }
        public double Budzet { get; set; }
        public DateTime DatumPustanjaUPogon { get; set; }
        public int Id { get; set; }
        public int IdPodsustav { get; set; }
        public int IdTipOpreme { get; set; }

        public virtual Podsustav IdPodsustavNavigation { get; set; }
        public virtual TipOpreme IdTipOpremeNavigation { get; set; }
        public virtual ICollection<Uredaj> Uredaj { get; set; }
    }
}