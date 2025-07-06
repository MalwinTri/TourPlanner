using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Exceptions
{
    public class TourPlannerManagerException : Exception
    {
        public TourPlannerManagerException() { }
        public TourPlannerManagerException(string message) : base(message) { }
        public TourPlannerManagerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
