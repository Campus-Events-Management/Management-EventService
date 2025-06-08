using EventManagement.EventService.Data;
using EventManagement.EventService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add SQLite database
builder.Services.AddDbContext<EventDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Data Source=EventDatabase.db");
});

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repository
builder.Services.AddScoped<IEventRepository, EventRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management API", Version = "v1" });
});

// Add explicit Kestrel configuration to bind to all addresses
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5075);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API v1"));

    // Create and migrate the database in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EventDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

// Use routing and endpoints
app.UseRouting();
app.UseHttpsRedirection();

// Ensure the event-images directory exists
var eventImagesPath = Path.Combine(app.Environment.ContentRootPath, "event-images");
if (!Directory.Exists(eventImagesPath))
{
    Directory.CreateDirectory(eventImagesPath);
}

// Serve static files for images
app.UseStaticFiles(); // Serve files from wwwroot

// Also serve files from the event-images directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(eventImagesPath),
    RequestPath = "/event-images"
});

// Use CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => "Service is healthy!");

// Print debugging information
Console.WriteLine("=============================================");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"ContentRoot Path: {builder.Environment.ContentRootPath}");
Console.WriteLine("Service is listening on: http://localhost:5075");
Console.WriteLine("API endpoint: http://localhost:5075/api/events");
Console.WriteLine("Health endpoint: http://localhost:5075/health");
Console.WriteLine("Swagger UI: http://localhost:5075/swagger");
Console.WriteLine("Images directory: " + eventImagesPath);
Console.WriteLine("=============================================");

// Start the application
app.Run(); 