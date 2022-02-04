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
    public class SmjenaReportController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly ILogger<SmjenaController> logger;
        public SmjenaReportController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SmjenaController> logger, IWebHostEnvironment environment)
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
        public async Task<IActionResult> KontroloriExcel()
        {
            var kontrolori = await ctx.Kontrolor
                                  .Select(k => new KontrolorViewModel
                                  {
                                      Ime = k.Ime,
                                      Prezime = k.Prezime,
                                      Oib = k.Oib,
                                      DatumZaposlenja = k.DatumZaposlenja,
                                      ZaposlenDo = k.ZaposlenDo,
                                      KorisnickoIme = k.KorisnickoIme,
                                      Lozinka = k.Lozinka,
                                      ImeRanga = k.IdRangNavigation.ImeRanga,
                                      PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene
                                  })
                                  .AsNoTracking()
                                  .OrderBy(r => r.ImeRanga)
                                  .ToListAsync();
            var krajeviSmjena = await ctx.KrajSmjene
                                  .AsNoTracking()
                                  .OrderBy(k => k.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis kontrolora";
                excel.Workbook.Properties.Author = "Fran Posarić";
                var worksheet = excel.Workbook.Worksheets.Add("Kontrolori");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Ime";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 2].Value = "Prezime";
                worksheet.Cells[1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 3].Value = "OIB";
                worksheet.Cells[1, 3].Style.Font.Bold = true;
                worksheet.Cells[1, 4].Value = "Datum zaposlenja";
                worksheet.Cells[1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 5].Value = "Zaposlen do datuma";
                worksheet.Cells[1, 5].Style.Font.Bold = true;
                worksheet.Cells[1, 6].Value = "Korisničko ime";
                worksheet.Cells[1, 6].Style.Font.Bold = true;
                worksheet.Cells[1, 7].Value = "Lozinka";
                worksheet.Cells[1, 7].Style.Font.Bold = true;
                worksheet.Cells[1, 8].Value = "Ime ranga";
                worksheet.Cells[1, 8].Style.Font.Bold = true;
                worksheet.Cells[1, 9].Value = "Početak smjene";
                worksheet.Cells[1, 9].Style.Font.Bold = true;

                for (int i = 0; i < kontrolori.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = kontrolori[i].Ime;
                    worksheet.Cells[i + 2, 2].Value = kontrolori[i].Prezime;
                    worksheet.Cells[i + 2, 3].Value = kontrolori[i].Oib;
                    worksheet.Cells[i + 2, 4].Value = kontrolori[i].DatumZaposlenja.ToString("dd.MM.yyyy.");
                    worksheet.Cells[i + 2, 5].Value = kontrolori[i].ZaposlenDo == null ? "Zaposlen na neodređeno" : kontrolori[i].ZaposlenDo?.ToString("dd.MM.yyyy.");
                    worksheet.Cells[i + 2, 6].Value = kontrolori[i].KorisnickoIme;
                    worksheet.Cells[i + 2, 7].Value = kontrolori[i].Lozinka;
                    worksheet.Cells[i + 2, 8].Value = kontrolori[i].ImeRanga;
                    worksheet.Cells[i + 2, 9].Value = kontrolori[i].PocetakSmjene;
                }

                worksheet.Cells[1, 1, kontrolori.Count + 1, 9].AutoFitColumns();

                worksheet = excel.Workbook.Worksheets.Add("Krajevi smjena");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Kraj smjene";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                for (int i = 0; i < krajeviSmjena.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = krajeviSmjena[i].VrijemeKrajaSmjene;
                }

                worksheet.Cells[1, 1, krajeviSmjena.Count + 1, 1].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Kontrolori.xlsx");
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

                    worksheet.Cells[1, 10].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int idSmjena = await ctx.Smjena.AsNoTracking()
                            .Where(s => s.PocetakSmjene.Trim().Equals(worksheet.Cells[row, 9].Value.ToString().Trim()))
                            .Select(s => s.Id)
                            .FirstOrDefaultAsync();
                        int idRang = await ctx.Rang.AsNoTracking()
                            .Where(r => r.ImeRanga.Equals(worksheet.Cells[row, 8].Value.ToString().Trim()))
                            .Select(r => r.Id)
                            .FirstOrDefaultAsync();

                        Kontrolor kontrolor = new Kontrolor
                        {
                            Ime = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Prezime = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Oib = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            DatumZaposlenja = DateTime.ParseExact(worksheet.Cells[row, 4].Value.ToString().Trim(), "dd.MM.yyyy.", CultureInfo.InvariantCulture),
                            ZaposlenDo = worksheet.Cells[row, 5].Value.ToString().Trim().Equals("Zaposlen na neodređeno") ? null : DateTime.ParseExact(worksheet.Cells[row, 5].Value.ToString().Trim(), "dd.MM.yyyy.", CultureInfo.InvariantCulture),
                            KorisnickoIme = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            Lozinka = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            IdRang = idRang,
                            IdSmjena = idSmjena
                        };

                        try
                        {
                            ctx.Add(kontrolor);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Kontrolor uspješno dodan. Id={kontrolor.Id}");
                            worksheet.Cells[row, 10].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 10].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog kontrolora: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }

                    result.Workbook.Worksheets.Add("StatusiDodavanjaKontrolora", worksheet);

                    worksheet = import.Workbook.Worksheets[1];
                    rowCount = worksheet.Dimension.End.Row;

                    worksheet.Cells[1, 2].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        KrajSmjene krajSmjene = new KrajSmjene
                        {
                            VrijemeKrajaSmjene = worksheet.Cells[row, 1].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(krajSmjene);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Kraj smjene uspješno dodan. Id={krajSmjene.Id}");
                            worksheet.Cells[row, 2].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 2].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog kraja smjene: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }
                    result.Workbook.Worksheets.Add("StatusiDodavanjaKrajaSmjene", worksheet);
                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion

        #region Export u Excel
        public async Task<IActionResult> SmjeneExcel()
        {
            var smjene = await ctx.Smjena
                                  .Select(s => new SmjenaViewModel
                                  {
                                      Id = s.Id,
                                      PlatniFaktor = s.PlatniFaktor,
                                      PocetakSmjene = s.PocetakSmjene,
                                      Kontrolori = s.Kontrolor
                                        .Select(k => new KontrolorViewModel
                                        {
                                            Ime = k.Ime,
                                            Prezime = k.Prezime,
                                            Oib = k.Oib,
                                            DatumZaposlenja = k.DatumZaposlenja,
                                            ZaposlenDo = k.ZaposlenDo,
                                            KorisnickoIme = k.KorisnickoIme,
                                            Lozinka = k.Lozinka,
                                            ImeRanga = k.IdRangNavigation.ImeRanga,
                                            PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene,
                                            IdRang = k.IdRang
                                        })
                                        .ToList(),
                                      VrijemeKrajaSmjene = s.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                                      IdKrajSmjene = s.IdKrajSmjene
                                  })
                                  .AsNoTracking()
                                  .OrderBy(s => s.PocetakSmjene)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis smjena";
                excel.Workbook.Properties.Author = "Fran Posarić";
                foreach (var smjena in smjene)
                {
                    var worksheet = excel.Workbook.Worksheets.Add($"Smjena Id={smjena.Id}");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Platni faktor";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = "Početak smjene";
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Value = "Vrijeme kraja smjene";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = smjena.PlatniFaktor;
                    worksheet.Cells[2, 2].Value = smjena.PocetakSmjene;
                    worksheet.Cells[2, 3].Value = smjena.VrijemeKrajaSmjene;

                    //First add the headers
                    worksheet.Cells[4, 1].Value = "Ime";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 2].Value = "Prezime";
                    worksheet.Cells[4, 2].Style.Font.Bold = true;
                    worksheet.Cells[4, 3].Value = "OIB";
                    worksheet.Cells[4, 3].Style.Font.Bold = true;
                    worksheet.Cells[4, 4].Value = "Datum zaposlenja";
                    worksheet.Cells[4, 4].Style.Font.Bold = true;
                    worksheet.Cells[4, 5].Value = "Zaposlen do datuma";
                    worksheet.Cells[4, 5].Style.Font.Bold = true;
                    worksheet.Cells[4, 6].Value = "Korisničko ime";
                    worksheet.Cells[4, 6].Style.Font.Bold = true;
                    worksheet.Cells[4, 7].Value = "Lozinka";
                    worksheet.Cells[4, 7].Style.Font.Bold = true;
                    worksheet.Cells[4, 8].Value = "Ime ranga";
                    worksheet.Cells[4, 8].Style.Font.Bold = true;
                    worksheet.Cells[4, 9].Value = "Početak smjene";
                    worksheet.Cells[4, 9].Style.Font.Bold = true;

                    for (int i = 0; i < smjena.Kontrolori.Count; i++)
                    {
                        //First add the headers
                        worksheet.Cells[i + 5, 1].Value = smjena.Kontrolori[i].Ime;
                        worksheet.Cells[i + 5, 2].Value = smjena.Kontrolori[i].Prezime;
                        worksheet.Cells[i + 5, 3].Value = smjena.Kontrolori[i].Oib;
                        worksheet.Cells[i + 5, 4].Value = smjena.Kontrolori[i].DatumZaposlenja.ToString("dd.MM.yyyy.");
                        worksheet.Cells[i + 5, 5].Value = smjena.Kontrolori[i].ZaposlenDo == null ? "Zaposlen na neodređeno." : smjena.Kontrolori[i].ZaposlenDo?.ToString("dd.MM.yyyy.");
                        worksheet.Cells[i + 5, 6].Value = smjena.Kontrolori[i].KorisnickoIme;
                        worksheet.Cells[i + 5, 7].Value = smjena.Kontrolori[i].Lozinka;
                        worksheet.Cells[i + 5, 8].Value = smjena.Kontrolori[i].ImeRanga;
                        worksheet.Cells[i + 5, 9].Value = smjena.Kontrolori[i].PocetakSmjene;
                    }
                    worksheet.Cells[1, 1, smjena.Kontrolori.Count + 5, 9].AutoFitColumns();
                }
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Smjene.xlsx");
        }
        #endregion

        public async Task<IActionResult> Smjene()
        {
            string naslov = $"Smjene";
            var kontrolori = await ctx.Kontrolor
                                  .Select(k => new KontrolorDenorm
                                  {
                                      IdSmjena = k.IdSmjena,
                                      PocetakSmjene = k.IdSmjenaNavigation.PocetakSmjene,
                                      PlatniFaktor = k.IdSmjenaNavigation.PlatniFaktor,
                                      KrajSmjene = k.IdSmjenaNavigation.IdKrajSmjeneNavigation.VrijemeKrajaSmjene,
                                      IdKontrolor = k.Id,
                                      Ime = k.Ime,
                                      Prezime = k.Prezime,
                                      Oib = k.Oib,
                                      KorisnickoIme = k.KorisnickoIme
                                  })
                                  .OrderBy(k => k.IdSmjena)
                                  .ThenBy(k => k.Prezime)
                                  .ThenBy(k => k.PlatniFaktor)
                                  .ToListAsync();
            kontrolori.ForEach(k => k.UrlSmjene = Url.Action("Uredi", "Smjena", new { id = k.IdSmjena }));
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(kontrolori));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<KontrolorDenorm>(k => k.IdSmjena);
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
                    column.PropertyName<KontrolorDenorm>(x => x.Ime);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Ime", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KontrolorDenorm>(x => x.Prezime);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Prezime", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KontrolorDenorm>(x => x.Oib);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("OIB", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<KontrolorDenorm>(x => x.KorisnickoIme);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Korisničko ime", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=Smjene.pdf");
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
                var idSmjena = newGroupInfo.GetSafeStringValueOf(nameof(KontrolorDenorm.IdSmjena));
                var urlSmjena = newGroupInfo.GetSafeStringValueOf(nameof(KontrolorDenorm.UrlSmjene));
                var pocetakSmjene = newGroupInfo.GetSafeStringValueOf(nameof(KontrolorDenorm.PocetakSmjene));
                var platniFaktor = newGroupInfo.GetSafeStringValueOf(nameof(KontrolorDenorm.PlatniFaktor));
                var krajSmjene = newGroupInfo.GetSafeStringValueOf(nameof(KontrolorDenorm.KrajSmjene));

                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f}) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Id smjene:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(KontrolorDenorm.IdSmjena),
                            NavigationUrlPropertyName = nameof(KontrolorDenorm.UrlSmjene),
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
                        cellData.Value = "Pocetak smjene:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = pocetakSmjene;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Platni faktor:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = platniFaktor;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Kraj smjene:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = krajSmjene;
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

