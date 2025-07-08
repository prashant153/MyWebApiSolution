**File Handling in .NET Core & ASP.NET Core Web API**
========================================================================

**1\. Introduction**
--------------------

This tutorial covers file operations in .NET Core, including:

*   Reading/writing files
    
*   File upload/download in APIs
    
*   Using FileExtensionContentTypeProvider
    
*   Best practices
    

**2\. Basic File Operations**
-----------------------------

### **2.1 Reading a File**

```csharp
string content = File.ReadAllText("example.txt");
```

### **2.2 Writing to a File**


```csharp
File.WriteAllText("example.txt", "Hello, .NET Core!");
```

### **2.3 Checking File Existence**


```csharp
bool exists = File.Exists("example.txt");
```

**3\. File Operations in Web API**
----------------------------------

### **3.1 File Upload**


```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded");
    
    var filePath = Path.Combine("Uploads", file.FileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    return Ok(new { filePath });
}
```

### **3.2 File Download (Basic)**

```csharp
[HttpGet("download")]
public IActionResult Download(string fileName)
{
    var filePath = Path.Combine("Uploads", fileName);
    return PhysicalFile(filePath, "application/octet-stream", fileName);
}
```

**4\. Using FileExtensionContentTypeProvider**
----------------------------------------------

### **4.1 What is it?**

*   Maps file extensions to MIME types (e.g., .pdf → application/pdf)
    
*   Ensures correct Content-Type headers
    

### **4.2 Registering in Program.cs**
```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register as service for DI
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(_ =>
{
    var provider = new FileExtensionContentTypeProvider();
    provider.Mappings[".myapp"] = "application/x-myapp-custom";
    return provider;
});

var app = builder.Build();

// 2. Configure static files
var staticFilesProvider = new FileExtensionContentTypeProvider();
staticFilesProvider.Mappings[".data"] = "application/data";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = staticFilesProvider
});

// 3. Use in minimal API
app.MapGet("/api/files/{fileName}", (string fileName, 
    FileExtensionContentTypeProvider provider) => // Injected
{
    var filePath = Path.Combine("Uploads", fileName);
    
    if (!File.Exists(filePath))
        return Results.NotFound();
    
    if (!provider.TryGetContentType(fileName, out var contentType))
        contentType = "application/octet-stream";
    
    return Results.File(filePath, contentType, fileName);
});

app.Run();
```
### **4.3 Using in a Controller**
```csharp
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _provider;

    // Inject via constructor
    public FilesController(FileExtensionContentTypeProvider provider)
    {
        _provider = provider;
    }

    [HttpGet("download")]
    public IActionResult Download(string fileName)
    {
        var filePath = Path.Combine("Uploads", fileName);
        
        if (!_provider.TryGetContentType(fileName, out string contentType))
            contentType = "application/octet-stream"; // Default
        
        return PhysicalFile(filePath, contentType, fileName);
    }
}
```
**5\. Advanced File Return Methods**
------------------------------------

### **5.1 Return as Stream**

```csharp
var stream = new FileStream(filePath, FileMode.Open);
return File(stream, contentType);
```

### **5.2 Return as Byte Array**
```csharp
byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
return File(fileBytes, contentType, fileName);
```
### **5.3 Dynamic File Generation (e.g., CSV)**
```csharp
[HttpGet("export-csv")]
public IActionResult ExportCsv()
{
    var csv = new StringBuilder();
    csv.AppendLine("Name,Email");
    csv.AppendLine("John,john@example.com");
    
    return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "users.csv");
}
```

**6\. Best Practices**
----------------------

✅ **Always:**

*   Use using for file streams
    
*   Validate file paths to prevent directory traversal
    
*   Set Content-Disposition for downloads
    
*   Use async methods (ReadAllTextAsync, CopyToAsync)
    

❌ **Avoid:**

*   Hardcoding MIME types
    
*   Trusting user-provided file paths directly
    
*   Loading large files into memory (use streaming)
    

**7\. Summary**
---------------

**TaskMethod**Read fileFile.ReadAllText()Write fileFile.WriteAllText()UploadIFormFile.CopyToAsync()DownloadPhysicalFile()MIME type mappingFileExtensionContentTypeProvider

### **Final Notes**

*   Use FileExtensionContentTypeProvider for consistent MIME type handling.
    
*   Prefer dependency injection over manual instantiation.
    
*   Test file operations with different file types (PDF, images, etc.).
