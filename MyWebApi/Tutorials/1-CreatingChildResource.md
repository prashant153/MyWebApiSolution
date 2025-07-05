## Sample for Creating a Child Resource (Points of Interest for a City)

This tutorial demonstrates how to create a child resource in ASP.NET Core Web API.  
We'll use the example of "Points of Interest" for a city, where each city can have multiple points of interest.

---

### 1. Define the Child DTO

Create a `PointOfInterestDto` class in your `Models` folder:

```csharp
// Models/PointOfInterestDto.cs
public class PointOfInterestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

---

### 2. Update the Parent DTO

Add a collection of points of interest to your `CityDto`:

```csharp
// Models/CityDto.cs
public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<PointOfInterestDto> PointsOfInterest { get; set; } = new();
}
```

---

### 3. Update the Data Store

Add sample points of interest to each city in your `CitiesDataStore`:

```csharp
// CitiesDataStore.cs
public CitiesDataStore()
{
    Cities = new List<CityDto>
    {
        new CityDto
        {
            Id = 1,
            Name = "New York",
            Description = "The city that never sleeps.",
            PointsOfInterest = new List<PointOfInterestDto>
            {
                new PointOfInterestDto { Id = 1, Name = "Central Park", Description = "A large public park in NYC." },
                new PointOfInterestDto { Id = 2, Name = "Empire State Building", Description = "A 102-story skyscraper." }
            }
        },
        // Add more cities as needed...
    };
}
```

---

### 4. Create the Child Resource Controller

Create a new controller called `PointsOfInterestController`:

```csharp
// Controllers/PointsOfInterestController.cs
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointId}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var point = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointId);
            if (point == null)
            {
                return NotFound();
            }
            return Ok(point);
        }
    }
}
```

---

### 5. Example Endpoints

- `GET /api/cities/1/pointsofinterest`  
  Returns all points of interest for city with ID 1.

- `GET /api/cities/1/pointsofinterest/2`  
  Returns the point of interest with ID 2 for city with ID 1.

---

### 6. Summary

- Define a child DTO and add it as a collection in the parent DTO.
- Update your data store to include child resources.
- Create a controller with routes that include the parent resource's ID.
- Use nested routes to access and manage child resources.

This pattern can be reused for any parent-child relationship in your APIs.
