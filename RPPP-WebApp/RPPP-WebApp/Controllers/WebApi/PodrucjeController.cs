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
    public class PodrucjeController : ControllerBase, ICustomController<int, PodrucjeViewModel>
    {
        private readonly RPPP02Context ctx;
        ILogger<PodrucjeController> logger;
        private static Dictionary<string, Expression<Func<PodrucjeRada, object>>> orderSelectors = new Dictionary<string, Expression<Func<PodrucjeRada, object>>>
        {
            [nameof(PodrucjeViewModel.Id).ToLower()] = p => p.Id,
            [nameof(PodrucjeViewModel.VrstaPodrucjaRada).ToLower()] = p => p.VrstaPodrucjaRada,
        };

        public PodrucjeController(RPPP02Context ctx, ILogger<PodrucjeController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }


        [HttpGet("count", Name = "BrojPodrucja")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.PodrucjeRada.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {

                query = query.Where(p => p.VrstaPodrucjaRada.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet(Name = "DohvatiPodrucja")]
        public async Task<List<PodrucjeViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.PodrucjeRada.AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            {
                query = query.Where(p => p.VrstaPodrucjaRada.Contains(loadParams.Filter));
            }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var result = await query.Select(p => new PodrucjeViewModel
            {
                VrstaPodrucjaRada = p.VrstaPodrucjaRada,
                Id = p.Id,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id}", Name = "DohvatiPodrucje")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PodrucjeViewModel>> Get(int id)
        {
            var podrucje = await ctx.PodrucjeRada
                                  .Where(p => p.Id == id)
                                  .FirstOrDefaultAsync();
            if (podrucje == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                var result = new PodrucjeViewModel
                {
                    Id = podrucje.Id,
                    VrstaPodrucjaRada = podrucje.VrstaPodrucjaRada
                };
                return result;
            }
        }

        [HttpDelete("{id}", Name = "ObrisiPodrucje")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var podrucje = await ctx.PodrucjeRada.FindAsync(id);
            if (podrucje == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(podrucje);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno obrisano podrucje rada. Id=" + id);
                return NoContent();
            };
        }

        [HttpPut("{id}", Name = "AzurirajPodrucje")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] PodrucjeViewModel model)
        {
            if (model.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
            }
            else
            {
                var podrucje = await ctx.PodrucjeRada.FindAsync(id);
                if (podrucje == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                podrucje.VrstaPodrucjaRada = model.VrstaPodrucjaRada;

                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno azurirano podrucje rada. Id=" + id);
                return NoContent();
            }
        }

        [HttpPost(Name = "DodajPodrucje")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] PodrucjeViewModel model)
        {
            PodrucjeRada podrucje = new PodrucjeRada
            {
                VrstaPodrucjaRada = model.VrstaPodrucjaRada
            };
            ctx.Add(podrucje);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Uspjesno dodano novo podrucje rada. Id=" + podrucje.Id);
            var addedItem = await Get(podrucje.Id);

            return CreatedAtAction(nameof(Get), new { id = podrucje.Id }, addedItem.Value);
        }
    }
}
