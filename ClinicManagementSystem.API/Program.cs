using ClinicManagementSystem.API.Middlewares;
using ClinicManagementSystem.Application;
using ClinicManagementSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Added: required for Swagger to discover endpoints
builder.Services.AddSwaggerGen();           // Added: registers Swagger generator
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Removed: app.MapOpenApi()
    app.UseSwagger();    // Added: enables Swagger JSON endpoint
    app.UseSwaggerUI();  // Added: enables Swagger UI at /swagger
}
#region Use Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
#endregion
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
