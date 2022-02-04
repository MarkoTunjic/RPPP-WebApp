using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class FunkcijaViewModel
    {
        public int Id { get; set; }
        public string Kategorija { get; set; }
        public string Naziv { get; set; }
        public string NazivPodsustav { get; set; }
        public int IdPodsustav { get; set; }
    }
}
