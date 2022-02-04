using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPPP_WebApp.ModelsValidation
{
    public class RadniListValidator : AbstractValidator<RadniList>
    {
        public RadniListValidator()
        {
            RuleFor(r => r.IdRadniNalog).NotEmpty().WithMessage("Obavezno odabrati tip rada za radni nalog!");
            RuleFor(r => r.IdStatus).NotEmpty().WithMessage("Obavezno unijeti status radnog lista!");
            RuleFor(r => r.IdTimZaOdrzavanje).NotEmpty().WithMessage("Obavezno odabrati tim za održavanje!");
            RuleFor(r => r.IdUredaj).NotEmpty().WithMessage("Obavezno odabrati uređaj");
            RuleFor(r => r.PocetakRada).NotEmpty().WithMessage("Obavezno unijeti datum početka rada!");
            RuleFor(r => r.TrajanjeRada).GreaterThanOrEqualTo(0).WithMessage("Trajanje obrade mora biti barem 0!");
            RuleFor(r => r.OpisRada).NotEmpty().WithMessage("Obavezno unijeti opis radova!");
        }
    }
}
