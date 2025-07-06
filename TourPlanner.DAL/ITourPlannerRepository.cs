using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.DAL
{
    public interface ITourPlannerRepository
    {
        Tour Add(Tour tour);
        Tour? Edit(Tour tour);
        bool Remove(Tour tour);
        TourLog? Add(TourLog tourLog);
        TourLog? Edit(TourLog tourLog);
        bool Remove(TourLog tourLog);

        IEnumerable<Tour> GetAllTours();
        IEnumerable<Tour> GetAllToursByText(string searchPattern);
        IEnumerable<TourLog> GetAllTourLogsById(Guid tourId);
        IEnumerable<TourLog> GetAllTourLogsByText(string searchPattern);
    }
}
