using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.DAL.Postgres
{
    public interface ITourPlannerPostgresRepositoryConfiguration
    {
        string ConnectionString { get; }
        string Username { get; }
        string Password { get; }
    }
}
