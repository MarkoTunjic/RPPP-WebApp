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
    public class KriticnostJController : ControllerBase, ICustomController<int, KriticnostJViewModel>
    {
        private readonly RPPP02Context ctx;
        ILogger<KriticnostJController> logger;
        private static Dictionary<string, Expression<Func<Kriticnost, object>>> orderSelectors = new Dictionary<string, Expression<Func<Kriticnost, object>>>
        {
            [nameof(KriticnostJViewModel.Id).ToLower()] = p => p.Id,
            [nameof(KriticnostJViewModel.StupanjKriticnosti).ToLower()] = p => p.StupanjKriticnosti,
        };

        public KriticnostJController(RPPP02Context ctx, ILogger<KriticnostJController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }


        [HttpGet("count", Name = "BrojKriticnosti")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.Kriticnost.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {

                query = query.Where(p => p.StupanjKriticnosti.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet(Name = "DohvatiKriticnosti")]
        public async Task<List<KriticnostJViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.Kriticnost.AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            {
                query = query.Where(p => p.StupanjKriticnosti.Contains(loadParams.Filter));
            }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var result = await query.Select(p => new KriticnostJViewModel
            {
                StupanjKriticnosti = p.StupanjKriticnosti,
                Id = p.Id,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id}", Name = "DohvatiKriticnost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KriticnostJViewModel>> Get(int id)
        {
            var kriticnost = await ctx.Kriticnost
                                  .Where(p => p.Id == id)
                                  .FirstOrDefaultAsync();
            if (kriticnost == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                var result = new KriticnostJViewModel
                {
                    StupanjKriticnosti = kriticnost.StupanjKriticnosti,
                    Id = kriticnost.Id,
                };
                return result;
            }
        }

        [HttpDelete("{id}", Name = "ObrisiKriticnost")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var kriticnost = await ctx.Kriticnost.FindAsync(id);
            if (kriticnost == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(kriticnost);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno obrisana kritičnost. Id=" + id);
                return NoContent();
            };
        }

        [HttpPut("{id}", Name = "AzurirajKriticnost")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] KriticnostJViewModel model)
        {
            if (model.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
            }
            else
            {
                var kriticnost = await ctx.Kriticnost.FindAsync(id);
                if (kriticnost == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                kriticnost.StupanjKriticnosti = model.StupanjKriticnosti;

                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno azurirana kritičnost. Id=" + id);
                return NoContent();
            }
        }

        [HttpPost(Name = "DodajKriticnost")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] KriticnostJViewModel model)
        {
            Kriticnost k = new Kriticnost
            {
                StupanjKriticnosti = model.StupanjKriticnosti
            };
            ctx.Add(k);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Uspjesno dodano nova kritičnost. Id=" + k.Id);
            var addedItem = await Get(k.Id);

            return CreatedAtAction(nameof(Get), new { id = k.Id }, addedItem.Value);
        }
    }
}
