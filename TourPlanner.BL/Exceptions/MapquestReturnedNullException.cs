using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Exceptions
{
    public class MapquestReturnedNullException : Exception
    {
        public MapquestReturnedNullException() { }
        public MapquestReturnedNullException(string message) : base(message) { }
        public MapquestReturnedNullException(string message, Exception innerException) : base(message, innerException) { }
    }
}