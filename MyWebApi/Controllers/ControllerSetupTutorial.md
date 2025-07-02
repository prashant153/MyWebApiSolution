# ASP.NET Core Minimal API to Controller-based API: Step-by-Step Tutorial

## Index

- [1. Registering Services](#1-registering-services)
- [2. Using Middleware](#2-using-middleware)
- [3. Mapping Endpoints](#3-mapping-endpoints)
- [4. Use() vs Map()](#4-use-vs-map)
- [5. Adding a Controller](#5-adding-a-controller)
- [6. Why Inherit from ControllerBase?](#6-why-inherit-from-controllerbase)
- [7. Accessing ModelState and User](#7-accessing-modelstate-and-user)
- [8. Summary of Steps](#8-summary-of-steps)

---

## 1. Registering Services

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

## 2. Using Middleware

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

## 3. Mapping Endpoints

Endpoints are mapped using `app.MapXxx()` methods. For controllers:

```
app.MapControllers();
```

- **MapControllers**: Maps attribute-routed controllers as endpoints in the app.

---

## 4. Use() vs Map()

- **Use()**: Adds middleware to the pipeline. Middleware can process all requests and responses.
- **Map()**: Defines endpoints and connects specific routes/URLs to handlers (like controllers or minimal APIs).

**Summary:**
- `Use()` = Add middleware (e.g., authentication, logging, Swagger, HTTPS redirection)
- `Map()` = Define how URLs are routed to your code (e.g., controllers, minimal APIs)

---

## 5. Adding a Controller

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

## 6. Why Inherit from ControllerBase?

- **ControllerBase** provides core features for Web API controllers:
  - Methods like `Ok()`, `NotFound()`, `BadRequest()`, etc., for standard HTTP responses.
  - Access to `ModelState` for validation.
  - Access to the current authenticated `User`.
- It does **not** include view-related features (those are in `Controller`).
- Use `ControllerBase` for APIs that return data (JSON, XML), not views.

---

## 7. Accessing ModelState and User

- **ModelState**: Use `ModelState.IsValid` to check if incoming data is valid.
- **User**: Use the `User` property to access the current authenticated user.

---

## 8. Summary of Steps

1. Register services: `AddEndpointsApiExplorer`, `AddSwaggerGen`, `AddControllers`.
2. Add middleware: `UseSwagger`, `UseSwaggerUI`, `UseHttpsRedirection`.
3. Map endpoints: `MapControllers`.
4. Create controllers inheriting from `ControllerBase`.
5. Use `ModelState` and `User` as needed in your controllers.

---

This file serves as a quick reference and tutorial for setting up and understanding controller-based APIs in ASP.NET Core.
