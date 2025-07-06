namespace TourPlanner.DAL.Postgres
{
    public interface ITourPlannerPostgresRepositoryConfiguration
    {
        string ConnectionString { get; }
        string Username { get; }
        string Password { get; }
    }
}
