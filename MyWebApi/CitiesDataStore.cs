using MyWebApi.Models;

namespace MyWebApi
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        // Singleton pattern for demo purposes
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore()
        {
            Cities = new List<CityDto>
            {
                new CityDto
                {
                    Id = 1, Name = "New York", Description = "The city that never sleeps.",
                    PointsOfInterest = new List<PointsOfInterestDto>
                    {
                        new PointsOfInterestDto { Id = 1, Name = "Statue of Liberty", Description = "A symbol of freedom." },
                        new PointsOfInterestDto { Id = 2, Name = "Central Park", Description = "A large public park in New York City." }
                    }
                },
                new CityDto
                {
                    Id = 2, Name = "Paris", Description = "The city of lights.",
                    PointsOfInterest = new List<PointsOfInterestDto>
                    {
                        new PointsOfInterestDto { Id = 3, Name = "Eiffel Tower", Description = "A wrought-iron lattice tower." },
                        new PointsOfInterestDto { Id = 4, Name = "Louvre Museum", Description = "The world's largest art museum." }
                    }
                },
                new CityDto
                {
                    Id = 3, Name = "Tokyo", Description = "The capital of Japan.",
                    PointsOfInterest = new List<PointsOfInterestDto>
                    {
                        new PointsOfInterestDto { Id = 5, Name = "Shinjuku Gyoen", Description = "A large park in Tokyo." },
                        new PointsOfInterestDto { Id = 6, Name = "Tokyo Tower", Description = "A communications and observation tower." }
                    }
                }
            };
        }
    }
}