using Microsoft.Azure.Cosmos;
using Shared.Models;
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
            await AddItemsToContainerAsync();
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
                await this.database.CreateContainerIfNotExistsAsync(this.container.Id, "/Date");
                Console.WriteLine($"Created Container: {this.container.Id}");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine($"Container {this.container.Id} already exists");
            }
        }

        public async Task AddItemsToContainerAsync()
        {
            var forecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Date = new DateOnly(2023, 9, 1), TemperatureC = 25, Summary = "Sunny" },
                new WeatherForecast { Date = new DateOnly(2023, 9, 2), TemperatureC = 22, Summary = "Cloudy" },
                // Add more forecast items as needed
            };

            foreach (var forecast in forecasts)
            {
                // Convert DateOnly to string for the partition key
                string partitionKey = forecast.Date.ToString();

                await this.container.CreateItemAsync(forecast, new PartitionKey(partitionKey));
                Console.WriteLine($"Added forecast for {forecast.Date}");
            }
        }

    }
}
