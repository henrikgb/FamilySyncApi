using FamilySyncApi.Repositories;
using FamilySyncApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Register strongly typed AzureBlobStorageSettings from configuration
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

// Register repository
builder.Services.AddScoped(typeof(IBlobStorageRepository<>), typeof(BlobStorageRepository<>));

// Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow CORS for local frontend + potential future prod frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:9000",                         // Local dev
                "https://your-frontend-url.azurestaticapps.net"  // Deployed static web app
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowLocalhostFrontend");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
