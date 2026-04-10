using Asp.Versioning;
using ClinicManagementSystem.API.Middlewares;
using ClinicManagementSystem.API.RateLimiting;
using ClinicManagementSystem.Application;
using ClinicManagementSystem.Domain.Entities.Identity;
using ClinicManagementSystem.Infrastructure;
using ClinicManagementSystem.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting Clinic Management System API");
    var builder = WebApplication.CreateBuilder(args);
    // Add Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));
    #region Services

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

    #endregion
    var app = builder.Build();
    #region Apply Migrations
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    #endregion
    // Add request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });
    #region Middleware
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
    #region Seed Admin User
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        const string adminUserName = "admin";
        const string adminPassword = "Admin@123456";

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // 🔑 Check if ANY user is in Admin role
        var admins = await userManager.GetUsersInRoleAsync(adminRole);

        if (!admins.Any())
        {
            var user = new ApplicationUser
            {
                UserName = adminUserName,
                Email = "admin@clinic.com"
            };

            var result = await userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
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
    #region Use My Middleware
    app.UseMiddleware<GlobalExceptionMiddleware>();
    #endregion
    app.UseHttpsRedirection();
    // Add middleware — must be before UseAuthentication
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    #endregion
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}