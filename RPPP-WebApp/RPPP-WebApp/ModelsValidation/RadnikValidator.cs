using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class RadnikValidator : AbstractValidator<Radnik>
    {
        public RadnikValidator()
        {
            RuleFor(r => r.IdStrucnaSprema).NotEmpty().WithMessage("Obavezno unijeti strucnu spremu")
                .GreaterThan(0).WithMessage("Obavezno unijeti strucnu spremu");
            RuleFor(r => r.Ime).NotEmpty().WithMessage("Obavezno unijeti strucnu ime");
            RuleFor(r => r.Prezime).NotEmpty().WithMessage("Obavezno unijeti prezime");
            RuleFor(r => r.Dezuran).NotEmpty().WithMessage("Obavezno unijeti je li radnik dezuran");
        }
    }
}
