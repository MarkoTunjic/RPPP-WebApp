using FluentValidation;
using RPPP_WebApp.Models;


namespace RPPP_WebApp.ModelsValidation
{
    public class StatusValidator : AbstractValidator<Status>
    {
        public StatusValidator()
        {
            RuleFor(s => s.NazivStatusa).NotEmpty().WithMessage("Obavezno unijeti naziv statusa");
            // .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)")
            //.WithMessage("Status smije sadržavati samo slova i razmake i ne smije imati razmak na kraju!");
        }
    }
}
