using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using TourPlanner.Logging;
using TourPlanner.Models;
using System.Globalization;

namespace TourPlanner.BL.iText
{
    public class iTextReportGenerator : IReportGenerator
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerManager _tourManager;
        private readonly ITourPlannerLogManager _logManager;
        private readonly string _targetPath;

        public iTextReportGenerator(ITourPlannerManager tourManager, ITourPlannerLogManager logManager, IItextConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<iTextReportGenerator>();

            _tourManager = tourManager;
            _logManager = logManager;
            _targetPath = configuration.OutputPath;
        }

        public bool GenerateReport(Tour tour)
        {
            try
            {
                _logger.Debug($"Generating report for tour {tour.Name}");
                if (!Directory.Exists(_targetPath))
                {
                    Directory.CreateDirectory(_targetPath);
                    _logger.Debug($"Created directory {_targetPath}");
                }

                var targetPdf = $"{_targetPath}{tour.Name}.pdf";

                if (File.Exists(targetPdf))
                {
                    targetPdf = $"{_targetPath}{tour.Name}-{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    _logger.Debug($"File already exists, creating new file at {targetPdf}");
                }

                var writer = new PdfWriter(targetPdf);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                var tourReportHeader = new Paragraph($"Tour {tour.Name}")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(16)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(ColorConstants.BLACK);
                document.Add(tourReportHeader);
                document.Add(new Paragraph(tour.Description)
                    .SetFontColor(new DeviceRgb(109, 104, 117)));

                var listHeader = new Paragraph("Tour Details")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(ColorConstants.BLACK);
                var list = new List()
                    .SetSymbolIndent(12)
                    .SetListSymbol("\u2022")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                var seconds = tour.Time ?? 0.0;
                list.Add(new ListItem(tour.Id.ToString()))
                    .Add(new ListItem(tour.Name))
                    .Add(new ListItem(tour.From))
                    .Add(new ListItem(tour.To))
                    .Add(new ListItem(tour.Transport))
                    .Add(new ListItem($"{tour.Distance}km"))
                    .Add(new ListItem(TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss")))
                    .Add(new ListItem($"Popularity: {tour.Popularity}"))
                    .Add(new ListItem($"Childfriendliness: {tour.ChildFriendliness}"))


                    .SetFontColor(new DeviceRgb(109, 104, 117));
                document.Add(listHeader);
                document.Add(list);

                var tableHeader = new Paragraph("Tour Logs")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(ColorConstants.BLACK);

                document.Add(tableHeader);
                var table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
                table.AddHeaderCell(GetHeaderCell("Date/Time"));
                table.AddHeaderCell(GetHeaderCell("Comment"));
                table.AddHeaderCell(GetHeaderCell("Difficulty"));
                table.AddHeaderCell(GetHeaderCell("Total time"));
                table.AddHeaderCell(GetHeaderCell("Rating"));
                table.SetFontSize(12).SetBackgroundColor(ColorConstants.WHITE);


                var logs = _logManager.FindMatchingTourLogs(tour).ToList();

                if (!logs.Any())
                {
                    table.AddCell("No logs found");
                    table.AddCell("No logs found");
                    table.AddCell("No logs found");
                    table.AddCell("No logs found");
                    table.AddCell("No logs found");
                }

                foreach (var log in logs)
                {
                    var sec = log.TotalTime ?? 0;
                    table.AddCell(log.Date.ToLocalTime().ToShortDateString());
                    table.AddCell(log.Comment);
                    table.AddCell(log.Difficulty.ToString());
                    table.AddCell(TimeSpan.FromSeconds(sec).ToString(@"hh\:mm\:ss"));
                    table.AddCell(log.Rating.ToString());
                }

                document.Add(table);

                var div = new Div().SetKeepTogether(true);

                var imageHeader = new Paragraph("Mapquest route")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(ColorConstants.BLACK);
                imageHeader.SetMarginTop(20);
                var imageData = ImageDataFactory.Create(tour.ImagePath);
                div.Add(imageHeader);
                div.Add(new Image(imageData).SetAutoScale(true));
                document.Add(div);

                document.Close();


                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Error generating report for tour {tour.Name}");
                _logger.Error(e.Message);
                throw;
            }
        }

        private static Cell GetHeaderCell(string s)
        {
            return new Cell().Add(new Paragraph(s)).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetBackgroundColor(ColorConstants.GRAY);
        }

        public bool GenerateSummary()
        {
            try
            {
                if (!Directory.Exists(_targetPath))
                {
                    Directory.CreateDirectory(_targetPath);
                    _logger.Debug($"Created directory {_targetPath}");
                }


                var tours = _tourManager.FindMatchingTours().ToList();

                var targetPdf = $"{_targetPath}toursummary.pdf";

                if (File.Exists(targetPdf))
                {
                    targetPdf = $"{_targetPath}toursummary-{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    _logger.Debug($"File already exists, creating new file at {targetPdf}");
                }

                var writer = new PdfWriter(targetPdf);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                foreach (var tour in tours)
                {
                    var tourReportHeader = new Paragraph($"Tour {tour.Name} summary")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(16)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontColor(ColorConstants.BLACK);
                    document.Add(tourReportHeader);
                    document.Add(new Paragraph(tour.Description)
                        .SetFontColor(new DeviceRgb(109, 104, 117)));

                    var listHeader = new Paragraph("Tour Details")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(14)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontColor(ColorConstants.BLACK);
                    var list = new List()
                        .SetSymbolIndent(12)
                        .SetListSymbol("\u2022")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                    var seconds = tour.Time ?? 0.0;
                    list.Add(new ListItem(tour.Id.ToString()))
                        .Add(new ListItem($"Name: {tour.Name}"))
                        .Add(new ListItem($"From: {tour.From}"))
                        .Add(new ListItem($"To: {tour.To}"))
                        .Add(new ListItem($"Transport: {tour.Transport}"))
                        .Add(new ListItem($"Distance: {tour.Distance:F2}km"))
                        .Add(new ListItem($"Time: {TimeSpan.FromSeconds(seconds):hh\\:mm\\:ss}"))
                        .Add(new ListItem($"Popularity: {tour.Popularity}"))
                        .Add(new ListItem($"Child Friendliness: {tour.ChildFriendliness}"))

                        .SetFontColor(new DeviceRgb(109, 104, 117));
                    document.Add(listHeader);
                    document.Add(list);

                    var tableHeader = new Paragraph("Tour Logs")
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(12)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontColor(ColorConstants.BLACK);
                    document.Add(tableHeader);
                    var table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
                    table.AddHeaderCell(GetHeaderCell("Mean Difficulty"));
                    table.AddHeaderCell(GetHeaderCell("Mean Total time"));
                    table.AddHeaderCell(GetHeaderCell("Mean Rating"));
                    table.SetFontSize(12).SetBackgroundColor(ColorConstants.WHITE);

                    var logs = _logManager.FindMatchingTourLogs(tour).ToList();

                    if (!logs.Any())
                    {
                        table.AddCell("No logs found");
                        table.AddCell("No logs found");
                        table.AddCell("No logs found");
                    }
                    else
                    {
                        var sec = 0.0;
                        var diff = 0.0;
                        var rate = 0.0;
                        foreach (var log in logs)
                        {
                            sec += log.TotalTime ?? 0;
                            diff += log.Difficulty ?? 0;
                            rate += log.Rating ?? 0;
                        }

                        sec /= logs.Count;
                        diff /= logs.Count;
                        rate /= logs.Count;

                        table.AddCell(diff.ToString("F2", CultureInfo.InvariantCulture));
                        table.AddCell(TimeSpan.FromSeconds(sec).ToString(@"hh\:mm\:ss"));
                        table.AddCell(rate.ToString("F2", CultureInfo.InvariantCulture));
                    }


                    document.Add(table);
                    document.Add(new Paragraph("\n"));
                }

                document.Close();

                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Error generating summary");
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
