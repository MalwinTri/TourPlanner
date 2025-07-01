using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Logging;
using TourPlanner.Models;

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

            using var context = new TourPlannerDbContext(_connectionString);
            context.Database.EnsureCreated();
        }

        public Tour Add(Tour tour)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);
                context.Add(tour);
                context.SaveChanges();

                _logger.Debug($"Added tour {tour.Name} with ID ({tour.Id})");
                return tour;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to add tour {tour.Name} with ID ({tour.Id})");
                throw new PostgresDataBaseException("Adding error", e);
            }
        }

        public TourLog? Add(TourLog tourLog)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);
                context.Add(tourLog);
                context.SaveChanges();

                _logger.Debug($"Added tour log with ID ({tourLog.Id})");
                return tourLog;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to add tour log with ID ({tourLog.Id})");
                throw new PostgresDataBaseException("Adding error", e);
            }
        }

        public Tour? Edit(Tour tour)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);

                context.Update(tour);
                context.SaveChanges();

                _logger.Debug($"Edited tour {tour.Name} with ID ({tour.Id})");
                return tour;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to edit tour {tour.Name} with ID ({tour.Id})");
                throw new PostgresDataBaseException("Editing error", e);
            }
        }

        public TourLog? Edit(TourLog tourLog)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);

                context.Update(tourLog);
                context.SaveChanges();

                _logger.Debug($"Edited tour log with ID ({tourLog.Id})");
                return tourLog;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to edit tour log with ID ({tourLog.Id})");
                throw new PostgresDataBaseException("Editing error", e);
            }
        }

        public IEnumerable<TourLog> GetAllTourLogsById(Guid tourId)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);

                _logger.Debug($"Retrieved all tour logs for tour with ID ({tourId})");
                return context.TourLogs
                    .Where(t => t.TourId.Equals(tourId))
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to retrieve all tour logs for tour with ID ({tourId})");
                throw new PostgresDataBaseException("Retrieving error", e);
            }
        }

        public IEnumerable<TourLog> GetAllTourLogsByText(string searchPattern)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);

                _logger.Debug($"Retrieved all tours matching search text ({searchPattern})");
                return context.TourLogs
                    .Where(t => t.Comment!.Contains(searchPattern))
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to retrieve all tours matching search text ({searchPattern})");
                throw new PostgresDataBaseException("Searching error", e);
            }
        }

        public IEnumerable<Tour> GetAllTours()
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);

                _logger.Debug("Retrieved all tours");
                return context.Tours.ToList();
            }
            catch (Exception e)
            {
                _logger.Error("Failed to retrieve all tours");
                throw new PostgresDataBaseException("Retrieving error", e);
            }
        }

        public IEnumerable<Tour> GetAllToursByText(string searchPattern)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);
                var searchTextUpper = searchPattern.ToUpper();
                List<Tour> tours;
                if (searchTextUpper.Contains("POPULARITY"))
                {
                    var searchTextRegex = Regex.Replace(searchTextUpper, "[^0-9]", "");
                    tours = context.Tours
                        .Where(t => t.Popularity.ToString()!.Contains(searchTextRegex))
                        .ToList();

                }
                else if (searchTextUpper.Contains("CHILDFRIENDLINESS"))
                {
                    var searchTextRegex = Regex.Replace(searchTextUpper, "[^0-9]", "");
                    tours = context.Tours
                        .Where(t => t.ChildFriendliness.ToString()!.Contains(searchTextRegex))
                        .ToList();
                }
                else
                {
                    tours = context.Tours
                        .Where(t => t.Name!.ToUpper().Contains(searchTextUpper)
                                    || t.From!.ToUpper().Contains(searchTextUpper)
                                    || t.To!.ToUpper().Contains(searchTextUpper)
                                    || t.Description!.ToUpper().Contains(searchTextUpper)
                                    || t.Popularity.ToString()!.Contains(searchTextUpper)
                                    || t.ChildFriendliness.ToString()!.Contains(searchTextUpper)
                                    || context.TourLogs.Any(log =>
                                        log.TourId == t.Id && log.Comment!.ToUpper().Contains(searchTextUpper)))
                        .ToList();
                }


                _logger.Debug($"Retrieved all tours matching search text ({searchPattern})");

                return tours;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to retrieve all tours matching search text ({searchPattern})");
                throw new PostgresDataBaseException("Searching error", e);
            }
        }

        public bool Remove(Tour tour)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);
                var t = context.Tours.Find(tour.Id);
                if (t == null) return false;
                if (!RemoveImage(t)) return false;
                context.Tours.Remove(t);
                context.SaveChanges();

                _logger.Debug($"Removed tour {tour.Name} with ID ({tour.Id})");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to remove tour {tour.Name} with ID ({tour.Id})");
                throw new PostgresDataBaseException("Removing error", e);
            }
        }

        public bool Remove(TourLog tourLog)
        {
            try
            {
                using var context = new TourPlannerDbContext(_connectionString);
                var t = context.TourLogs.Find(tourLog.Id);
                if (t == null) return false;
                context.TourLogs.Remove(t);
                context.SaveChanges();

                _logger.Debug($"Removed tour log with ID ({tourLog.Id})");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to remove tour log with ID ({tourLog.Id})");
                throw new PostgresDataBaseException("Removing error", e);
            }
        }

        private static bool RemoveImage(Tour tour)
        {
            var path = tour.ImagePath;
            if (path == null) return true;
            File.Delete(path);
            return true;
        }
    }
}
