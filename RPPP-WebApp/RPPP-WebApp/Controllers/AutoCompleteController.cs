using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RPPP_WebApp.Controllers
{
    public class AutoCompleteController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly AppSettings appData;

        public AutoCompleteController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options)
        {
            this.ctx = ctx;
            appData = options.Value;
        }
        public async Task<IEnumerable<IdLabel>> PodrucjeRada(string term)
        {
            var query = ctx.PodrucjeRada
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.VrstaPodrucjaRada
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
        public async Task<IEnumerable<IdLabel>> StrucnaSprema(string term)
        {
            var query = ctx.StrucnaSprema
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.RazinaStrucneSpreme
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Status(string term)
        {
            var query = ctx.Status
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.NazivStatusa
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Uredaj(string term)
        {
            var query = ctx.Uredaj
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.Naziv
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> TimZaOdrzavanje(string term)
        {
            var query = ctx.TimZaOdrzavanje
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.NazivTima
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Prioritet(string term)
        {
            var query = ctx.Prioritet
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.StupanjPrioriteta
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Kontrolor(string term)
        {
            var query = ctx.Kontrolor
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.KorisnickoIme
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Lokacija(string term)
        {
            var query = ctx.Lokacija
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.Naziv
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> Kvar(string term)
        {
            var query = ctx.Kvar
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.Opis
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
        public async Task<IEnumerable<IdLabel>> KrajSmjene(string term)
        {
            var query = ctx.KrajSmjene
                            .Select(k => new IdLabel
                            {
                                Id = k.Id,
                                Label = k.VrijemeKrajaSmjene
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            Console.WriteLine("patak");
            list.ForEach(l => Console.WriteLine(l.Label));
            return list;
        }
        public async Task<IEnumerable<IdLabel>> Rang(string term)
        {
            var query = ctx.Rang
                            .Select(r => new IdLabel
                            {
                                Id = r.Id,
                                Label = r.ImeRanga
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> StupanjKriticnosti(string term)
        {
            var query = ctx.Kriticnost
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.StupanjKriticnosti
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> VrstaSustava(string term)
        {
            var query = ctx.VrstaSustava
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.NazivVrsteSustava
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }

        public async Task<IEnumerable<IdLabel>> NazivLokacije(string term)
        {
            var query = ctx.Lokacija
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.Naziv
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
        public async Task<IEnumerable<IdLabel>> TipOpreme(string term)
        {
            var query = ctx.TipOpreme
                            .Select(t => new IdLabel
                            {
                                Id = t.Id,
                                Label = t.TipOpreme1
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
        public async Task<IEnumerable<IdLabel>> Podsustav(string term)
        {
            var query = ctx.Podsustav
                            .Select(p => new IdLabel
                            {
                                Id = p.Id,
                                Label = p.Naziv
                            })
                            .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                                  .ThenBy(l => l.Id)
                                  .Take(appData.AutoCompleteCount)
                                  .ToListAsync();
            return list;
        }
    }
}
