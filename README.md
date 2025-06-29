# 🚀 .NET Core Web API Starter (Using CLI)

This guide walks you through creating a simple .NET Core Web API using the `dotnet CLI`, step by step — no Visual Studio required.

---

## 🛠 Prerequisites

Make sure the following are installed:

- [.NET SDK (6.0 or later)](https://dotnet.microsoft.com/en-us/download)
- Terminal or Command Prompt
- Optional: Postman or `curl` to test the API

---

## ⚙️ Setup Instructions

### 🧩 Step 1: Create a Working Directory

Create a new folder for your project and navigate into it.

```bash
mkdir MyWebApiSolution
cd MyWebApiSolution
```

---

### 🧩 Step 2: Create a Solution File

Generate a solution file to group related projects.

```bash
dotnet new sln -n MyWebApiSolution
```

---

### 🧩 Step 3: Create the Web API Project

Scaffold a Web API project named `MyWebApi`.

```bash
dotnet new webapi -n MyWebApi
```

✅ This creates a folder named `MyWebApi/` with a default controller and Swagger UI enabled.

---

### 🧩 Step 4: Add the Web API Project to the Solution

Link the project to your solution file.

```bash
dotnet sln add MyWebApi/MyWebApi.csproj
```

---

### 🧩 Step 5: Run the Web API

Navigate into the project directory and run the API.

```bash
cd MyWebApi
dotnet run
```
Or you can run the following to choose a different profile (eg: https) - check your launchsettings
```bash
dotnet run --launch-profile "https"
```
You should see output like:

```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```

---

### 🧩 Step 6: Test the Default Endpoint

You can test the default `WeatherForecast` endpoint using:

**Browser (Swagger UI):**

```
https://localhost:5001/swagger
```

**Using curl:**

```bash
curl https://localhost:5001/weatherforecast --insecure
```

---

## 📁 Project Structure

Your folder structure should now look like this:

```
MyWebApiSolution/
│
├── MyWebApi/               # Web API project
│   ├── Controllers/
│   ├── Program.cs
│   └── MyWebApi.csproj
│
└── MyWebApiSolution.sln    # Solution file
```

---

## 🧹 Useful CLI Commands

### 🔄 Clean the solution:

```bash
dotnet clean
```

### 🏗️ Build the solution:

```bash
dotnet build
```

### 📦 Restore dependencies:

```bash
dotnet restore
```

---

### 🧩 Step 7 (Optional): Add a Class Library Project

To keep business logic separate:

```bash
dotnet new classlib -n MyWebApi.Core
dotnet sln add MyWebApi.Core/MyWebApi.Core.csproj
dotnet add MyWebApi/MyWebApi.csproj reference MyWebApi.Core/MyWebApi.Core.csproj
```

---

## 🚧 Next Steps

- Add your own controllers under `Controllers/`
- Create a `Models/` folder for DTOs and domain models
- Add database access via Entity Framework Core
- Add JWT authentication & role-based authorization
- Dockerize your app
- Set up CI/CD with GitHub Actions

---
## 📘 OpenAPI vs Swagger – What's the Difference?

Although often used interchangeably, **OpenAPI** and **Swagger** are not the same. Here's a quick overview:

| Term        | Description                                                                 |
|-------------|-----------------------------------------------------------------------------|
| **OpenAPI** | The official **specification/standard** for defining REST APIs in JSON/YAML |
| **Swagger** | A set of **tools** built around the OpenAPI spec (like Swagger UI, Editor)  |

### 🧠 Analogy:

> **OpenAPI** is the **blueprint** (API contract).  
> **Swagger** is the **toolbox** to visualize and use the blueprint.

---

## 🛠 Enabling Swagger in .NET Core Web API (via Swashbuckle)

To convert your OpenAPI setup into a full Swagger UI experience using **Swashbuckle**, follow these steps:

### 🔹 Step 1: Install Swashbuckle

In your Web API project directory:

```bash
dotnet add package Swashbuckle.AspNetCore
```

---

### 🔹 Step 2: Update `Program.cs`

Replace or update your `Program.cs` file like this:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ✅ Register OpenAPI services + Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Serves Swagger UI at /swagger
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

---

### 🔹 Step 3: Run the App

Use the correct launch profile that includes HTTPS and Swagger:

```bash
dotnet run --launch-profile "MyWebApi"
```

Then visit in browser:

```
https://localhost:7240/swagger
```

✅ You'll see a beautiful Swagger UI with your `/weatherforecast` endpoint!

---

### 📌 Summary

- **OpenAPI** describes the API structure (routes, params, responses)
- **Swashbuckle + Swagger** generate a visual and interactive UI from that spec
- `.NET 6+` minimal APIs work seamlessly with Swagger


