using FluentValidation;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;

namespace RPPP_WebApp.ModelsValidation
{
    public class TimZaOdrzavanjeValidator : AbstractValidator<TimZaOdrzavanje>
    {
        public TimZaOdrzavanjeValidator()
        {
            RuleFor(t => t.NazivTima).NotEmpty().WithMessage("Naziv tima ne moze biti prazan!");
            RuleFor(t => t.IdPodrucjeRada).GreaterThanOrEqualTo(1).WithMessage("Obavezno odabrati podrucje rada");
            RuleFor(t => t.DatumOsnivanja).NotEmpty().WithMessage("Obavezno unijeti datum osnivanja");
            RuleFor(t => t.Satnica).NotEmpty().WithMessage("Obavezno unijeti satnicu");
        }
    }
}
