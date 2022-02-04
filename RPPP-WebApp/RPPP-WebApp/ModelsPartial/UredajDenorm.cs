using System;

namespace RPPP_WebApp.ModelsPartial
{
    public class UredajDenorm
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Proizvodac { get; set; }
        public string GodinaProizvodnje { get; set; }
        public int IdOprema { get; set; }
        public string TipStanja { get; set; }

        public string UrlOpreme { get; set; }

    }
}