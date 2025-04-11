using FamilySyncApi.Repositories;
using FamilySyncApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register strongly typed AzureBlobStorageSettings
builder.Services.Configure<AzureBlobStorageSettings>(options =>
{
    builder.Configuration.GetSection("AzureBlobStorage").Bind(options);
    options.ContainerName = builder.Configuration["AzureBlobStorageContainerName"] ?? string.Empty;
});

// Register repository
builder.Services.AddScoped(typeof(IBlobStorageRepository<>), typeof(BlobStorageRepository<>));

// Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
