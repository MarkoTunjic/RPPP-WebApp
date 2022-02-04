using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPPP_WebApp.Models;
using FluentValidation;

namespace RPPP_WebApp.ModelsValidation
{
    public class StanjeOpremeValidator : AbstractValidator<Stanje>
    {
        public StanjeOpremeValidator()
        {
            RuleFor(s => s.TipStanja).NotEmpty().WithMessage("Tip stanja ne smije biti prazan")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
