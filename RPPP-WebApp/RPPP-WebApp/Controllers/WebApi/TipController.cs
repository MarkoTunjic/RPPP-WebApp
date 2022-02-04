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
    public class TipController : ControllerBase, ICustomController<int, TipViewModel>
    {
        private readonly RPPP02Context ctx;
        ILogger<TipController> logger;
        private static Dictionary<string, Expression<Func<TipOpreme, object>>> orderSelectors = new Dictionary<string, Expression<Func<TipOpreme, object>>>
        {
            [nameof(TipViewModel.Id).ToLower()] = t => t.Id,
            [nameof(TipViewModel.TipOpreme).ToLower()] = t => t.TipOpreme1,
        };

        public TipController(RPPP02Context ctx, ILogger<TipController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }


        [HttpGet("count", Name = "BrojTipova")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.TipOpreme.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {

                query = query.Where(t => t.TipOpreme1.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet(Name = "DohvatiTipove")]
        public async Task<List<TipViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.TipOpreme.AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            {
                query = query.Where(t => t.TipOpreme1.Contains(loadParams.Filter));
            }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var result = await query.Select(t => new TipViewModel
            {
                TipOpreme = t.TipOpreme1,
                Id = t.Id,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id}", Name = "DohvatiTip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TipViewModel>> Get(int id)
        {
            var tip = await ctx.TipOpreme
                                  .Where(t => t.Id == id)
                                  .FirstOrDefaultAsync();
            if (tip == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                var result = new TipViewModel
                {
                    Id = tip.Id,
                    TipOpreme = tip.TipOpreme1
                };
                return result;
            }
        }

        [HttpDelete("{id}", Name = "ObrisiTip")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var tip = await ctx.TipOpreme.FindAsync(id);
            if (tip == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(tip);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspješno obrisan tip opreme. Id=" + id);
                return NoContent();
            };
        }

        [HttpPut("{id}", Name = "AzurirajTip")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] TipViewModel model)
        {
            if (model.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
            }
            else
            {
                var tip = await ctx.TipOpreme.FindAsync(id);
                if (tip == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                tip.TipOpreme1 = model.TipOpreme;

                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspješno ažuriran tip opreme. Id=" + id);
                return NoContent();
            }
        }

        [HttpPost(Name = "DodajTip")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] TipViewModel model)
        {
            TipOpreme tip = new TipOpreme
            {
                TipOpreme1 = model.TipOpreme
            };
            ctx.Add(tip);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Uspješno dodan tip opreme. Id=" + tip.Id);
            var addedItem = await Get(tip.Id);

            return CreatedAtAction(nameof(Get), new { id = tip.Id }, addedItem.Value);
        }
    }
}
