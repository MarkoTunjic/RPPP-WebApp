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
    public class StatusJController : ControllerBase, ICustomController<int, StatusJViewModel>
    {
        private readonly RPPP02Context ctx;
        ILogger<StatusJController> logger;
        private static Dictionary<string, Expression<Func<Status, object>>> orderSelectors = new Dictionary<string, Expression<Func<Status, object>>>
        {
            [nameof(StatusJViewModel.Id).ToLower()] = k => k.Id,
            [nameof(StatusJViewModel.NazivStatusa).ToLower()] = k => k.NazivStatusa,
        };

        public StatusJController(RPPP02Context ctx, ILogger<StatusJController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }


        [HttpGet("count", Name = "BrojStatusa")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.Status.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(k => k.NazivStatusa.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet(Name = "DohvatiStatuse")]
        public async Task<List<StatusJViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.Status.AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            {
                query = query.Where(k => k.NazivStatusa.Contains(loadParams.Filter));
            }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var result = await query.Select(k => new StatusJViewModel
            {
                NazivStatusa = k.NazivStatusa,
                Id = k.Id,
            }).ToListAsync();

            return result;
        }

        [HttpGet("{id}", Name = "DohvatiStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatusJViewModel>> Get(int id)
        {
            var status = await ctx.Status
                                  .Where(k => k.Id == id)
                                  .FirstOrDefaultAsync();
            if (status == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                var result = new StatusJViewModel
                {
                    Id = status.Id,
                    NazivStatusa = status.NazivStatusa
                };
                return result;
            }
        }

        [HttpDelete("{id}", Name = "ObrisiStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await ctx.Status.FindAsync(id);
            if (status == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(status);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno obrisan status. Id=" + id);
                return NoContent();
            };
        }

        [HttpPut("{id}", Name = "AzurirajStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] StatusJViewModel model)
        {
            if (model.Id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
            }
            else
            {
                var status = await ctx.Status.FindAsync(id);
                if (status == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                Console.WriteLine(model.NazivStatusa);
                status.NazivStatusa = model.NazivStatusa;

                await ctx.SaveChangesAsync();
                logger.LogInformation("Uspjesno azuriran status. Id=" + id);
                return NoContent();
            }
        }

        [HttpPost(Name = "DodajStatus")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] StatusJViewModel model)
        {
            Status status = new Status
            {
                NazivStatusa = model.NazivStatusa
            };
            ctx.Add(status);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Uspjesno dodano novi status. Id=" + status.Id);
            var addedItem = await Get(status.Id);

            return CreatedAtAction(nameof(Get), new { id = status.Id }, addedItem.Value);
        }
    }
}
