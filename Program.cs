using FamilySyncApi.Repositories;
using FamilySyncApi.Settings;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

// Load .env variables early
Env.Load();
IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.AddConsole();

// Add controllers
builder.Services.AddControllers();

// Configure strongly typed AzureBlobStorageSettings
builder.Services.Configure<AzureBlobStorageSettings>(options =>
{
    builder.Configuration.GetSection("AzureBlobStorage").Bind(options);

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

    var connectionStringFromEnv = builder.Configuration.GetConnectionString("AzureBlobStorage");
    if (!string.IsNullOrWhiteSpace(connectionStringFromEnv))
    {
        options.ConnectionString = connectionStringFromEnv;
    }
    else
    {
        logger.LogWarning("AzureBlobStorage is missing or empty!");
    }

    var containerNameFromEnv = builder.Configuration["AzureBlobStorageContainerName"];
    if (!string.IsNullOrWhiteSpace(containerNameFromEnv))
    {
        options.ContainerName = containerNameFromEnv;
    }
    else
    {
        logger.LogWarning("AzureBlobStorageContainerName is missing or empty!");
    }
});

// Configure authentication and authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var expectedAudience = $"api://{clientId}";

    options.MapInboundClaims = false;
    options.TokenValidationParameters.ValidAudiences = new[]
    {
        expectedAudience, 
        clientId          
    };
    options.TokenValidationParameters.RoleClaimType = "roles";
});


// Authorization
builder.Services.AddAuthorization();

// Register repository
builder.Services.AddScoped(typeof(IBlobStorageRepository<>), typeof(BlobStorageRepository<>));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:9000", // Local dev
            "https://yellow-sea-0e75e7e03.6.azurestaticapps.net" // Deployed
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowFrontend");

// Swagger UI
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection and authentication
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Authenticated user: {Name}", context.User.Identity.Name);

        foreach (var claim in context.User.Claims)
        {
            logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
    }
    else
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Unauthenticated request to: {Path}", context.Request.Path);
    }

    await next();
});


// Map controllers
app.MapControllers();

// Start app
app.Run();
