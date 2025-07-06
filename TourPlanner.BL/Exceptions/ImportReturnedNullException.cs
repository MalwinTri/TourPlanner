namespace TourPlanner.BL.Exceptions
{
    public class ImportReturnedNullException : Exception
    {
        public ImportReturnedNullException() { }
        public ImportReturnedNullException(string objectIsNull) : base(objectIsNull) { }
        public ImportReturnedNullException(string message, Exception innerException) : base(message, innerException) { }
    }
}
