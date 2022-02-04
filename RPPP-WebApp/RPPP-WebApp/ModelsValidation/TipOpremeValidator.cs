using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class TipOpremeValidator : AbstractValidator<TipOpreme>
    {
        public TipOpremeValidator()
        {
            RuleFor(p => p.TipOpreme1).NotEmpty().WithMessage("Obavezno unijeti naziv tipa opreme")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
