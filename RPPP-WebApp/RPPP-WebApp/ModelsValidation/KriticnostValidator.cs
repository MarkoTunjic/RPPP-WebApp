using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class KriticnostValidator : AbstractValidator<Kriticnost>
    {
        public KriticnostValidator()
        {
            RuleFor(kriticnost => kriticnost.StupanjKriticnosti).NotEmpty().WithMessage("Obavezno unijeti stupanj kritičnosti")
                .Matches(@"^[a-zA-Z ]*$").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
