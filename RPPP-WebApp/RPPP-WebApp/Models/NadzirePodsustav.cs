﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class NadzirePodsustav
    {
        public int IdPodsustav { get; set; }
        public int IdKontrolor { get; set; }

        public virtual Kontrolor IdKontrolorNavigation { get; set; }
        public virtual Podsustav IdPodsustavNavigation { get; set; }
    }
}