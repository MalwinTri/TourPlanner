using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Logging;

namespace TourPlanner.DAL.Postgres
{
    public class TourPlannerPostgresRepository : ITourPlannerRepository
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;

        public TourPlannerPostgresRepository(ITourPlannerPostgresRepositoryConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TourPlannerPostgresRepository>();

            var stringBuilder = new NpgsqlConnectionStringBuilder(configuration.ConnectionString)
            {
                Username = configuration.Username,
                Password = configuration.Password
            };
            _connectionString = stringBuilder.ConnectionString;

            var context = new TourPlannerDbContext(_connectionString);
            //context.Database.EnsureCreated();
        }
    }
}
