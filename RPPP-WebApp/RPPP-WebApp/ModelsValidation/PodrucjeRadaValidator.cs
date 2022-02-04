using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class PodrucjeRadaValidator : AbstractValidator<PodrucjeRada>
    {
        public PodrucjeRadaValidator()
        {
            RuleFor(p => p.VrstaPodrucjaRada).NotEmpty().WithMessage("Obavezno unijeti nesto")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
