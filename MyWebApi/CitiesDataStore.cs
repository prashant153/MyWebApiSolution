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
                new CityDto { Id = 1, Name = "New York", Description = "The city that never sleeps." },
                new CityDto { Id = 2, Name = "Paris", Description = "The city of lights." },
                new CityDto { Id = 3, Name = "Tokyo", Description = "The capital of Japan." }
            };
        }
    }
}