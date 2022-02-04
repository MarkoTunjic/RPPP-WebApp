using System;

namespace RPPP_WebApp.ModelsPartial
{
    public class RadnikDenorm
    {
        public int IdTim { get; set; }
        public DateTime DatumOsnivanja { get; set; }
        public string NazivTima { get; set; }
        public int Satnica { get; set; }
        public string PodrucjeRada { get; set; }
        public int IdRadnik { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Certifikat { get; set; }
        public string IstekCertifikata { get; set; }

        public string UrlTima { get; set; }

    }
}
