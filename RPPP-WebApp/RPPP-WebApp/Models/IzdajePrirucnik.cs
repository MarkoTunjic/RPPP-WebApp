﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class IzdajePrirucnik
    {
        public int IdNadleznoTijelo { get; set; }
        public int IdPrirucnik { get; set; }

        public virtual NadleznoTijelo IdNadleznoTijeloNavigation { get; set; }
        public virtual Prirucnik IdPrirucnikNavigation { get; set; }
    }
}