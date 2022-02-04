using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class PlanOdrzavanjaValidator : AbstractValidator<PlanOdrzavanja>
    {
        public PlanOdrzavanjaValidator()
        {
            RuleFor(p => p.IdTimZaOdrzavanje).NotEmpty().WithMessage("Obavezno odabrati naziv tima");
            RuleFor(p => p.DatumOdrzavanja).NotEmpty().WithMessage("Obavezno unijeti datum odrzavanja");
            RuleFor(p => p.IdPodsustav).NotEmpty().WithMessage("Obavezno odabrati podsustav");
            RuleFor(p => p.RazinaStrucnosti)
                .NotEmpty().WithMessage("Obavezno unijeti razinu strucnosti")
                .GreaterThanOrEqualTo(1).WithMessage("Dozvoljen raspon od 1 do 10")
                .LessThanOrEqualTo(10).WithMessage("Dozvoljen raspon od 1 do 10");

        }

    }
}
