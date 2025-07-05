namespace TourPlanner.BL.WeatherAPI.DTO
{
    internal class CurrentWeatherDto
    {
        public string? Last_updated { get; set; }
        public ConditionDto? Condition { get; set; }
        public double? Temp_c { get; set; }
        public double? Wind_kph { get; set; }
        public string? Wind_dir { get; set; }
        public double? Precip_mm { get; set; }
    }
}
