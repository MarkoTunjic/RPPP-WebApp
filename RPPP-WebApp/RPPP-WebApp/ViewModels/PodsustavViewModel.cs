using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ViewModels
{
    public class PodsustavViewModel
    {
        public int UcestalostOdrzavanja { get; set; }
        public int Osjetljivost { get; set; }
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int IdSustav { get; set; }

        public string OpisSustava { get; set; }
        public int IdStupanjKriticnosti { get; set; }
       
        public string StupanjKriticnosti { get; set; }
        public int IdLokacija { get; set; }
        public string NazivLokacije { get; set; }
    }
}
