using System;

namespace RPPP_WebApp.ViewModels
{
    public class UredajViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Proizvodac { get; set; }
        public string GodinaProizvodnje { get; set; }
        public int IdOprema { get; set; }
        public int IdStanja { get; set; }
        public string TipStanja { get; set; }
    }
}