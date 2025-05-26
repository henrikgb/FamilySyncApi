using FamilySyncApi.Repositories;
using FamilySyncApi.Settings;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

// Load .env variables early
Env.Load();

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

// Manually configure valid audiences to match the token
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var audience = builder.Configuration["AzureAd:Audience"];

    options.TokenValidationParameters.ValidAudiences = new[]
    {
        audience,
        clientId,
        $"api://{clientId}"
    };
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

// Map controllers
app.MapControllers();

// Start app
app.Run();
