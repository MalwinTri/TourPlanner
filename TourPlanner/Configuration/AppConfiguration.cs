using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.DAL.Postgres;

namespace TourPlanner.Configuration
{
    internal class AppConfiguration : ITourPlannerPostgresRepositoryConfiguration
    {
        private readonly IConfiguration _configuration;

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration["postgres:connectionstring"]!;
        public string Username => _configuration["postgres:username"]!;
        public string Password => _configuration["postgres:password"]!;
    }
}
