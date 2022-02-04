using System;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class RadnikViewModel
    {
        public int Id { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        public string Certifikat { get; set; }

        public DateTime? IstekCertifikata { get; set; }

        public int Dezuran { get; set; }
        public int IdStrucnaSprema { get; set; }
        public string RazinaStrucneSpreme { get; set; }
    }
}
