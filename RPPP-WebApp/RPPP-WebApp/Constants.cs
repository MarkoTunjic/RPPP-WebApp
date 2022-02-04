using System.Collections.Generic;

namespace RPPP_WebApp
{
    public class Constants
    {
        public static string Message
        {
            get { return "Message"; }
        }

        public static string ErrorOccurred
        {
            get { return "ErrorOccurred"; }
        }

        public static Dictionary<string, Dictionary<string, string>> StudentTables
        {
            get
            {
                return new Dictionary<string, Dictionary<string, string>>()
                {
                    { "Zdravko", new Dictionary<string, string>(){
                     // key=display name, value=ime_tablice
                        { "Radni nalog (STP)", "RadniNalog" },
                        { "Radni list (JTP)", "RadniList" },
                        { "Prioritet radnog naloga (JTP)", "Prioritet" },
                        { "Status radnog lista (JTP)", "Status" },
                        {"Izvješća","RadniNalogReport" },
                        {"Status crud","StatusCrud" }
                    }
                    },
                    {
                    "Marko", new Dictionary<string, string>(){
                     // key=display name, value=ime_tablice
                        { "Tim za održavanje (STP)", "TimZaOdrzavanje" },
                        { "Plan održavanja (JTP)", "PlanOdrzavanja" },
                        { "Područje rada (JTP)", "PodrucjeRada" },
                        { "Stručna sprema (JTP)", "StrucnaSprema" },
                        {"Izvješća","TimZaOdrzavanjeReport" },
                        {"Podrucja rada crud","PodrucjeRadaCrud" }
                    }
                    },
                    {
                    "Fran", new Dictionary<string, string>(){
                     // key=display name, value=ime_tablice
                        { "Smjena (STP)", "Smjena" },
                        { "Kontrolor (JTP)", "Kontrolor"},
                        { "Kraj smjene (JTP)", "KrajSmjene" },
                        { "Rang kontrolora (JTP)", "Rang" },
                        { "Izvješća", "SmjenaReport" },
                        { "Kraj smjene CRUD", "KrajSmjeneCrud" }
                    }
                    },
                    {
                    "Blaž", new Dictionary<string, string>(){
                     // key=display name, value=ime_tablice
                        { "Sustav (STP)", "Sustav" },
                        { "Vrsta sustava (JTP)", "VrstaSustava" },
                        { "Kritičnost (JTP)", "Kriticnost" },
                        { "Funkcije (JTP)", "Funkcije" },
                        { "Izvješća" , "SustavReport" },
                        { "Funkcije sustava CRUD", "KriticnostCrud" }
                    }
                    },
                    {
                    "Nina", new Dictionary<string, string>(){
                     // key=display name, value=ime_tablice
                        { "Oprema (STP)", "Oprema" },
                        { "Tip opreme (JTP)", "TipOpreme" },
                        { "Stanje opreme (JTP)", "StanjeOpreme" },
                        {"Izvješća","OpremaReport" },
                        {"Tip opreme crud","TipOpremeCrud" }
                    }
                    },
                };
            }
        }
    }
}
