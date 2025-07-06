namespace TourPlanner.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger<TContext>();
    }
}
