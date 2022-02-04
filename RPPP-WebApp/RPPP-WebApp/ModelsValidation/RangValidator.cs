using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class RangValidator : AbstractValidator<Rang>
    {
        public RangValidator()
        {
            RuleFor(r => r.ImeRanga).NotEmpty().WithMessage("Obavezno odabrati naziv ranga")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Samo slova i razmaci su dozvoljeni");
        }
    }
}