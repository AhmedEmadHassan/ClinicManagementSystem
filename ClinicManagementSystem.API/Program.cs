using Asp.Versioning;
using ClinicManagementSystem.API;
using ClinicManagementSystem.API.Middlewares;
using ClinicManagementSystem.Application;
using ClinicManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Add Controllers with Suppressed Model State Validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
//builder.Services.AddControllers();
#endregion
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Added: required for Swagger to discover endpoints
#region Add Swagger Gen
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic Management System", Version = "v1" });
    //options.SwaggerDoc("v2", new OpenApiInfo { Title = "Clinic Management System", Version = "v2" }); // Add for each new version

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
#endregion
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
// Add Rate Limiting registration
builder.Services.AddRateLimiting(builder.Configuration);
#region Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic Management System v1");
        //options.SwaggerEndpoint("/swagger/v2/swagger.json", "Clinic Management System v2"); // Add for each new version

    });
}
#region Use Middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
#endregion
app.UseHttpsRedirection();
// Add middleware — must be before UseAuthentication
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
