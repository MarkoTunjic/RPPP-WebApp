using RPPP_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RPPP_WebApp.Models
{
    public partial class SustavViewModel
    {
        

        public int Id { get; set; }
        public int Osjetljivost { get; set; }
        public string Opis { get; set; }
        public int IdStupanjKriticnosti { get; set; }
        public string StupanjKriticnosti { get; set; }
        public int IdVrstaSustava { get; set; }
        public string VrstaSustava { get; set; }

        public ICollection<PodsustavViewModel> Podsustavi { get; set; }
        public SustavViewModel()
        {
            this.Podsustavi = new List<PodsustavViewModel>();
        }
    }
}