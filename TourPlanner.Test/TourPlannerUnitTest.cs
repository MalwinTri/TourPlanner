using TourPlanner.BL.iText;
using TourPlanner.DAL;
using TourPlanner.Models;
using TourPlanner.Logging;
using TourPlanner.BL;
using TourPlanner.BL.Enum;

namespace TourPlanner.Test
{
    public class InMemoryTourPlannerRepository : ITourPlannerRepository
    {
        private readonly List<Tour> _tours = new();
        private readonly List<TourLog> _logs = new();

        public Task<Tour> AddAsync(Tour tour)
        {
            _tours.Add(tour);
            return Task.FromResult(tour);
        }

        public Tour? Edit(Tour tour)
        {
            var idx = _tours.FindIndex(t => t.Id == tour.Id);
            if (idx == -1) return null;
            _tours[idx] = tour;
            return tour;
        }

        public bool Remove(Tour tour)
        {
            return _tours.RemoveAll(t => t.Id == tour.Id) > 0;
        }



        public TourLog? Edit(TourLog tourLog)
        {
            var idx = _logs.FindIndex(l => l.Id == tourLog.Id);
            if (idx == -1) return null;
            _logs[idx] = tourLog;
            return tourLog;
        }

        public bool Remove(TourLog tourLog)
        {
            return _logs.RemoveAll(l => l.Id == tourLog.Id) > 0;
        }

        public IEnumerable<Tour> GetAllTours() => _tours;

