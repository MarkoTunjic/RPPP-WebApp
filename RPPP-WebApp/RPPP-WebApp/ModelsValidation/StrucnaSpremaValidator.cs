using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class StrucnaSpremaValidator : AbstractValidator<StrucnaSprema>
    {
        public StrucnaSpremaValidator()
        {
            RuleFor(ss => ss.RazinaStrucneSpreme).NotEmpty().WithMessage("Razina stručne spreme ne smije biti prazna")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
