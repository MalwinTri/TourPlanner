using TourPlanner.BL.Enum;
using TourPlanner.DAL;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL
{
    public class TourPlannerManager : ITourPlannerManager
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerRepository _repository;
        private readonly ITourPlannerLogManager _logManager;
        private readonly ITourPlannerGenerator _generator;

        public TourPlannerManager(ITourPlannerRepository repository, ITourPlannerLogManager tourLogManager, ITourPlannerGenerator generator, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TourPlannerManager>();

            _repository = repository;
            _logManager = tourLogManager;
            _generator = generator;
        }

        public async Task<Tour?> Add(Tour tour)
        {
            try
            {
                var t = await _generator.GenerateTourFromTourAsync(tour);
                if (t == null)
                {
                    _logger.Error("Generator returned null tour");
                    throw new ArgumentException("Generator returned null tour");
                }
                t.Popularity = CalculatePopularity(t);
                t.ChildFriendliness = CalculateChildFriendliness(t);
                _repository.Add(t);

                _logger.Debug($"Added tour {t.Name}");
                return t;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public int CalculateChildFriendliness(Tour tour)
        {
            try
            {
                var logs = _logManager.FindMatchingTourLogs(tour).ToList();
                if (!logs.Any()) return 0;


                var totalDifficulty = logs.Aggregate(0.0, (current, log) => current + (log.Difficulty ?? 0.0));
                var meanDifficulty = totalDifficulty / logs.Count;

                var childFriendliness = meanDifficulty switch
                {
                    < 2.0 => 2,
                    < 3.0 => 1,
                    _ => 0
                };

                switch (tour.Transport)
                {
                    case "Walking":
                        switch (tour.Distance)
                        {
                            case < 1:
                                childFriendliness += 3;
                                break;
                            case < 4:
                                childFriendliness += 2;
                                break;
                        }

                        break;
                    case "Car":
                        switch (tour.Distance)
                        {
                            case < 50:
                                childFriendliness += 3;
                                break;
                            case < 200:
                                childFriendliness += 2;
                                break;
                        }

                        break;
                    case "Bike":
                        switch (tour.Distance)
                        {
                            case < 5:
                                childFriendliness += 3;
                                break;
                            case < 15:
                                childFriendliness += 2;
                                break;
                        }

                        break;
                }

                var timeDiff = logs.Select(log => (log.TotalTime - tour.Time ?? 0.0)).ToList();
                var meanTimeDiff = timeDiff.Aggregate(0.0, (current, diff) => current + diff) / timeDiff.Count;
                if (meanTimeDiff < 0) childFriendliness += 1;

                return childFriendliness switch
                {
                    < 1 => 1,
                    > 5 => 5,
                    _ => childFriendliness
                };

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                //throw new TourPlannerManagerException("CalculateChildFriendliness error", e);
                throw new Exception("CalculateChildFriendliness error", e);
            }

        }

        public int CalculatePopularity(Tour tour)
        {
            try
            {
                var logs = _logManager.FindMatchingTourLogs(tour);
                var currentDate = DateTime.Now;
                var twoWeeksAgo = currentDate.AddDays(-14);
                var popularityCount = logs.Select(log => log.Date)
                    .Count(logDate => logDate >= twoWeeksAgo && logDate <= currentDate);

                return popularityCount switch
                {
                    < 2 => 1,
                    < 5 => 2,
                    _ => 3
                };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                //throw new TourPlannerManagerException("CalculatePopularity error", e);
                throw new Exception("CalculatePopularity error", e);
            }
        }

        public void DeleteTour(Tour tour)
        {
            try
            {
                _repository.Remove(tour);
                var logs = _logManager.FindMatchingTourLogs(tour);
                foreach (var log in logs)
                {
                    _logManager.Delete(log);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public async Task<Tour?> Edit(Tour tour, edit mode = edit.Generate)
        {
            try
            {
                var t = tour;
                if (mode == edit.Generate)
                {
                    t = await _generator.GenerateTourFromTourAsync(tour);
                }

                if (t == null)
                {
                    _logger.Error("Generator returned null tour");
                    throw new ArgumentException("Generator returned null tour");
                }

                t.Popularity = CalculatePopularity(t);
                t.ChildFriendliness = CalculateChildFriendliness(t);
                _repository.Edit(t);
                return t;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public IEnumerable<Tour> FindMatchingTours(string? searchText = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return _repository.GetAllTours();
                }

                return _repository.GetAllToursByText(searchText);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
