﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class StrucnaSprema
    {
        public StrucnaSprema()
        {
            Radnik = new HashSet<Radnik>();
        }

        public int Id { get; set; }
        public string RazinaStrucneSpreme { get; set; }

        public virtual ICollection<Radnik> Radnik { get; set; }
    }
}