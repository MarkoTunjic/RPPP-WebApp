using System;

namespace RPPP_WebApp.ModelsPartial
{
    public class RadniListDenorm
    {
        public int IdRadniNalog { get; set; }
        public string TipRada { get; set; }
        public string TragKvara { get; set; }
        public DateTime PocetakRada { get; set; }
        public string Kontrolor { get; set; }
        public string Status { get; set; }
        public int IdRadniList { get; set; }
        public int TrajanjeRada { get; set; }
        public string OpisRada { get; set; }
        public string NazivUredaja { get; set; }
        public string TimZaOdrzavanje { get; set; }
        public string UrlRadnogNaloga { get; set; }
    }
}
