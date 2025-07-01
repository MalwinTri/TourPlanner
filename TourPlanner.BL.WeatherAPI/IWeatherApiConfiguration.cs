using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.WeatherAPI
{
    public interface IWeatherApiConfiguration
    {
        public string WeatherApiKey { get; }
        public string WeatherApiUrl { get; }
    }
}
