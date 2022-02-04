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
    public class OpremaReportController : Controller
    {
        private readonly RPPP02Context ctx;
        private readonly IWebHostEnvironment environment;
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly ILogger<OpremaController> logger;
        public OpremaReportController(RPPP02Context ctx, IOptionsSnapshot<AppSettings> options, ILogger<OpremaController> logger, IWebHostEnvironment environment)
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
        public async Task<IActionResult> UredajiExcel()
        {
            var uredaji = await ctx.Uredaj
                                  .Select(u => new UredajDenorm
                                  {
                                      Naziv = u.Naziv,
                                      Proizvodac = u.Proizvodac,
                                      GodinaProizvodnje = u.GodinaProizvodnje,
                                      IdOprema = u.IdOprema,
                                      TipStanja = u.IdStanjeNavigation.TipStanja
                                  })
                                  .AsNoTracking()
                                  .OrderBy(u => u.Naziv)
                                  .ToListAsync();

            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis uređaja";
                excel.Workbook.Properties.Author = "Nina Petrušić";
                var worksheet = excel.Workbook.Worksheets.Add("Uređaji");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Naziv";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 2].Value = "Proizvodac";
                worksheet.Cells[1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 3].Value = "Godina proizvodnje";
                worksheet.Cells[1, 3].Style.Font.Bold = true;
                worksheet.Cells[1, 4].Value = "ID opreme";
                worksheet.Cells[1, 4].Style.Font.Bold = true;
                worksheet.Cells[1, 5].Value = "Tip stanja";
                worksheet.Cells[1, 5].Style.Font.Bold = true;

                for (int i = 0; i < uredaji.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = uredaji[i].Naziv;
                    worksheet.Cells[i + 2, 2].Value = uredaji[i].Proizvodac;
                    worksheet.Cells[i + 2, 3].Value = uredaji[i].GodinaProizvodnje;
                    worksheet.Cells[i + 2, 4].Value = uredaji[i].IdOprema;
                    worksheet.Cells[i + 2, 5].Value = uredaji[i].TipStanja;
                }

                worksheet.Cells[1, 1, uredaji.Count + 1, 5].AutoFitColumns();

                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Uredaji.xlsx");
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

                    worksheet.Cells[1, 6].Value = "Status";

                    for (int row = 2; row <= rowCount; row++)
                    {
                        int idOprema = await ctx.Oprema.AsNoTracking()
                            .Where(o => o.Id.Equals(worksheet.Cells[row, 4].Value))
                            .Select(o => o.Id)
                            .FirstOrDefaultAsync();
                        int idStanja = await ctx.Stanje.AsNoTracking()
                            .Where(s => s.TipStanja.Equals(worksheet.Cells[row, 5].Value))
                            .Select(s => s.Id)
                            .FirstOrDefaultAsync();

                        Uredaj uredaj = new Uredaj
                        {
                            Naziv = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Proizvodac = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            GodinaProizvodnje = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            IdOprema = idOprema,
                            IdStanje = idStanja
                        };

                        try
                        {
                            ctx.Add(uredaj);
                            await ctx.SaveChangesAsync();

                            logger.LogInformation($"Uređaj uspješno dodan. Id={uredaj.Id}");
                            worksheet.Cells[row, 6].Value = "ADDED";
                        }
                        catch (Exception exc)
                        {
                            worksheet.Cells[row, 6].Value = "ERROR";
                            logger.LogError("Pogreška prilikom dodavanja novog uređaja: {0}", exc.CompleteExceptionMessage());
                            ModelState.AddModelError(string.Empty, exc.Message);
                        }
                    }

                    result.Workbook.Worksheets.Add("StatusiDodavanjaUredaja", worksheet);

                }
            }
            return File(result.GetAsByteArray(), ExcelContentType, "StatusiDodavanja.xlsx");
        }
        #endregion

        #region Export u Excel
        public async Task<IActionResult> OpremaListExcel()
        {
            var oprema = await ctx.Oprema
                                  .Select(o => new OpremaViewModel
                                  {
                                      Redundantnost = o.Redundantnost,
                                      Id = o.Id,
                                      Budzet = o.Budzet,
                                      DatumPustanjaUPogon = o.DatumPustanjaUPogon,
                                      TipOpreme = o.IdTipOpremeNavigation.TipOpreme1,
                                      PodsustavNaziv = o.IdPodsustavNavigation.Naziv,
                                      Uredaji = o.Uredaj.Select(u => new UredajViewModel
                                      {
                                          Naziv = u.Naziv,
                                          Proizvodac = u.Proizvodac,
                                          Id = u.Id,
                                          GodinaProizvodnje = u.GodinaProizvodnje,
                                          IdOprema = u.IdOprema,
                                          IdStanja = u.IdStanje,
                                          TipStanja = u.IdStanjeNavigation.TipStanja,
                                      }).ToList(),
                                  })
                                  .AsNoTracking()
                                  .OrderBy(o => o.Id)
                                  .ToListAsync();
            byte[] content;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Properties.Title = "Popis opreme";
                excel.Workbook.Properties.Author = "Nina Petrušić";
                foreach (var opremaSingle in oprema)
                {
                    var worksheet = excel.Workbook.Worksheets.Add($"Oprema Id={opremaSingle.Id}");
                    //First add the headers
                    worksheet.Cells[1, 1].Value = "Redundantnost";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = "Budžet";
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 3].Value = "Datum puštanja u pogon";
                    worksheet.Cells[1, 3].Style.Font.Bold = true;
                    worksheet.Cells[1, 4].Value = "Tip opreme";
                    worksheet.Cells[1, 4].Style.Font.Bold = true;
                    worksheet.Cells[1, 5].Value = "Naziv podsustava";
                    worksheet.Cells[1, 5].Style.Font.Bold = true;

                    worksheet.Cells[2, 1].Value = opremaSingle.Redundantnost;
                    worksheet.Cells[2, 2].Value = opremaSingle.Budzet;
                    worksheet.Cells[2, 3].Value = opremaSingle.DatumPustanjaUPogon;
                    worksheet.Cells[2, 4].Value = opremaSingle.TipOpreme;
                    worksheet.Cells[2, 5].Value = opremaSingle.PodsustavNaziv;

                    //First add the headers
                    worksheet.Cells[4, 1].Value = "Naziv";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 2].Value = "Proizvođač";
                    worksheet.Cells[4, 2].Style.Font.Bold = true;
                    worksheet.Cells[4, 3].Value = "Godina proizvodnje";
                    worksheet.Cells[4, 3].Style.Font.Bold = true;
                    worksheet.Cells[4, 4].Value = "Id opreme";
                    worksheet.Cells[4, 4].Style.Font.Bold = true;
                    worksheet.Cells[4, 5].Value = "Id stanja";
                    worksheet.Cells[4, 5].Style.Font.Bold = true;
                    worksheet.Cells[4, 6].Value = "Tip stanja";
                    worksheet.Cells[4, 6].Style.Font.Bold = true;

                    for (int i = 0; i < opremaSingle.Uredaji.Count; i++)
                    {
                        //First add the headers
                        worksheet.Cells[5 + i, 1].Value = opremaSingle.Uredaji.ElementAt(i).Naziv;
                        worksheet.Cells[5 + i, 2].Value = opremaSingle.Uredaji.ElementAt(i).Proizvodac;
                        worksheet.Cells[5 + i, 3].Value = opremaSingle.Uredaji.ElementAt(i).GodinaProizvodnje;
                        worksheet.Cells[5 + i, 4].Value = opremaSingle.Uredaji.ElementAt(i).IdOprema;
                        worksheet.Cells[5 + i, 5].Value = opremaSingle.Uredaji.ElementAt(i).IdStanja;
                        worksheet.Cells[5 + i, 6].Value = opremaSingle.Uredaji.ElementAt(i).TipStanja;
                    }
                    worksheet.Cells[1, 1, opremaSingle.Uredaji.Count + 6, 6].AutoFitColumns();
                }
                content = excel.GetAsByteArray();
            }
            return File(content, ExcelContentType, "Oprema.xlsx");
        }
        #endregion

        public async Task<IActionResult> OpremaList()
        {
            string naslov = $"Oprema";
            var uredaji = await ctx.Uredaj
                                  .Select(u => new UredajDenorm
                                  {
                                      Naziv = u.Naziv,
                                      Proizvodac = u.Proizvodac,
                                      GodinaProizvodnje = u.GodinaProizvodnje,
                                      IdOprema = u.IdOprema,
                                      TipStanja = u.IdStanjeNavigation.TipStanja
                                  })
                                  .OrderBy(u => u.Naziv)
                                  .ToListAsync();
            uredaji.ForEach(u => u.UrlOpreme = Url.Action("Uredi", "Oprema", new { id = u.IdOprema }));
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
            report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(uredaji));

            report.MainTableColumns(columns =>
            {
                #region Stupci po kojima se grupira
                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(u => u.IdOprema);
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
                    column.PropertyName<UredajDenorm>(x => x.Naziv);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Naziv", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.Proizvodac);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(2);
                    column.HeaderCell("Proizvodac", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.GodinaProizvodnje);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Godina proizvodnje", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.IdOprema);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Id opreme", horizontalAlignment: HorizontalAlignment.Center);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UredajDenorm>(x => x.TipStanja);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Width(1);
                    column.HeaderCell("Tip stanja", horizontalAlignment: HorizontalAlignment.Center);
                });
            });

            #endregion
            byte[] pdf = report.GenerateAsByteArray();

            if (pdf != null)
            {
                Response.Headers.Add("content-disposition", "inline; filename=OpremaList.pdf");
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
                var idOprema = newGroupInfo.GetSafeStringValueOf(nameof(UredajDenorm.IdOprema));
                var UrlOpreme = newGroupInfo.GetSafeStringValueOf(nameof(UredajDenorm.UrlOpreme));
                var naziv = newGroupInfo.GetSafeStringValueOf(nameof(UredajDenorm.Naziv));
                var proizvodac = newGroupInfo.GetSafeStringValueOf(nameof(UredajDenorm.Proizvodac));
                var godinaProizvodnje = newGroupInfo.GetValueOf(nameof(UredajDenorm.GodinaProizvodnje));
                var tipStanja = newGroupInfo.GetValueOf(nameof(UredajDenorm.TipStanja));

                var table = new PdfGrid(relativeWidths: new[] { 2f, 2f, 2f, 2f, 2f, 2f }) { WidthPercentage = 100 };

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Id opreme:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                        var cellTemplate = new HyperlinkField(BaseColor.Black, false)
                        {
                            TextPropertyName = nameof(UredajDenorm.IdOprema),
                            NavigationUrlPropertyName = nameof(UredajDenorm.UrlOpreme),
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
                        cellData.Value = "Naziv opreme:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = naziv;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Proizvođač:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = proizvodac;
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                table.AddSimpleRow(
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = "Godina proizvodnje:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = godinaProizvodnje;
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
                        cellData.Value = "Tip stanja:";
                        cellProperties.PdfFont = PdfRptFont;
                        cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                        cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                    },
                    (cellData, cellProperties) =>
                    {
                        cellData.Value = tipStanja;
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
