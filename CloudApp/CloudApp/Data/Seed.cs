using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Shared.Models;
using System.Drawing;
using System.Net;

namespace CloudApp.Data
{
    public class Seed
    {
        private readonly CosmosClient cosmosClient;
        private readonly Database database;
        private readonly Container container;

        public Seed(string connectionString)
        {
            this.cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = this.cosmosClient.GetDatabase("WeatherDb");
            this.container = this.database.GetContainer("Forecasts");
        }

        public async Task InitializeAsync()
        {
            await CreateDatabaseAsync();
            await CreateContainerAsync();
            await AddWeatherForecastToContainerAsync();
        }

        public async Task CreateDatabaseAsync()
        {
            try
            {
                await this.cosmosClient.CreateDatabaseIfNotExistsAsync(database.Id);
                Console.WriteLine($"Created Database: {this.database.Id}");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine($"Database {this.database.Id} already exists");
            }
        }

        public async Task CreateContainerAsync()
        {
            try
            {
                await this.database.CreateContainerIfNotExistsAsync(this.container.Id, "/partitionKey");
                Console.WriteLine($"Created Container: {this.container.Id}");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine($"Container {this.container.Id} already exists");
            }
        }


        private async Task AddWeatherForecastToContainerAsync()
        {
            var forecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Id = "1", PartitionKey = "Forecast", Date = new DateOnly(2023, 9, 1), TemperatureC = 25, Summary = "Sunny" },
                new WeatherForecast { Id = "2", PartitionKey = "Forecast", Date = new DateOnly(2023, 9, 2), TemperatureC = 22, Summary = "Cloudy" }
                // Add more forecast items as needed
            };

            foreach (var item in forecasts)
            {
                try
                {
                    // Read the item to see if it exists.
                    ItemResponse<WeatherForecast> forecastResponse = await this.container.ReadItemAsync<WeatherForecast>(item.Id, new PartitionKey(item.PartitionKey));
                    Console.WriteLine("Item in database with id: {0} already exists\n", forecastResponse.Resource.Id);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // Create an item in the container representing the forecast. Note we provide the value of the partition key for this item.
                    ItemResponse<WeatherForecast> forecastResponse = await this.container.CreateItemAsync<WeatherForecast>(item, new PartitionKey(item.PartitionKey));

                    // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse.
                    // We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", forecastResponse.Resource.Id, forecastResponse.RequestCharge);
                }
            }
        }

    }
}
