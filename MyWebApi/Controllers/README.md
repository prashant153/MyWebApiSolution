## 🧱 Understanding MVC in .NET Core Web API

### What is MVC?

**MVC** stands for:

| Component     | Purpose                                |
|---------------|----------------------------------------|
| **Model**     | Represents the data & business rules   |
| **View**      | UI layer (HTML in web apps, **JSON in Web API**) |
| **Controller**| Handles HTTP requests and responses    |

In a Web API, we **don’t use Views (HTML)** — instead, we return **JSON**. So the pattern is often simplified to just **Model + Controller**.

---

### 🍽 Real-World Analogy – The Restaurant

Imagine your Web API as a restaurant:

| MVC Part       | Role                          | Analogy       |
|----------------|-------------------------------|---------------|
| Controller     | Takes HTTP request, sends response | The **Waiter** |
| Model          | Contains data and logic        | The **Kitchen** |
| View (JSON)    | What the client receives       | The **Plate of food** |

> 🧠 The waiter (Controller) takes an order, gives it to the kitchen (Model/Logic), and returns the dish (JSON/View) to the customer (client).

---

### 🧩 MVC in a Layered Architecture

In a typical `N-Layered` architecture:

```
Presentation (Controllers) → Application (Services, DTOs)
                     ↓
           Domain (Entities, Interfaces)
                     ↓
        Infrastructure (Database, APIs)
```

| MVC Element   | Fits In This Layer               |
|---------------|----------------------------------|
| Controller    | Presentation (API Layer)         |
| Model (DTOs)  | Application Layer                |
| Model (Entities) | Domain Layer                  |
| View (JSON)   | Automatically handled by Web API |

---

### 🧪 Example

#### 🔸 Model

```csharp
public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
}
```

#### 🔸 Controller

```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = "Chilly"
        });
    }
}
```

#### 🔸 View (JSON output in Postman/Browser)

```json
[
  {
    "date": "2025-06-29T00:00:00",
    "temperatureC": 25,
    "summary": "Cool"
  }
]
```

---

### ✅ Summary

- MVC is used at the **top layer** (Presentation Layer) in Web APIs.
- You don't use HTML Views — **JSON is your view**.
- Keep business logic and data access **out of the controllers** and push it to the Application & Domain layers.

