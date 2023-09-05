using Business.Repositories.Default;
using CloudApp.Data;
using CloudApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);


// Register the WeatherRepository using the desired configuration

string cosmosConnectionString = builder.Configuration.GetConnectionString("Database");
string databaseName = "WeatherDb";
string containerName = "Forecasts";
builder.Services.AddSingleton<WeatherRepository>(sp =>
    new WeatherRepository(cosmosConnectionString, databaseName, containerName));

builder.Services.AddSingleton(provider => cosmosConnectionString);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

SeedCosmosDatabase(cosmosConnectionString).Wait();

app.Run();


static async Task SeedCosmosDatabase(string connectionString)
{
    try
    {
        // Create an instance of the Seed class
        var seed = new Seed(connectionString);

        // Initialize the Cosmos DB database and container
        await seed.InitializeAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding Cosmos DB: {ex.Message}");
    }
}