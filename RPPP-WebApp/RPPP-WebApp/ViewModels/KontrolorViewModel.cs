using System;

namespace RPPP_WebApp.ViewModels
{
    public class KontrolorViewModel
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Oib { get; set; }
        public DateTime DatumZaposlenja { get; set; }
        public DateTime? ZaposlenDo { get; set; }
        public string Lozinka { get; set; }
        public string KorisnickoIme { get; set; }
        public string ImeRanga { get; set; }
        public string PocetakSmjene { get; set; }
        public int IdRang { get; set; }
    }
}