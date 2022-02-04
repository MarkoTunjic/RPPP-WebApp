using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using RPPP_WebApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPPP_WebApp.ModelsPartial;
using RPPP_WebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using RPPP_WebApp.Extensions;
using System.Globalization;

namespace RPPP_WebApp.Controllers
{
    public class RadniNalogReportController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly ILogger<RadniNalogController> logger;
        public RadniNalogReportController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<RadniNalogController> logger, IWebHostEnvironment environment)
        {
            this.ctx = ctx;
            this.environment = environment;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Izvješća";
            ViewBag.StudentTables = Constants.StudentTables;
            return View();
        }

        #region Export u Excel
        public async Task<IActionResult> RadniListoviExcel()
        {
            var radniListovi = await ctx.RadniList
                                  .Select(r => new RadniListViewModel
                                  {
                                      NazivUredaja = r.IdUredajNavigation.Naziv,
                                      PocetakRada = r.PocetakRada,
                                      TrajanjeRada = r.TrajanjeRada,
                                      OpisRada = r.OpisRada,
                                      Status = r.IdStatusNavigation.NazivStatusa,
                                      TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                                      TrajanjeRN = r.IdRadniNalogNavigation.Trajanje
                                  })
                                  .AsNoTracking()
                                  .OrderBy(r => r.PocetakRada)
                                  .ToListAsync();
            var statusi = await ctx.Status
                                  .AsNoTracking()
                                  .OrderBy(p => p.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis radnih listova";
                excel.Workbook.Properties.Author = "Zdravko Petričušić";
                var worksheet = excel.Workbook.Worksheets.Add("Radni Listovi");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv uredaja";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 2].Value = "Pocetak rada";
                worksheet.Cells[1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 3].Value = "Trajanje rada";
                worksheet.Cells[1, 3].Style.Font.Bold = true;
                worksheet.Cells[1, 4].Value = "Opis rada";
                worksheet.Cells[1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 5].Value = "Status";
                worksheet.Cells[1, 5].Style.Font.Bold = true;
                worksheet.Cells[1, 6].Value = "Tim za odrzavanje";
                worksheet.Cells[1, 6].Style.Font.Bold = true;
                worksheet.Cells[1, 7].Value = "Trajanje Radnog naloga";
                worksheet.Cells[1, 7].Style.Font.Bold = true;

                for (int i = 0; i < radniListovi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = radniListovi[i].NazivUredaja;
                    worksheet.Cells[i + 2, 2].Value = radniListovi[i].PocetakRada.ToString("dd.MM.yyyy.");
                    worksheet.Cells[i + 2, 3].Value = radniListovi[i].TrajanjeRada;
                    worksheet.Cells[i + 2, 4].Value = radniListovi[i].OpisRada;
                    worksheet.Cells[i + 2, 5].Value = radniListovi[i].Status;
                    worksheet.Cells[i + 2, 6].Value = radniListovi[i].TimZaOdrzavanje;
                    worksheet.Cells[i + 2, 7].Value = radniListovi[i].TrajanjeRN;
                }

                worksheet.Cells[1, 1, radniListovi.Count + 1, 7].AutoFitColumns();

                worksheet = excel.Workbook.Worksheets.Add("Statusi");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Statusi";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                for (int i = 0; i < statusi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = statusi[i].NazivStatusa;
                }

                worksheet.Cells[1, 1, statusi.Count + 1, 1].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "RadniListovi.xlsx");
        }
        #endregion

        #region import u Excel
        public async Task<IActionResult> ImportExcel(IFormFile importFile)
        {
            ExcelPackage result = new ExcelPackage();

            await using (var ms = new MemoryStream())
            {
                await importFile.CopyToAsync(ms);
                using (ExcelPackage import = new ExcelPackage(ms))
                {
                    ExcelWorksheet worksheet = import.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.End.Row;

                    worksheet.Cells[1, 7].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int idTim = await ctx.TimZaOdrzavanje.AsNoTracking()
                            .Where(t => t.NazivTima.Equals(worksheet.Cells[row, 6].Value.ToString().Trim()))
                            .Select(t => t.Id)
                            .FirstOrDefaultAsync();
                        int idUredaj = await ctx.Uredaj.AsNoTracking()
                            .Where(p => p.Naziv.Equals(worksheet.Cells[row, 1].Value.ToString().Trim()))
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        int idStatus = await ctx.Status.AsNoTracking()
                            .Where(p => p.NazivStatusa.Equals(worksheet.Cells[row, 5].Value.ToString().Trim()))
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();
                        int idRadniNalog = await ctx.RadniNalog.AsNoTracking()
                            .Where(p => p.Trajanje.ToString().Equals(worksheet.Cells[row, 7].Value.ToString().Trim()))
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();


                        RadniList radniList = new RadniList
                        {
                            IdUredaj = idUredaj,
                            PocetakRada = DateTime.ParseExact(worksheet.Cells[row, 2].Value.ToString().Trim(), "dd.MM.yyyy.", CultureInfo.InvariantCulture),
                            TrajanjeRada = Int32.Parse(worksheet.Cells[row, 3].Value.ToString().Trim()),
                            OpisRada = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            IdStatus = idStatus,
                            IdTimZaOdrzavanje = idTim,
                            IdRadniNalog = idRadniNalog
                        };

                        try
                        {
                            ctx.Add(radniList);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Radni list uspješno dodan. Id={radniList.Id}");
                            worksheet.Cells[row, 7].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 7].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog radnog lista: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }

                    result.Workbook.Worksheets.Add("StatusiDodavanjaRadnogLista", worksheet);

                    worksheet = import.Workbook.Worksheets[1];
                    rowCount = worksheet.Dimension.End.Row;

                    worksheet.Cells[1, 2].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        Status status = new Status
                        {
                            NazivStatusa = worksheet.Cells[row, 1].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(status);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Status uspješno dodan. Id={status.Id}");
                            worksheet.Cells[row, 2].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 2].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog statusa: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }
                    result.Workbook.Worksheets.Add("StatusiDodavanjaStatusa", worksheet);
                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion

        #region Export u Excel
        public async Task<IActionResult> RadniNaloziExcel()
        {
            var radniNalozi = await ctx.RadniNalog
                                  .Select(p => new RadniNalogViewModel
                                  {
                                      Id = p.Id,
                                      Sla = p.Sla,
                                      Trajanje = p.Trajanje,
                                      TipRada = p.TipRada,
                                      TragKvara = p.TragKvara,
                                      PocetakRada = p.PocetakRada,
                                      Kontrolor = p.IdKontrolorNavigation.KorisnickoIme,
                                      Kvar = p.IdKvarNavigation.Opis,
                                      Status = p.IdStatusNavigation.NazivStatusa,
                                      StupanjPrioriteta = p.IdStupanjPrioritetaNavigation.StupanjPrioriteta,
                                      RadniListovi = p.RadniList.Select(r => new RadniListViewModel
                                      {
                                          NazivUredaja = r.IdUredajNavigation.Naziv,
                                          PocetakRada = r.PocetakRada,
                                          TrajanjeRada = r.TrajanjeRada,
                                          OpisRada = r.OpisRada,
                                          Status = r.IdStatusNavigation.NazivStatusa,
                                          TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima
                                      }).ToList(),
                                  })
                                  .AsNoTracking()
                                  .OrderBy(t => t.PocetakRada)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis radnih naloga";
                excel.Workbook.Properties.Author = "Zdravko Petričušić";
                foreach (var radniNalog in radniNalozi)
                {
                    var worksheet = excel.Workbook.Worksheets.Add($"Radni nalog Id={radniNalog.Id}");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Id";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = "Sla";
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Value = "Trajanje";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;
                    worksheet.Cells[1, 4].Value = "Trag Kvara";
                    worksheet.Cells[1, 4].Style.Font.Bold = true;
                    worksheet.Cells[1, 5].Value = "Pocetak Rada";
                    worksheet.Cells[1, 5].Style.Font.Bold = true;
                    worksheet.Cells[1, 6].Value = "Kontrolor";
                    worksheet.Cells[1, 6].Style.Font.Bold = true;
                    worksheet.Cells[1, 7].Value = "Kvar";
                    worksheet.Cells[1, 7].Style.Font.Bold = true;
                    worksheet.Cells[1, 8].Value = "Status";
                    worksheet.Cells[1, 8].Style.Font.Bold = true;
                    worksheet.Cells[1, 9].Value = "Stupanj Prioriteta";
                    worksheet.Cells[1, 9].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = radniNalog.Id;
                    worksheet.Cells[2, 2].Value = radniNalog.Sla;
                    worksheet.Cells[2, 3].Value = radniNalog.Trajanje;
                    worksheet.Cells[2, 4].Value = radniNalog.TragKvara;
                    worksheet.Cells[2, 5].Value = radniNalog.PocetakRada.ToString("dd.MM.yyyy.");
                    worksheet.Cells[2, 6].Value = radniNalog.Kontrolor;
                    worksheet.Cells[2, 7].Value = radniNalog.Kvar;
                    worksheet.Cells[2, 8].Value = radniNalog.Status;
                    worksheet.Cells[2, 9].Value = radniNalog.StupanjPrioriteta;

                    //First add the headers
                    worksheet.Cells[4, 1].Value = "Naziv Uredaja";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 2].Value = "Pocetak Rada";
                    worksheet.Cells[4, 2].Style.Font.Bold = true;
                    worksheet.Cells[4, 3].Value = "Trajanje Rada";
                    worksheet.Cells[4, 3].Style.Font.Bold = true;
                    worksheet.Cells[4, 4].Value = "Opis Rada";
                    worksheet.Cells[4, 4].Style.Font.Bold = true;
                    worksheet.Cells[4, 5].Value = "Status";
                    worksheet.Cells[4, 5].Style.Font.Bold = true;
                    worksheet.Cells[4, 6].Value = "Tim Za Odrzavanje";
                    worksheet.Cells[4, 6].Style.Font.Bold = true;

                    for (int i = 0; i < radniNalog.RadniListovi.Count; i++)
                    {
                        //First add the headers
                        worksheet.Cells[5 + i, 1].Value = radniNalog.RadniListovi[i].NazivUredaja;
                        worksheet.Cells[5 + i, 2].Value = radniNalog.RadniListovi[i].PocetakRada.ToString("dd.MM.yyyy.");
                        worksheet.Cells[5 + i, 3].Value = radniNalog.RadniListovi[i].TrajanjeRada;
                        worksheet.Cells[5 + i, 4].Value = radniNalog.RadniListovi[i].OpisRada;
                        worksheet.Cells[5 + i, 5].Value = radniNalog.RadniListovi[i].Status;
                        worksheet.Cells[5 + i, 6].Value = radniNalog.RadniListovi[i].TimZaOdrzavanje;
                    }
                    worksheet.Cells[1, 1, radniNalog.RadniListovi.Count + 5, 9].AutoFitColumns();
                }
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "RadniNalozi.xlsx");
        }
        #endregion

        public async Task<IActionResult> RadniNalozi()
        {
            string naslov = $"Radni nalozi";
            var radniListovi = await ctx.RadniList
                                  .Select(r => new RadniListDenorm
                                  {
                                      IdRadniNalog = r.IdRadniNalog,
                                      TipRada = r.IdRadniNalogNavigation.TipRada,
                                      TragKvara = r.IdRadniNalogNavigation.TragKvara,
                                      PocetakRada = r.PocetakRada,
                                      Kontrolor = r.IdRadniNalogNavigation.IdKontrolorNavigation.KorisnickoIme,
                                      Status = r.IdStatusNavigation.NazivStatusa,
                                      IdRadniList = r.Id,
                                      TrajanjeRada = r.TrajanjeRada,
                                      OpisRada = r.OpisRada,
                                      NazivUredaja = r.IdUredajNavigation.Naziv,
                                      TimZaOdrzavanje = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                                  })
                                  .OrderBy(r => r.IdRadniNalog)
                                  .ThenBy(r => r.PocetakRada)
                                  .ThenBy(r => r.Status)
                                  .ToListAsync();
            radniListovi.ForEach(r => r.UrlRadnogNaloga = Url.Action("Uredi", "RadniNalog", new { id = r.IdRadniNalog }));
            PdfReport report = CreateReport(naslov);
            #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.CustomHeader(new MasterDetailsHeaders(naslov)
                {
                    PdfRptFont = header.PdfFont
                });
            });
            #endregion
            #region Postavljanje izvora podataka i stupaca
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(radniListovi));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(r => r.IdRadniNalog);
                    column.Group(
                        (val1, val2) =>
                        {
                            return (int)val1 == (int)val2;
                        });
                });
                #endregion
                columns.AddColumn(column =>
                {
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.TipRada);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Tip Rada", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.TragKvara);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Trag Kvara", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.PocetakRada);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Pocetak Rada", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.Kontrolor);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Kontrolor", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.Status);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Status", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.TrajanjeRada);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Trajanje Rada", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.OpisRada);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Opis Rada", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.NazivUredaja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Naziv Uredaja", horizontalAlignment: HorizontalAlignment.Center);
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadniListDenorm>(x => x.TimZaOdrzavanje);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Tim Za Odrzavanje", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=RadniNalozi.pdf");
                return File(pdf, "application/pdf");
            }
            else
                return NotFound();
        }

        #region Master-detail header
        public class MasterDetailsHeaders : IPageHeader
        {
            private string naslov;
            public MasterDetailsHeaders(string naslov)
            {
                this.naslov = naslov;
            }
            public IPdfFont PdfRptFont { set; get; }

            public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
            {
                var idRadniNalog = newGroupInfo.GetSafeStringValueOf(nameof(RadniListDenorm.IdRadniNalog));
                var urlRadnogNaloga = newGroupInfo.GetSafeStringValueOf(nameof(RadniListDenorm.UrlRadnogNaloga));
                var tipRada = newGroupInfo.GetSafeStringValueOf(nameof(RadniListDenorm.TipRada));
                var tragKvara = newGroupInfo.GetSafeStringValueOf(nameof(RadniListDenorm.TragKvara));
                var pocetakRada = (DateTime)newGroupInfo.GetValueOf(nameof(RadniListDenorm.PocetakRada));
                var kontrolor = newGroupInfo.GetValueOf(nameof(RadniListDenorm.Kontrolor));
                var status = newGroupInfo.GetValueOf(nameof(RadniListDenorm.Status));


                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f, 2f, 2f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Id radnog naloga:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(RadniListDenorm.IdRadniNalog),
                            NavigationUrlPropertyName = nameof(RadniListDenorm.UrlRadnogNaloga),
                            BasicProperties = new CellBasicProperties
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                PdfFontStyle = DocumentFontStyle.Bold,
                                PdfFont = PdfRptFont
                            }
                        };

                        cellData.CellTemplate = cellTemplate;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "TipRada:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = tipRada;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Trag Kvara:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = tragKvara;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                    );

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Pocetak Rada:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = pocetakRada;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                        cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Kontrolor:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = kontrolor;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Status:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = status;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
            }

            public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
            {
                var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
                table.AddSimpleRow(
                   (cellData, cellProperties) =>
                   {
                       cellData.Value = naslov;
                       cellProperties.PdfFont = PdfRptFont;
                       cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                       cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
                   });
                return table.AddBorderToTable();
            }
        }
        #endregion

        #region Private methods
        private PdfReport CreateReport(string naslov)
        {
            var pdf = new PdfReport();

            pdf.DocumentPreferences(doc =>
            {
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "FER-ZPR",
                    Application = "RPPP02-WebApp Core",
                    Title = naslov
                });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            //fix za linux https://github.com/VahidN/PdfReport.Core/issues/40
            .DefaultFonts(fonts =>
            {
                fonts.Path(Path.Combine(environment.WebRootPath, "fonts", "verdana.ttf"),
                           Path.Combine(environment.WebRootPath, "fonts", "tahoma.ttf"));
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
            //
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                //table.NumberOfDataRowsPerPage(20);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 150,
                    SpacingAfterAllGroupsSummary = 5f
                });
                table.SpacingAfter(4f);
            });

            return pdf;
        }
        #endregion
    }
}

