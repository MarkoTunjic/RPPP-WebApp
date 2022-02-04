using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class KrajSmjeneValidator : AbstractValidator<KrajSmjene>
    {
        public KrajSmjeneValidator()
        {
            RuleFor(k => k.VrijemeKrajaSmjene).NotEmpty().WithMessage("Obavezno odabrati vrijeme završetka smjene")
                .Length(2).WithMessage("Duljina mora biti 2")
                .Matches(@"[0-1][0-9]|[2][0-3]").WithMessage("Samo brojevi od 00-23 su dozvoljeni");
        }
    }
}