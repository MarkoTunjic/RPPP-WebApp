using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable
{
    /// <summary>
    /// Lookup controller prilagođen za jTable
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class LookupController : ControllerBase
    {
        private readonly RPPP02Context ctx;

        public LookupController(RPPP02Context ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet]
        [HttpPost]
        public async Task<OptionsResult> PodrucjaRada()
        {
            var options = await ctx.PlanOdrzavanja
                                   .OrderBy(p => p.DatumOdrzavanja)
                                   .Select(d => new TextValue
                                   {
                                       DisplayText = d.IdTimZaOdrzavanjeNavigation.NazivTima.ToString(),
                                       Value = d.Id.ToString()
                                   })
                                   .ToListAsync();
            return new OptionsResult(options);
        }

        [HttpGet]
        [HttpPost]
        public async Task<OptionsResult> Uredaji()
        {
            var options = await ctx.Uredaj
                                   .OrderBy(u => u.Naziv)
                                   .Select(u => new TextValue
                                   {
                                       DisplayText = u.IdOprema.ToString(),
                                       Value = u.Id.ToString()
                                   })
                                   .ToListAsync();
            return new OptionsResult(options);
        }
    }
}
