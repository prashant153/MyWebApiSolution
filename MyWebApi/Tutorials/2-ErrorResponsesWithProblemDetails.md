# Problem Details for HTTP APIs in .NET Core

## Table of Contents
1. [Introduction](#introduction)
2. [Why Use Problem Details?](#why-use-problem-details)
3. [Basic Setup](#basic-setup)
4. [Custom Responses](#custom-responses)
5. [Validation Errors](#validation-errors)
6. [Global Exception Handling](#global-exception-handling)
7. [Advanced Customizations](#advanced-customizations)

## Introduction
Problem Details is a standardized format for HTTP API error responses defined in [RFC 7807](https://tools.ietf.org/html/rfc7807).

<pre><code>{
  "type": "https://example.com/errors/invalid-request",
  "title": "Invalid Request",
  "status": 400,
  "detail": "The product ID must be positive",
  "instance": "/api/products/0"
}</code></pre>

## Why Use Problem Details?

### 1. Standard Compliance
<pre><code>{
  "type": "about:blank",
  "title": "Not Found",
  "status": 404,
  "detail": "Product not found"
}</code></pre>

### 2. Consistent Structure
- Standard fields: type, title, status, detail, instance
- Custom extensions allowed

### 3. Better Debugging
<pre><code>{
  "type": "https://example.com/errors/database",
  "title": "Database Error",
  "status": 500,
  "detail": "Connection timeout",
  "timestamp": "2023-11-15T12:00:00Z"
}</code></pre>

## Basic Setup

1. Install the package:
<pre><code>dotnet add package Hellang.Middleware.ProblemDetails</code></pre>

2. Configure in Program.cs:
<pre><code>builder.Services.AddProblemDetails(options => 
{
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();
});

app.UseProblemDetails();</code></pre>

## Custom Responses

Controller example:
<pre><code>[HttpGet("{id}")]
public IActionResult GetProduct(int id)
{
    if (id <= 0)
    {
        return Problem(
            title: "Invalid ID",
            detail: "ID must be positive",
            statusCode: 400,
            type: "https://api/errors/invalid-id");
    }
    // ...
}</code></pre>

## Validation Errors

Automatic ModelState handling:
<pre><code>[HttpPost]
public IActionResult Create([FromBody] ProductDto product)
{
    if (!ModelState.IsValid)
    {
        return ValidationProblem(ModelState);
    }
    // ...
}</code></pre>

## Global Exception Handling

Middleware example:
<pre><code>app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        
        var problem = new ProblemDetails
        {
            Title = "Server Error",
            Status = 500,
            Detail = exception?.Message
        };
        
        await context.Response.WriteAsJsonAsync(problem);
    });
});</code></pre>

## Advanced Customizations

### Custom Factory
<pre><code>public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(
        HttpContext context,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };
    }
}</code></pre>

### Minimal API Example
<pre><code>app.MapGet("/products/{id}", (int id) =>
{
    return id > 0 
        ? Results.Ok(new Product(id)) 
        : Results.Problem(
            title: "Invalid ID",
            statusCode: 400);
});</code></pre>

## References
- [RFC 7807 Specification](https://tools.ietf.org/html/rfc7807)
- [Microsoft Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors)