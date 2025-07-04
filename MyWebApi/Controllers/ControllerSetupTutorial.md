# ASP.NET Core Minimal API to Controller-based API: Step-by-Step Tutorial

## Index

- [1. Installing Swagger Package](#1-installing-swagger-package)
- [2. Registering Services](#2-registering-services)
- [3. Using Middleware](#3-using-middleware)
- [4. Mapping Endpoints](#4-mapping-endpoints)
- [5. Use() vs Map()](#5-use-vs-map)
- [6. Adding a Controller](#6-adding-a-controller)
- [7. Why Inherit from ControllerBase?](#7-why-inherit-from-controllerbase)
- [8. Accessing ModelState and User](#8-accessing-modelstate-and-user)
- [9. What Does ApiController Attribute Do?](#9-what-does-apicontroller-attribute-do)
- [10. Middleware Routing in ASP.NET Core](#10--understanding-appmapcontrollers-vs-userouting--useendpoints)
- [11. Route attribute in ASP.NET Core](#11--understanding-the-route-attribute-in-aspnet-core)
- [12. Summary of Steps](#12-summary-of-steps)

---

## 1. Installing Swagger Package

To enable Swagger/OpenAPI support, you need to add the Swashbuckle.AspNetCore NuGet package to your project.  
Run the following command in your terminal:

```bash
dotnet add package Swashbuckle.AspNetCore
```

---

## 2. Registering Services

Before you can use controllers or Swagger/OpenAPI documentation, you must register the necessary services in your `Program.cs`:

```
builder.Services.AddEndpointsApiExplorer(); // For minimal API endpoint discovery
builder.Services.AddSwaggerGen();           // For Swagger/OpenAPI generation
builder.Services.AddControllers();          // For controller support
```

- **AddEndpointsApiExplorer**: Registers the API explorer for minimal APIs.
- **AddSwaggerGen**: Registers Swagger/OpenAPI generator for API documentation.
- **AddControllers**: Registers support for attribute-routed controllers.

---

## 3. Using Middleware

Middleware is added to the HTTP request pipeline using `app.UseXxx()` methods. For example:

```
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Serves the Swagger/OpenAPI JSON
    app.UseSwaggerUI();  // Serves the interactive Swagger UI
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
```

- **UseSwagger**: Adds middleware to serve the OpenAPI JSON.
- **UseSwaggerUI**: Adds middleware to serve the Swagger UI web page.
- **UseHttpsRedirection**: Redirects HTTP requests to HTTPS.

---

## 4. Mapping Endpoints

Endpoints are mapped using `app.MapXxx()` methods. For controllers:

```
app.MapControllers();
```

- **MapControllers**: Maps attribute-routed controllers as endpoints in the app.

---

## 5. Use() vs Map()

- **Use()**: Adds middleware to the pipeline. Middleware can process all requests and responses.
- **Map()**: Defines endpoints and connects specific routes/URLs to handlers (like controllers or minimal APIs).

**Summary:**
- `Use()` = Add middleware (e.g., authentication, logging, Swagger, HTTPS redirection)
- `Map()` = Define how URLs are routed to your code (e.g., controllers, minimal APIs)

---

## 6. Adding a Controller

Create a new controller in the `Controllers` folder, e.g., `HelloWorldController.cs`:

```
using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello, World!");
        }
    }
}
```

- **[ApiController]**: Enables API-specific behaviors (automatic model validation, etc.).
- **[Route("api/[controller]")]**: Sets the route to `api/helloworld`.
- **ControllerBase**: Base class for API controllers (see below).

---

## 7. Why Inherit from ControllerBase?

- **ControllerBase** provides core features for Web API controllers:
  - Methods like `Ok()`, `NotFound()`, `BadRequest()`, etc., for standard HTTP responses.
  - Access to `ModelState` for validation.
  - Access to the current authenticated `User`.
- It does **not** include view-related features (those are in `Controller`).
- Use `ControllerBase` for APIs that return data (JSON, XML), not views.

---

## 8. Accessing ModelState and User

- **ModelState**: Use `ModelState.IsValid` to check if incoming data is valid.
- **User**: Use the `User` property to access the current authenticated user.

---

## 9. What Does ApiController Attribute Do?

The `[ApiController]` attribute enables several helpful features for your API controllers:

### 1. Attribute Routing Support
- Controllers with `[ApiController]` use **attribute routing** by default.
- Example:
  ```csharp
  [Route("api/[controller]")]
  public class HelloWorldController : ControllerBase { ... }
  ```
  This means the route is automatically set to `/api/helloworld`.

### 2. Automatic Model Validation
- If the incoming request model is invalid, the framework automatically returns a `400 Bad Request` with error details.
- Example:
  ```csharp
  [HttpPost]
  public IActionResult Post([FromBody] MyModel model)
  {
      // No need to check ModelState.IsValid manually
      ...
  }
  ```

### 3. Automatic Problem Details Responses
- On validation errors or certain exceptions, the response is formatted as a [Problem Details](https://datatracker.ietf.org/doc/html/rfc7807) object (standardized error response).
- Example error response:
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
      "Name": ["The Name field is required."]
    }
  }
  ```

### 4. Parameter Binding Improvements
- `[ApiController]` infers parameter binding sources (e.g., `[FromBody]`, `[FromRoute]`, `[FromQuery]`) so you often don't need to specify them explicitly.

---
## 10. 🔀 Understanding `app.MapControllers()` vs `UseRouting()` + `UseEndpoints()`

In ASP.NET Core, both of these setups help route incoming HTTP requests to your controllers. However, they apply to **different versions and hosting models**.

---

### ✅ 1. Using `app.MapControllers()` (Modern Minimal Hosting)

This is the **preferred approach in .NET 6+** using the new **minimal hosting model** (`Program.cs` only, no `Startup.cs`).

#### 🔸 Code Example

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add controller support
builder.Services.AddControllers();

var app = builder.Build();

// Maps attribute-routed controllers
app.MapControllers();

app.Run();
```

#### 🔍 Notes
- Simple and clean
- No need for `UseRouting()` or `UseEndpoints(...)`
- Works well with attribute routing like `[Route("api/[controller]")]`

---

### ✅ 2. Using `app.UseRouting()` + `app.UseEndpoints(...)` (Classic Middleware Style)

This is used in **.NET Core 3.1 / 5** or when you're using a **Startup.cs file**.

#### 🔸 Code Example (inside `Startup.cs → Configure` method)

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Enable routing middleware
    app.UseRouting();

    // Optional: authz/authn goes here
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controller endpoints
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

#### 🔍 Notes
- More verbose, but flexible
- Required if you're using custom middleware pipeline
- Must call `UseRouting()` **before** `UseEndpoints()`

---

## 🔁 Quick Comparison

| Feature                    | `app.MapControllers()`         | `UseRouting()` + `UseEndpoints()`      |
|----------------------------|--------------------------------|-----------------------------------------|
| Introduced In              | .NET 6+ (minimal hosting)      | .NET Core 3.1 / 5                       |
| Simpler Syntax             | ✅ Yes                          | ❌ No (more verbose)                    |
| Attribute Routing Support  | ✅ Yes                          | ✅ Yes                                  |
| Pipeline Control           | 🔸 Limited                      | ✅ More control                         |
| Needs Startup.cs           | ❌ No                           | ✅ Yes

## 11. 🧭 Understanding the `[Route]` Attribute in ASP.NET Core

The `[Route]` attribute in ASP.NET Core Web API is used to **map incoming HTTP requests to the correct controller and action methods** based on the URL.

---

### 🔹 What Does `[Route]` Do?

- It defines the **URL pattern** that an API controller or action responds to.
- You can apply it at the **controller level** (base route) and/or **action level** (specific endpoints).

---

### 📌 Route Tokens

ASP.NET Core allows **tokens** inside the route for dynamic resolution:

| Token         | Description                                     |
|---------------|-------------------------------------------------|
| `[controller]`| Replaced by the controller class name (minus `Controller`) |
| `[action]`    | Replaced by the method name                     |
| `[area]`      | Used for MVC Areas (if applicable)              |

---

## 🧪 Examples

### ✅ Example 1: `[Route("api/[controller]")]`

```csharp
[Route("api/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Today's forecast");
}
```

📍 **Responds to:** `GET /api/weatherforecast`

---

### ✅ Example 2: Add Action-Specific Route

```csharp
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet("featured")]
    public IActionResult GetFeatured() => Ok("Featured products");
}
```

📍 **Responds to:** `GET /api/products/featured`

---

### ✅ Example 3: Use Route Parameters

```csharp
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetById(int id) => Ok($"Product ID: {id}");
}
```

📍 **Responds to:** `GET /api/products/42`

---

### ✅ Example 4: Absolute Route on Controller

```csharp
[Route("store/products")]
[ApiController]
public class StoreController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Store products list");
}
```

📍 **Responds to:** `GET /store/products`

---

## 🧠 Real-World Analogy

> 🧑‍🍳 Think of `[Route]` like a **restaurant menu item code**.
>
> The **URL is the customer order**, and the `[Route]` tells the API **which kitchen station (controller + method)** will fulfill it.

---

## 📌 Summary

| Element         | Purpose                                      |
|------------------|----------------------------------------------|
| `[Route]`        | Binds URL path to controller/action           |
| `[HttpGet("x")]` | Shorthand for GET + route mapping             |
| Tokens like `[controller]` | Replaceable placeholders             |
| Works With       | `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, etc. |

---

✅ Use `[Route]` to cleanly organize your API endpoints and make them predictable and RESTful.


## 12. Summary of Steps

1. **Install Swagger package:** `dotnet add package Swashbuckle.AspNetCore`
2. **Register services:** `AddEndpointsApiExplorer`, `AddSwaggerGen`, `AddControllers`.
3. **Add middleware:** `UseSwagger`, `UseSwaggerUI`, `UseHttpsRedirection`.
4. **Map endpoints:** `MapControllers`.
5. **Create controllers** inheriting from `ControllerBase`.
6. **Use `ModelState` and `User`** as needed in your controllers.
7. **Decorate controllers with `[ApiController]`** for enhanced API behaviors.

---

This file serves as a quick reference and tutorial for setting up and understanding controller-based APIs in ASP.NET Core.