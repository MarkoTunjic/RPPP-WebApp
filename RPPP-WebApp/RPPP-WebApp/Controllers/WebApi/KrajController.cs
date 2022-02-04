using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Models;
using WebServices.Util.ExceptionFilters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class KrajController : ControllerBase, ICustomController<int, KrajViewModel>
    {
        private readonly RPPP02Context ctx;
        ILogger<KrajController> logger;
        private static Dictionary<string, Expression<Func<KrajSmjene, object>>> orderSelectors = new Dictionary<string, Expression<Func<KrajSmjene, object>>>
        {
            [nameof(KrajViewModel.Id).ToLower()] = k => k.Id,
            [nameof(KrajViewModel.VrijemeKrajaSmjene).ToLower()] = k => k.VrijemeKrajaSmjene,
        };

        public KrajController(RPPP02Context ctx, ILogger<KrajController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }


        [HttpGet("count", Name = "BrojKrajevaSmjena")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.KrajSmjene.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {

                query = query.Where(k => k.VrijemeKrajaSmjene.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet(Name = "DohvatiKrajeve")]
        public async Task<List<KrajViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.KrajSmjene.AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            {
                query = query.Where(k => k.VrijemeKrajaSmjene.Contains(loadParams.Filter));
            }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var result = await query.Select(k => new KrajViewModel
            {
                VrijemeKrajaSmjene = k.VrijemeKrajaSmjene,
                Id = k.Id,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id}", Name = "DohvatiKraj")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KrajViewModel>> Get(int id)
        {
            var kraj = await ctx.KrajSmjene
                                  .Where(k => k.Id == id)
                                  .FirstOrDefaultAsync();
            if (kraj == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                var result = new KrajViewModel
                {
                    Id = kraj.Id,
                    VrijemeKrajaSmjene = kraj.VrijemeKrajaSmjene
                };
                return result;
            }
        }

        [HttpDelete("{id}", Name = "ObrisiKraj")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var kraj = await ctx.KrajSmjene.FindAsync(id);
            if (kraj == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(kraj);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno obrisan kraj smjene. Id=" + id);
                return NoContent();
            };
        }

        [HttpPut("{id}", Name = "AzurirajKraj")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] KrajViewModel model)
        {
            if (model.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
            }
            else
            {
                var kraj = await ctx.KrajSmjene.FindAsync(id);
                if (kraj == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                Console.WriteLine(model.VrijemeKrajaSmjene);
                kraj.VrijemeKrajaSmjene = model.VrijemeKrajaSmjene;

                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno azuriran kraj smjene. Id=" + id);
                return NoContent();
            }
        }

        [HttpPost(Name = "DodajKraj")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] KrajViewModel model)
        {
            KrajSmjene kraj = new KrajSmjene
            {
                VrijemeKrajaSmjene = model.VrijemeKrajaSmjene
            };
            ctx.Add(kraj);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Uspjesno dodano novi kraj smjene. Id=" + kraj.Id);
            var addedItem = await Get(kraj.Id);

            return CreatedAtAction(nameof(Get), new { id = kraj.Id }, addedItem.Value);
        }
    }
}
