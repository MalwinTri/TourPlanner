using System.Data;
using TourPlanner.Logging;
using TourPlanner.Models;
using Newtonsoft.Json;
using TourPlanner.BL.Exceptions;

namespace TourPlanner.BL.Import
{
    public class ImportGenerator : IImportManager
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerManager _tourManager;
        private readonly ITourPlannerLogManager _tourLogManager;
        private readonly ITourPlannerGenerator _tourGenerator;

        public ImportGenerator(ITourPlannerManager tourManager, ITourPlannerLogManager tourLogManager, ITourPlannerGenerator tourGenerator, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ImportGenerator>();

            _tourManager = tourManager;
            _tourLogManager = tourLogManager;
            _tourGenerator = tourGenerator;
        }

        public async Task<Tour> ImportTour(string tourPath)
        {
            try
            {
                var existingTours = _tourManager.FindMatchingTours();

                // Read the JSON file
                var json = await File.ReadAllTextAsync(tourPath);

                // Deserialize the JSON into objects
                var deserializedObject = JsonConvert.DeserializeObject<dynamic>(json);


                if (deserializedObject == null)
                {
                    //todo throw custom exception

                    _logger.Error("Deserialized object is null");
                    throw new ImportReturnedNullException("Deserialized object is null");
                }

                Tour tourObject = deserializedObject.tour.ToObject<Tour>();

                if (tourObject == null)
                {
                    _logger.Error("Tour is null");
                    throw new ImportReturnedNullException("Tour is null");
                }

                if (existingTours.Any(t => t.Name == tourObject.Name))
                {
                    _logger.Error("Tour already exists");
                    throw new DuplicateNameException("Tour already exists");
                }

                await _tourGenerator.LoadImage(tourObject);
                await _tourManager.Add(tourObject);

                List<TourLog> tourLogObject = deserializedObject.tourLogs.ToObject<List<TourLog>>();


                tourLogObject ??= new List<TourLog>();

                foreach (var tourLog in tourLogObject)
                {
                    _tourLogManager.Add(tourLog);
                }



                return tourObject;
            }
            catch
            {
                throw new Exception();
            }
            //catch (MapquestReturnedNullException e)
            //{
            //    _logger.Error("Mapquest error");
            //    if (e.InnerException != null)
            //    {
            //        throw new ImportReturnedNullException("Mapquest error", e.InnerException);
            //    }
            //    else
            //    {
            //        throw new ImportReturnedNullException("Mapquest error", e);
            //    }
            //}

        }
    }
}
