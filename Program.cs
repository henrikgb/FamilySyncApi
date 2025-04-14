using FamilySyncApi.Repositories;
using FamilySyncApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Register strongly typed AzureBlobStorageSettings from configuration
builder.Services.Configure<AzureBlobStorageSettings>(options =>
{
    // Bind from appsettings.development.json or environment variables
    builder.Configuration.GetSection("AzureBlobStorage").Bind(options);

    // Manually override ContainerName from environment variable (App settings)
    var containerNameFromEnv = builder.Configuration["AzureBlobStorageContainerName"];
    if (!string.IsNullOrWhiteSpace(containerNameFromEnv))
    {
        options.ContainerName = containerNameFromEnv;
    }
});

// Register repository
builder.Services.AddScoped(typeof(IBlobStorageRepository<>), typeof(BlobStorageRepository<>));

// Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
