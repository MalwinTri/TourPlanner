using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface IReportGenerator
    {
        public bool GenerateReport(Tour tour);
        public bool GenerateSummary();
    }
}
