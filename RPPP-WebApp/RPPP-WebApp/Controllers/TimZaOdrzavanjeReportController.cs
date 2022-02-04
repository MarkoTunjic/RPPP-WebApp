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
    public class TimZaOdrzavanjeReportController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly ILogger<TimZaOdrzavanjeController> logger;
        public TimZaOdrzavanjeReportController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<TimZaOdrzavanjeController> logger, IWebHostEnvironment environment)
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
        public async Task<IActionResult> PlanoviOdrzavnjaExcel()
        {
            var planovi = await ctx.PlanOdrzavanja
                                  .Select(p => new PlanOdrzavanjaDenorm
                                  {
                                      DatumOdrzavanja = p.DatumOdrzavanja,
                                      NazivPodsustava = p.IdPodsustavNavigation.Naziv,
                                      NazivTima = p.IdTimZaOdrzavanjeNavigation.NazivTima,
                                      RazinaStrucnosti = p.RazinaStrucnosti
                                  })
                                  .AsNoTracking()
                                  .OrderBy(d => d.DatumOdrzavanja)
                                  .ToListAsync();
            var podrucjaRada = await ctx.PodrucjeRada
                                  .AsNoTracking()
                                  .OrderBy(p => p.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis planova za odrzavanje";
                excel.Workbook.Properties.Author = "Marko Tunjić";
                var worksheet = excel.Workbook.Worksheets.Add("Planovi za odrzavanje");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Datum Odrzavanja";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 2].Value = "Naziv tima";
                worksheet.Cells[1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 3].Value = "Naziv podsustava";
                worksheet.Cells[1, 3].Style.Font.Bold = true;
                worksheet.Cells[1, 4].Value = "Razina strucnosti";
                worksheet.Cells[1, 4].Style.Font.Bold = true;

                for (int i = 0; i < planovi.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = planovi[i].DatumOdrzavanja.ToString("dd.MM.yyyy.");
                    worksheet.Cells[i + 2, 2].Value = planovi[i].NazivTima;
                    worksheet.Cells[i + 2, 3].Value = planovi[i].NazivPodsustava;
                    worksheet.Cells[i + 2, 4].Value = planovi[i].RazinaStrucnosti;
                }

                worksheet.Cells[1, 1, planovi.Count + 1, 4].AutoFitColumns();

                worksheet = excel.Workbook.Worksheets.Add("Podrucja rada");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Podrucje rada";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                for (int i = 0; i < podrucjaRada.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = podrucjaRada[i].VrstaPodrucjaRada;
                }

                worksheet.Cells[1, 1, podrucjaRada.Count + 1, 1].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "PlanoviZaOdrzavanje.xlsx");
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

                    worksheet.Cells[1, 5].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int idTim = await ctx.TimZaOdrzavanje.AsNoTracking()
                            .Where(t => t.NazivTima.Equals(worksheet.Cells[row, 2].Value.ToString().Trim()))
                            .Select(t => t.Id)
                            .FirstOrDefaultAsync();
                        int idPodsustav = await ctx.Podsustav.AsNoTracking()
                            .Where(p => p.Naziv.Equals(worksheet.Cells[row, 3].Value.ToString().Trim()))
                            .Select(p => p.Id)
                            .FirstOrDefaultAsync();

                        PlanOdrzavanja planOdrzavanja = new PlanOdrzavanja
                        {
                            DatumOdrzavanja = DateTime.ParseExact(worksheet.Cells[row, 1].Value.ToString().Trim(), "dd.MM.yyyy.", CultureInfo.InvariantCulture),
                            IdPodsustav = idPodsustav,
                            IdTimZaOdrzavanje = idTim,
                            RazinaStrucnosti = Int32.Parse(worksheet.Cells[row, 4].Value.ToString().Trim())
                        };

                        try
                        {
                            ctx.Add(planOdrzavanja);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Plan održavanja uspješno dodan. Id={planOdrzavanja.Id}");
                            worksheet.Cells[row, 5].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 5].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog plana održavanja: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }

                    result.Workbook.Worksheets.Add("StatusiDodavanjaPlana", worksheet);

                    worksheet = import.Workbook.Worksheets[1];
                    rowCount = worksheet.Dimension.End.Row;

                    worksheet.Cells[1, 2].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        PodrucjeRada podrucjeRada = new PodrucjeRada
                        {
                            VrstaPodrucjaRada = worksheet.Cells[row, 1].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(podrucjeRada);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Plan održavanja uspješno dodan. Id={podrucjeRada.Id}");
                            worksheet.Cells[row, 2].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 2].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog plana održavanja: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }
                    result.Workbook.Worksheets.Add("StatusiDodavanjaPodrucja", worksheet);
                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion

        #region Export u Excel
        public async Task<IActionResult> TimoviZaOdrzavanjeExcel()
        {
            var timovi = await ctx.TimZaOdrzavanje
                                  .Select(p => new TimZaOdrzavanjeViewModel
                                  {
                                      DatumOsnivanja = p.DatumOsnivanja,
                                      Id = p.Id,
                                      IdPodrucjaRada = p.IdPodrucjeRada,
                                      NazivTima = p.NazivTima,
                                      Satnica = p.Satnica,
                                      VrstaPodrucjaRada = p.IdPodrucjeRadaNavigation.VrstaPodrucjaRada,
                                      Radnici = p.Radnik.Select(r => new RadnikViewModel
                                      {
                                          Certifikat = r.Certifikat,
                                          Dezuran = r.Dezuran,
                                          Id = r.Id,
                                          IdStrucnaSprema = r.IdStrucnaSprema,
                                          Ime = r.Ime,
                                          Prezime = r.Prezime,
                                          IstekCertifikata = r.IstekCertifikata,
                                          RazinaStrucneSpreme = r.IdStrucnaSpremaNavigation.RazinaStrucneSpreme
                                      }).ToList(),
                                  })
                                  .AsNoTracking()
                                  .OrderBy(t => t.DatumOsnivanja)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis timova za odrzavanje";
                excel.Workbook.Properties.Author = "Marko Tunjić";
                foreach (var tim in timovi)
                {
                    var worksheet = excel.Workbook.Worksheets.Add($"Tim za odrzavanje Id={tim.Id}");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Naziv Tima";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = "Datum osnivanja";
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Value = "Podrucje rada";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;
                    worksheet.Cells[1, 4].Value = "Satnica";
                    worksheet.Cells[1, 4].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = tim.NazivTima;
                    worksheet.Cells[2, 2].Value = tim.DatumOsnivanja.ToString("dd.MM.yyyy.");
                    worksheet.Cells[2, 3].Value = tim.VrstaPodrucjaRada;
                    worksheet.Cells[2, 4].Value = tim.Satnica;

                    //First add the headers
                    worksheet.Cells[4, 1].Value = "Ime radnika";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 2].Value = "Prezime radnika";
                    worksheet.Cells[4, 2].Style.Font.Bold = true;
                    worksheet.Cells[4, 3].Value = "Strucna sprema";
                    worksheet.Cells[4, 3].Style.Font.Bold = true;
                    worksheet.Cells[4, 4].Value = "Certifikat";
                    worksheet.Cells[4, 4].Style.Font.Bold = true;
                    worksheet.Cells[4, 5].Value = "Istek certifikata";
                    worksheet.Cells[4, 5].Style.Font.Bold = true;

                    for (int i = 0; i < tim.Radnici.Count; i++)
                    {
                        //First add the headers
                        worksheet.Cells[5 + i, 1].Value = tim.Radnici.ElementAt(i).Ime;
                        worksheet.Cells[5 + i, 2].Value = tim.Radnici.ElementAt(i).Prezime;
                        worksheet.Cells[5 + i, 3].Value = tim.Radnici.ElementAt(i).RazinaStrucneSpreme;
                        worksheet.Cells[5 + i, 4].Value = tim.Radnici.ElementAt(i).Certifikat == null || string.IsNullOrEmpty(tim.Radnici.ElementAt(i).Certifikat) ? "Nema" : tim.Radnici.ElementAt(i).Certifikat;
                        worksheet.Cells[5 + i, 5].Value = tim.Radnici.ElementAt(i).IstekCertifikata.HasValue ? tim.Radnici.ElementAt(i).IstekCertifikata?.ToString("dd.MM.yyyy.") : "Nema";
                    }
                    worksheet.Cells[1, 1, tim.Radnici.Count + 5, 5].AutoFitColumns();
                }
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "TimoviZaOdrzavanje.xlsx");
        }
        #endregion

        public async Task<IActionResult> TimoviZaOdrzavanje()
        {
            string naslov = $"Timovi za odrzavanje";
            var radnici = await ctx.Radnik
                                  .Select(r => new RadnikDenorm
                                  {
                                      IdTim = r.IdTimZaOdrzavanje,
                                      DatumOsnivanja = r.IdTimZaOdrzavanjeNavigation.DatumOsnivanja,
                                      NazivTima = r.IdTimZaOdrzavanjeNavigation.NazivTima,
                                      Satnica = r.IdTimZaOdrzavanjeNavigation.Satnica,
                                      PodrucjeRada = r.IdTimZaOdrzavanjeNavigation.IdPodrucjeRadaNavigation.VrstaPodrucjaRada,
                                      IdRadnik = r.Id,
                                      Ime = r.Ime,
                                      Prezime = r.Prezime,
                                      Certifikat = r.Certifikat,
                                      IstekCertifikata = r.IstekCertifikata.HasValue ? r.IstekCertifikata.Value.ToString("dd.MM.yyyy") : "Nema",
                                  })
                                  .OrderBy(r => r.IdTim)
                                  .ThenBy(r => r.Prezime)
                                  .ThenBy(r => r.Ime)
                                  .ToListAsync();
            radnici.ForEach(r => r.UrlTima = Url.Action("Uredi", "TimZaOdrzavanje", new { id = r.IdTim }));
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(radnici));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<RadnikDenorm>(r => r.IdTim);
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
                    column.PropertyName<RadnikDenorm>(x => x.Ime);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Ime", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadnikDenorm>(x => x.Prezime);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Prezime", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadnikDenorm>(x => x.Certifikat);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Certifikat", horizontalAlignment: HorizontalAlignment.Center);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                      ? "nema" : obj.ToString());
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<RadnikDenorm>(x => x.IstekCertifikata);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Istek certifikata", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=TimoviZaOdrzavanje.pdf");
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
                var idTim = newGroupInfo.GetSafeStringValueOf(nameof(RadnikDenorm.IdTim));
                var urlTim = newGroupInfo.GetSafeStringValueOf(nameof(RadnikDenorm.UrlTima));
                var nazivTima = newGroupInfo.GetSafeStringValueOf(nameof(RadnikDenorm.NazivTima));
                var podrucjeRada = newGroupInfo.GetSafeStringValueOf(nameof(RadnikDenorm.PodrucjeRada));
                var datumOsnivanja = (DateTime)newGroupInfo.GetValueOf(nameof(RadnikDenorm.DatumOsnivanja));
                var satnica = (int)newGroupInfo.GetValueOf(nameof(RadnikDenorm.Satnica));

                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f, 2f, 2f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Id tima:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(RadnikDenorm.IdTim),
                            NavigationUrlPropertyName = nameof(RadnikDenorm.UrlTima),
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
                        cellData.Value = "Naziv tima:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = nazivTima;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Datum osnivanja:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = datumOsnivanja;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                        cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Podrucje rada:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = podrucjeRada;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Satnica:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = satnica;
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

