namespace TourPlanner.BL.Exceptions
{
    public class OpenRouteServicemanagerException : Exception
    {
        public OpenRouteServicemanagerException() { }

        public OpenRouteServicemanagerException(string message) : base(message) { }

        public OpenRouteServicemanagerException(string message, Exception innerException): base(message, innerException) { }
    }
}
