using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Exceptions
{
    public class WeatherApiReturnedNullException : Exception
    {
        public WeatherApiReturnedNullException() { }
        public WeatherApiReturnedNullException(string message) : base(message) { }
        public WeatherApiReturnedNullException(string message, Exception innerException) : base(message, innerException) { }
    }
}
