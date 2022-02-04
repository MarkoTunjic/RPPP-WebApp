using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ModelsValidation
{
    public class PrioritetValidator : AbstractValidator<Prioritet>
    {
        public PrioritetValidator()
        {
            RuleFor(p => p.StupanjPrioriteta).NotEmpty().WithMessage("Obavezno unijeti stupanj prioriteta")
                .Matches(@"(^[a-zA-Z]+(\s[a-zA-Z]+)*)").WithMessage("Stupanj prioriteta smije sadržavati samo slova i razmake i ne smije imati razmak na kraju!");
        }
    }
}
