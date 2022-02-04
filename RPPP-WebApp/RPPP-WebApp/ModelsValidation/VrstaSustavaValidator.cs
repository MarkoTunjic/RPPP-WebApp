using FluentValidation;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.ModelsValidation
{
    public class VrstaSustavaValidator : AbstractValidator<VrstaSustava>
    {
        public VrstaSustavaValidator()
        {
            RuleFor(vrSustav => vrSustav.NazivVrsteSustava).NotEmpty().WithMessage("Obavezno unijeti naziv vrste sustava")
                .Matches(@"^[a-zA-Z ]*$").WithMessage("Samo slova i razmaci dozvoljeni");
        }
    }
}
