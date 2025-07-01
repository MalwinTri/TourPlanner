using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface IWeatherGenerator
    {
        Task<Weather?> GetWeather(Tour tour);
    }
}
