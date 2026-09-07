using EmployeeManagementApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "Employee Management API",
//        Version = "v1",
//        Description = "A simple CRUD API for managing employees."
//    });
//});

// Database configuration
// By default this uses an in-memory database so the project runs immediately
// with no setup. To use a real SQL Server database instead:
//   1. Update the connection string in appsettings.json
//   2. Replace UseInMemoryDatabase(...) below with:
//        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
//   3. Run: dotnet ef migrations add InitialCreate
//   4. Run: dotnet ef database update
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeManagementDb"));

// CORS - allow any origin for development purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
//    options.RoutePrefix = string.Empty; // Serve Swagger UI at the app root
//});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
