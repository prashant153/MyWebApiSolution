Understanding Formatters and Content Negotiation in .NET Core
=============================================================

Formatters and content negotiation are key concepts in ASP.NET Core that determine how your API serializes and deserializes data, and how it responds with different content types based on client requests.

Content Negotiation Basics
--------------------------

Content negotiation is the process where the client and server agree on the best format (JSON, XML, etc.) for the response. This is primarily driven by the Accept header in the HTTP request.

### How it works:

1.  Client sends a request with Accept header (e.g., application/json)
    
2.  Server examines available formatters
    
3.  Server selects the first formatter that can produce the requested format
    
4.  Server serializes the response using that formatter
    

Built-in Formatters
-------------------

.NET Core includes these default formatters:

*   SystemTextJsonOutputFormatter - Handles JSON using System.Text.Json
    
*   SystemTextJsonInputFormatter - Handles JSON input
    
*   XmlSerializerOutputFormatter - Handles XML output
    
*   XmlSerializerInputFormatter - Handles XML input
    

Configuring Formatters
----------------------

Here's how to configure formatters in your Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Add XML support
    options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
    
    // Respect browser Accept header
    options.RespectBrowserAcceptHeader = true;
    
    // Return 406 Not Acceptable if no formatter can handle the request
    options.ReturnHttpNotAcceptable = true;
});

var app = builder.Build();
```

Example API Controller
----------------------

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 19.99m }
    };

    // GET: api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        return _products;
    }

    // GET api/products/1
    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    // POST api/products
    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```
Testing Content Negotiation
---------------------------

You can test different response formats using these approaches:

### 1\. Using Accept header

```http
GET /api/products/1
Accept: application/xml
```

### 2\. Using format query parameter (if enabled)

```http
GET /api/products/1?format=xml
```
### 3\. Using file extension

```http
GET /api/products/1.xml
```
Custom Formatters
-----------------

You can create custom formatters for specialized formats. Here's an example CSV formatted:

```csharp
public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add("text/csv");
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type)
    {
        if (typeof(IEnumerable<Product>).IsAssignableFrom(type))
        {
            return true;
        }
        return false;
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;
        var buffer = new StringBuilder();

        if (context.Object is IEnumerable<Product> products)
        {
            buffer.AppendLine("Id,Name,Price");
            foreach (var product in products)
            {
                buffer.AppendLine($"{product.Id},{product.Name},{product.Price}");
            }
        }

        await response.WriteAsync(buffer.ToString(), selectedEncoding);
    }
}
```

Register the custom formatter in Program.cs:

```csharp
[HttpGet]
[Produces("application/json")] // Always returns JSON
public ActionResult<IEnumerable<Product>> Get()
{
    return _products;
}

// Or for multiple formats
[HttpGet("{id}")]
[Produces("application/json", "application/xml")]
public ActionResult<Product> Get(int id)
{
    var product = _products.FirstOrDefault(p => p.Id == id);
    if (product == null)
    {
        return NotFound();
    }
    return product;
}
```

Key Points to Remember
----------------------

1.  Content negotiation is primarily driven by the Accept header
    
2.  The order of formatters matters - first matching formatter is used
    
3.  You can configure default behavior for when no format is specified
    
4.  Custom formatters allow you to support any format you need
    
5.  You can override negotiation with attributes like \[Produces\]
