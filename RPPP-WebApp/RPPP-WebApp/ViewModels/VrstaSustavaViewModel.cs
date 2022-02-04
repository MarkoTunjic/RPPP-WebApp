using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
    public class VrstaSustavaViewModel
    {
        public List<VrstaSustava> VrsteSustava { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
