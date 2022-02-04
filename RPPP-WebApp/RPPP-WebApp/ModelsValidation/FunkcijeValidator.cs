using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class FunckijeValidator : AbstractValidator<Funkcije>
    {
        public FunckijeValidator()
        {
            RuleFor(funk => funk.Naziv).NotEmpty().WithMessage("Obavezno unijeti naziv nove funkcije")
                .Matches(@"^[a-zA-Z ]*$").WithMessage("Samo slova i razmaci dozvoljeni");

            RuleFor(funk => funk.IdPodsustav).NotEmpty().WithMessage("Obavezno odabrati tip podsustava!");

            RuleFor(funk => funk.Kategorija).NotEmpty().WithMessage("Obavezno unijeti kategoriju funkcije")
                .Matches(@"^[a-zA-Z ]*$").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