        public IEnumerable<Tour> GetAllToursByText(string searchPattern)
            => _tours.Where(t => t.Name != null && t.Name.Contains(searchPattern, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<TourLog> GetAllTourLogsById(Guid tourId)
            => _logs.Where(l => l.TourId == tourId);

        public IEnumerable<TourLog> GetAllTourLogsByText(string searchPattern)
            => _logs.Where(l => l.Comment != null && l.Comment.Contains(searchPattern, StringComparison.OrdinalIgnoreCase));


        public Task<TourLog?> AddAsync(TourLog tourLog)
        {
            _logs.Add(tourLog);
            return Task.FromResult<TourLog?>(tourLog);
    }

        public Task<Tour> AddTourWithLogsAsync(Tour tour, List<TourLog> tourLogs)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyLogger : ILogger
    {
        public List<string> Messages { get; } = new();
        public void Debug(string message) => Messages.Add("DEBUG: " + message);
        public void Warning(string message) => Messages.Add("WARN: " + message);
        public void Error(string message) => Messages.Add("ERROR: " + message);
        public void Fatal(string message) => Messages.Add("FATAL: " + message);
    }

    public class DummyLoggerFactory : ILoggerFactory
    {
        private readonly DummyLogger _logger = new();
        public ILogger CreateLogger<TContext>() => _logger;
        public DummyLogger Logger => _logger;
    }

    public class DummyItextConfig : IItextConfiguration
    {
        public string OutputPath { get; set; } = Path.GetTempPath();
    }

    public class DummyTourManager : ITourPlannerManager
    {
        private readonly List<Tour> _tours;
        public DummyTourManager(List<Tour> tours) => _tours = tours;
        public Task<Tour?> Add(Tour tour) { _tours.Add(tour); return Task.FromResult<Tour?>(tour); }
        public Task<Tour?> Edit(Tour tour, edit mode = edit.Generate) { return Task.FromResult<Tour?>(tour); }
        public void DeleteTour(Tour tour) => _tours.Remove(tour);
        public IEnumerable<Tour> FindMatchingTours(string? searchText = null)
            => string.IsNullOrEmpty(searchText) ? _tours : _tours.Where(t => t.Name != null && t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        public int CalculatePopularity(Tour tour) => 1;
        public int CalculateChildFriendliness(Tour tour) => 1;
    }

    public class DummyTourLogManager : ITourPlannerLogManager
    {
        private readonly List<TourLog> _logs;
        public DummyTourLogManager(List<TourLog> logs) => _logs = logs;
        public Task<TourLog?> AddAsync(TourLog tourLog) { _logs.Add(tourLog); return Task.FromResult<TourLog?>(tourLog); }
        public TourLog? Edit(TourLog tourLog) { return tourLog; }
        public void Delete(TourLog tour) => _logs.Remove(tour);
        public IEnumerable<TourLog> FindMatchingTourLogs(Tour tour) => _logs.Where(l => l.TourId == tour.Id);
    }

    [TestFixture]
    public class RepositoryAndReportTests
    {
        private InMemoryTourPlannerRepository _repo;
        private Tour _tour;
        private TourLog _log;

        [SetUp]
        public void Setup()
        {
            _repo = new InMemoryTourPlannerRepository();
            _tour = new Tour(Guid.NewGuid(), "Alpen", "Schöne Tour", "Wien", "Graz", "car");
            _log = new TourLog(Guid.NewGuid(), _tour.Id, DateTime.Now, "Super!", 2, 3600, 5);
        }

        [Test] public async Task Test_01_Add_Tour_Works() { var t = await _repo.AddAsync(_tour); Assert.AreEqual(_tour, t); }
        [Test] public void Test_02_Edit_Tour_Works() { _repo.AddAsync(_tour); _tour.Name = "Alpen2"; var t = _repo.Edit(_tour); Assert.AreEqual("Alpen2", t?.Name); }
        [Test] public void Test_03_Remove_Tour_Works() { _repo.AddAsync(_tour); Assert.IsTrue(_repo.Remove(_tour)); }
        [Test] public async Task Test_04_Add_TourLog_Works() { var l = await _repo.AddAsync(_log); Assert.AreEqual(_log, l); }
        [Test] public void Test_05_Edit_TourLog_Works() { _repo.AddAsync(_log); _log.Comment = "Geändert"; var l = _repo.Edit(_log); Assert.AreEqual("Geändert", l?.Comment); }
        [Test] public void Test_06_Remove_TourLog_Works() { _repo.AddAsync(_log); Assert.IsTrue(_repo.Remove(_log)); }
        [Test] public void Test_07_GetAllTours_ReturnsAll() { _repo.AddAsync(_tour); Assert.AreEqual(1, _repo.GetAllTours().Count()); }
        [Test] public void Test_08_GetAllToursByText_FindsTour() { _repo.AddAsync(_tour); Assert.AreEqual(1, _repo.GetAllToursByText("Alpen").Count()); }
        [Test] public void Test_09_GetAllTourLogsById_FindsLog() { _repo.AddAsync(_log); Assert.AreEqual(1, _repo.GetAllTourLogsById(_tour.Id).Count()); }
        [Test] public void Test_10_GetAllTourLogsByText_FindsLog() { _repo.AddAsync(_log); Assert.AreEqual(1, _repo.GetAllTourLogsByText("Super").Count()); }

        [Test]
        public void Test_11_GetAllToursByText_ReturnsEmpty_IfNoMatch()
        {
            _repo.AddAsync(_tour);
            var result = _repo.GetAllToursByText("Not found");
            Assert.IsEmpty(result);
        }


        [Test]
        public void Test_12_GetAllTourLogsByText_ReturnsEmpty_IfNoMatch()
        {
            _repo.AddAsync(_log);
            var result = _repo.GetAllTourLogsByText("Not found");
            Assert.IsEmpty(result);
        }

            var generator = new iTextReportGenerator(dummyTourManager, dummyLogManager, dummyConfig, dummyLoggerFactory);
            var result = generator.GenerateReport(_tour);
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_13_GenerateSummary_CreatesPdf()
        {
            var logs = new List<TourLog> { _log };
            var tours = new List<Tour> { _tour };
            var dummyTourManager = new DummyTourManager(tours);
            var dummyLogManager = new DummyTourLogManager(logs);
            var dummyConfig = new DummyItextConfig();
            var dummyLoggerFactory = new DummyLoggerFactory();

            _tour.ImagePath = Path.Combine(Path.GetTempPath(), "dummyimg3.png");
            File.WriteAllBytes(_tour.ImagePath, new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

            var generator = new iTextReportGenerator(dummyTourManager, dummyLogManager, dummyConfig, dummyLoggerFactory);
            var result = generator.GenerateSummary();
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_14_GenerateSummary_NoLogs_AddsNoLogsFound()
        {
            var dummyTourManager = new DummyTourManager(new List<Tour> { _tour });
            var dummyLogManager = new DummyTourLogManager(new List<TourLog>());
            var dummyConfig = new DummyItextConfig();
            var dummyLoggerFactory = new DummyLoggerFactory();

            _tour.ImagePath = Path.Combine(Path.GetTempPath(), "dummyimg4.png");
            File.WriteAllBytes(_tour.ImagePath, new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

            var generator = new iTextReportGenerator(dummyTourManager, dummyLogManager, dummyConfig, dummyLoggerFactory);
            var result = generator.GenerateSummary();
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_15_GetAllTourLogsById_ReturnsAllLogsForTour()
        {
            var log1 = new TourLog(Guid.NewGuid(), _tour.Id, DateTime.Now, "Log1", 1, 1000, 3);
            var log2 = new TourLog(Guid.NewGuid(), _tour.Id, DateTime.Now, "Log2", 2, 2000, 4);
            var logOther = new TourLog(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Other", 3, 3000, 5); 

            _repo.AddAsync(_tour);
            _repo.AddAsync(log1);
            _repo.AddAsync(log2);
            _repo.AddAsync(logOther);

            var logsForTour = _repo.GetAllTourLogsById(_tour.Id);

            Assert.AreEqual(2, logsForTour.Count());
            Assert.IsTrue(logsForTour.Any(l => l.Id == log1.Id));
            Assert.IsTrue(logsForTour.Any(l => l.Id == log2.Id));
        }


        [Test]
        public void Test_16_Edit_Tour_DoesNotChangeCount()
        {
            _repo.AddAsync(_tour);
            var originalCount = _repo.GetAllTours().Count();
            _tour.Name = "Geändert";
            _repo.Edit(_tour);
            Assert.AreEqual(originalCount, _repo.GetAllTours().Count());
        }

            var generator = new iTextReportGenerator(dummyTourManager, dummyLogManager, dummyConfig, dummyLoggerFactory);
            var result = generator.GenerateSummary();
            Assert.IsTrue(Directory.Exists(tempDir));
        }
        [Test]
        public void Test_17_Edit_Tour_ReturnsNull_IfNotExists()
        {
            var t = new Tour(Guid.NewGuid(), "NichtDa", "x", "x", "x", "car");
            Assert.IsNull(_repo.Edit(t));
        }

        [Test]
        public void Test_18_Edit_TourLog_ReturnsNull_IfNotExists()
        {
            var l = new TourLog(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "x", 1, 1, 1);
            Assert.IsNull(_repo.Edit(l));
        }

        [Test]
        public void Test_19_Remove_Tour_ReturnsFalse_IfNotExists()
        {
            var t = new Tour(Guid.NewGuid(), "NichtDa", "x", "x", "x", "car");
            Assert.IsFalse(_repo.Remove(t));
        }

        [Test]
        public void Test_20_Remove_TourLog_ReturnsFalse_IfNotExists()
        {
            var l = new TourLog(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "x", 1, 1, 1);
            Assert.IsFalse(_repo.Remove(l));
        }
    }
}