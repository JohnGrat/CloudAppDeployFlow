using Microsoft.Azure.Cosmos;
using Shared.Models;

namespace Business.Repositories.Default
{
    public class WeatherRepository : IRepository<WeatherForecast> // Assuming string is the type of your entity's ID
    {
        private readonly Container _container;

        public WeatherRepository(string cosmosConnectionString, string databaseName, string containerName)
        {
            //var client = new CosmosClient(cosmosConnectionString);
            //var database = client.GetDatabase(databaseName);
            //_container = database.GetContainer(containerName);
        }

        public async Task<WeatherForecast> AddAsync(WeatherForecast entity)
        {
            var response = await _container.CreateItemAsync(entity);
            return response.Resource;
        }

        public Task<WeatherForecast> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WeatherForecast>> GetAllAsync(string? sort, string? filter, int? page, int? pageSize)
        {
            var query = _container.GetItemQueryIterator<WeatherForecast>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<WeatherForecast>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<WeatherForecast> GetByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<WeatherForecast>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<WeatherForecast> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<WeatherForecast> UpdateAsync(WeatherForecast entity)
        {
            var response = await _container.UpsertItemAsync(entity);
            return response.Resource;
        }
    }
}
