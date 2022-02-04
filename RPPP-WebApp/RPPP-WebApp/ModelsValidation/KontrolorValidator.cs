using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class KontrolorValidator : AbstractValidator<Kontrolor>
    {
        public KontrolorValidator()
        {
            RuleFor(k => k.Ime).NotEmpty().WithMessage("Obavezno navesti ime kontrolora!");
            RuleFor(k => k.Prezime).NotEmpty().WithMessage("Obavezno navesti prezime kontrolora!");
            RuleFor(k => k.Oib).NotEmpty().WithMessage("Obavezno unijeti OIB!");
            RuleFor(k => k.DatumZaposlenja).NotEmpty().WithMessage("Obavezno navesti datum zaposlenja");
            RuleFor(k => k.KorisnickoIme).NotEmpty().WithMessage("Obavezno odabrati korisničko ime!");
            RuleFor(k => k.Lozinka).NotEmpty().WithMessage("Obavezno unijeti lozinku!");
            RuleFor(k => k.IdSmjena).NotEmpty().WithMessage("Obavezno unijeti ID smjene!");
            RuleFor(k => k.IdRang).NotEmpty().WithMessage("Obavezno unijeti ID ranga!");
        }
    }
}