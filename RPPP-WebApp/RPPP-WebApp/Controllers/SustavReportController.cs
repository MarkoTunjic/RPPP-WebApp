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
    public class SustavReportController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly ILogger<SustavController> logger;
        public SustavReportController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<SustavController> logger, IWebHostEnvironment environment)
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
        public async Task<IActionResult> KriticnostiVrsteSustavaExcel()
        {
            var kriticnosti = await ctx.Kriticnost
                                  .AsNoTracking()
                                  .OrderBy(k => k.Id)
                                  .ToListAsync();

            var vrsteSustava = await ctx.VrstaSustava
                                  .AsNoTracking()
                                  .OrderBy(v => v.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis kritičnosti i vrsti sustava";
                excel.Workbook.Properties.Author = "Blaž Solić";

                var worksheet = excel.Workbook.Worksheets.Add("Kriticnosti");
                worksheet.Cells[1, 1].Value = "Kriticnosti";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                for (int i = 0; i < kriticnosti.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = kriticnosti[i].StupanjKriticnosti;
                }
                worksheet.Cells[1, 1, kriticnosti.Count + 1, 1].AutoFitColumns();

                worksheet = excel.Workbook.Worksheets.Add("Vrste sustava");
                worksheet.Cells[1, 1].Value = "Vrste sustava";
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                for (int i = 0; i < vrsteSustava.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = vrsteSustava[i].NazivVrsteSustava;
                }
                worksheet.Cells[1, 1, vrsteSustava.Count + 1, 1].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "KriticnostiVrsteSustava.xlsx");
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
                    worksheet.Cells[1, 2].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        Kriticnost kriticnost = new Kriticnost
                        {
                            StupanjKriticnosti = worksheet.Cells[row, 1].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(kriticnost);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Kritičnost uspješno dodana. Id={kriticnost.Id}");
                            worksheet.Cells[row, 2].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 2].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja kritičnosti: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }


                    result.Workbook.Worksheets.Add("StatusiDodavanjaKriticnosti", worksheet);

                    worksheet = import.Workbook.Worksheets[1];
                    rowCount = worksheet.Dimension.End.Row;

                    worksheet.Cells[1, 2].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        VrstaSustava vrstaSustava = new VrstaSustava
                        {
                            NazivVrsteSustava = worksheet.Cells[row, 1].Value.ToString().Trim()
                        };

                        try
                        {
                            ctx.Add(vrstaSustava);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Vrsta sustava uspješno dodana. Id={vrstaSustava.Id}");
                            worksheet.Cells[row, 2].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 2].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja vrste sustava: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }
                    result.Workbook.Worksheets.Add("StatusiDodavanjaKritičnosti", worksheet);
                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion

        #region Export u Excel
        public async Task<IActionResult> SustaviExcel()
        {
            var sustavi = await ctx.Sustav
                                  .Select(s => new SustavViewModel
                                  {
                                      Id = s.Id,
                                      IdStupanjKriticnosti = s.IdKriticnost,
                                      StupanjKriticnosti = s.IdKriticnostNavigation.StupanjKriticnosti,
                                      Opis = s.Opis,
                                      Osjetljivost = s.Osjetljivost,
                                      IdVrstaSustava = s.IdVrstaSustava,
                                      VrstaSustava = s.IdVrstaSustavaNavigation.NazivVrsteSustava,
                                      Podsustavi = s.Podsustav.Select(p => new PodsustavViewModel
                                      {
                                          Id = p.Id,
                                          UcestalostOdrzavanja = p.UcestalostOdrzavanja,
                                          Osjetljivost = p.Osjetljivost,
                                          Naziv = p.Naziv,
                                          Opis = p.Opis,
                                          OpisSustava = p.IdSustavNavigation.Opis,
                                          NazivLokacije = p.IdLokacijaNavigation.Naziv,
                                          IdStupanjKriticnosti = p.IdKriticnostNavigation.Id,
                                          StupanjKriticnosti = p.IdKriticnostNavigation.StupanjKriticnosti,

                                      })
                                      .ToList(),
                                  })
                                  .AsNoTracking()
                                  .OrderBy(s => s.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis sustava";
                excel.Workbook.Properties.Author = "Blaž Solić";
                foreach (var sustav in sustavi)
                {
                    var worksheet = excel.Workbook.Worksheets.Add($"Sustav Id={sustav.Id}");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Opis";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = "Vrsta sustava";
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Value = "Osjetljivost";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;
                    worksheet.Cells[1, 4].Value = "Stupanj kritičnosti";
                    worksheet.Cells[1, 4].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = sustav.Opis;
                    worksheet.Cells[2, 2].Value = sustav.VrstaSustava;
                    worksheet.Cells[2, 3].Value = sustav.Osjetljivost;
                    worksheet.Cells[2, 4].Value = sustav.StupanjKriticnosti;

                    //First add the headers
                    worksheet.Cells[4, 1].Value = "Naziv";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 2].Value = "Opis";
                    worksheet.Cells[4, 2].Style.Font.Bold = true;
                    worksheet.Cells[4, 3].Value = "Osjetljivost";
                    worksheet.Cells[4, 3].Style.Font.Bold = true;
                    worksheet.Cells[4, 4].Value = "Stupanj Kritičnosti";
                    worksheet.Cells[4, 4].Style.Font.Bold = true;
                    worksheet.Cells[4, 5].Value = "Lokacija";
                    worksheet.Cells[4, 5].Style.Font.Bold = true;
                    worksheet.Cells[4, 6].Value = "Učestalost održavanja";
                    worksheet.Cells[4, 6].Style.Font.Bold = true;

                    for (int i = 0; i < sustav.Podsustavi.Count; i++)
                    { 
                        worksheet.Cells[5 + i, 1].Value = sustav.Podsustavi.ElementAt(i).Naziv;
                        worksheet.Cells[5 + i, 2].Value = sustav.Podsustavi.ElementAt(i).Opis;
                        worksheet.Cells[5 + i, 3].Value = sustav.Podsustavi.ElementAt(i).Osjetljivost;
                        worksheet.Cells[5 + i, 4].Value = sustav.Podsustavi.ElementAt(i).StupanjKriticnosti;
                        worksheet.Cells[5 + i, 5].Value = sustav.Podsustavi.ElementAt(i).NazivLokacije;
                        worksheet.Cells[5 + i, 6].Value = sustav.Podsustavi.ElementAt(i).UcestalostOdrzavanja;
                    }
                    worksheet.Cells[1, 1, sustav.Podsustavi.Count + 6, 6].AutoFitColumns();
                }
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Sustavi.xlsx");
        }
        #endregion

        public async Task<IActionResult> Sustavi()
        {
            string naslov = $"Sustavi";
            var podsustavi = await ctx.Podsustav
                                  .Select(p => new PodsustavDenorm
                                  {
                                      IdSustav = p.IdSustav,
                                      Opis = p.IdSustavNavigation.Opis,
                                      Osjetljivost = p.IdSustavNavigation.Osjetljivost,
                                      StupanjKriticnosti = p.IdSustavNavigation.IdKriticnostNavigation.StupanjKriticnosti,
                                      VrstaSustava = p.IdSustavNavigation.IdVrstaSustavaNavigation.NazivVrsteSustava,
                                      IdPodsustav = p.Id,
                                      Naziv = p.Naziv,
                                      OpisPodsustav = p.Opis,
                                      NazivLokacije = p.IdLokacijaNavigation.Naziv,
                                      UcestalostOdrzavanja = p.UcestalostOdrzavanja,
                                      OsjetljivostPodsustav = p.Osjetljivost,
                                      StupanjKriticnostiPodsustav = p.IdKriticnostNavigation.StupanjKriticnosti,
                                  })
                                  .OrderBy(p => p.IdSustav)
                                  .ToListAsync();
            podsustavi.ForEach(p => p.UrlSustava = Url.Action("Uredi", "Sustav", new { id = p.IdSustav }));
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(podsustavi));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<PodsustavDenorm>(p => p.IdSustav);
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
                    column.PropertyName<PodsustavDenorm>(p => p.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Naziv", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PodsustavDenorm>(p => p.OpisPodsustav);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Opis", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PodsustavDenorm>(p => p.StupanjKriticnostiPodsustav);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Stupanj kritičnosti", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PodsustavDenorm>(p => p.OsjetljivostPodsustav);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Osjetljivost", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<PodsustavDenorm>(p => p.UcestalostOdrzavanja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Učestalost održanja", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=Sustavi.pdf");
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
                var idSustav = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.IdSustav));
                var urlSustav = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.UrlSustava));
                var opis = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.Opis));
                var osjetljivost = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.Osjetljivost));
                var stupanjKriticnosti = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.StupanjKriticnosti));
                var vrstaSustava = newGroupInfo.GetSafeStringValueOf(nameof(PodsustavDenorm.VrstaSustava));

                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f, 2f, 2f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Id sustava:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(PodsustavDenorm.IdSustav),
                            NavigationUrlPropertyName = nameof(PodsustavDenorm.UrlSustava),
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
                        cellData.Value = "Opis sustava:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = opis;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Osjetljivost:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = osjetljivost;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Kriticnost:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = stupanjKriticnosti;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Vrsta sustava";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = vrstaSustava;
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

