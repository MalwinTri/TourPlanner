namespace TourPlanner.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);

    }
}
