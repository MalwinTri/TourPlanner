using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.BL.WeatherAPI.DTO
{
    internal class WeatherApiDto
    {
        public CurrentWeatherDto? Current { get; set; }


        public Weather ToWeather()
        {
            var weather = new Weather
            {
                Temperature = Current?.Temp_c ?? 0,
                WindSpeed = Current?.Wind_kph ?? 0,
                WindDirection = Current?.Wind_dir ?? "",
                Precipitation = Current?.Precip_mm ?? 0,
                Description = Current?.Condition?.Text ?? "",
                Icon = $"https:{Current?.Condition?.Icon ?? ""}" ?? ""
            };

            return weather;
        }
    }
}
