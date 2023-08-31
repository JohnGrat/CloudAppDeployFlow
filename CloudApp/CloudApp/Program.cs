using Business.Repositories.Default;
using CloudApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);


// Register the WeatherRepository using the desired configuration
string cosmosConnectionString = builder.Configuration.GetConnectionString("Database");
string databaseName = "ToDoList";
string containerName = "Items";
builder.Services.AddSingleton<WeatherRepository>(sp =>
    new WeatherRepository(cosmosConnectionString, databaseName, containerName));


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

app.Run();
