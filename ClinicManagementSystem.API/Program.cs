using ClinicManagementSystem.API.Middlewares;
using ClinicManagementSystem.Application;
using ClinicManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Added: required for Swagger to discover endpoints
builder.Services.AddSwaggerGen();           // Added: registers Swagger generator
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
var app = builder.Build();

#region Add Roles Seed
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Doctor", "Patient", "Receptionist" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
#endregion

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
