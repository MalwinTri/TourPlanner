using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.DAL.Exceptions
{
    public class PostgresDataBaseException : Exception
    {
        public PostgresDataBaseException() { }
        public PostgresDataBaseException(string message) : base(message) { }
        public PostgresDataBaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
