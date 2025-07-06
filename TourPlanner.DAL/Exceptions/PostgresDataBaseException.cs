namespace TourPlanner.DAL.Exceptions
{
    public class PostgresDataBaseException : Exception
    {
        public PostgresDataBaseException() { }
        public PostgresDataBaseException(string message) : base(message) { }
        public PostgresDataBaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
