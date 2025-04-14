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

    var connectionStringFromEnv = builder.Configuration.GetConnectionString("AzureBlobStorage__ConnectionString");
    if (!string.IsNullOrWhiteSpace(connectionStringFromEnv))
    {
        logger.LogInformation("Using ConnectionString from Azure Connection Strings section.");
        options.ConnectionString = connectionStringFromEnv;
    }
    else
    {
        logger.LogWarning("AzureBlobStorage__ConnectionString is missing or empty!");
    }

    var containerNameFromEnv = builder.Configuration["AzureBlobStorageContainerName"];
    if (!string.IsNullOrWhiteSpace(containerNameFromEnv))
    {
        logger.LogInformation("Using ContainerName from environment variable.");
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
