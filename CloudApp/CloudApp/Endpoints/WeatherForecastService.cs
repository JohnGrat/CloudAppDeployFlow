using Business.Repositories.Default;
using Shared.Models;

namespace CloudApp.Endpoints
{
    public class WeatherForecastService
    {
        private WeatherRepository _weatherRepository;

        public WeatherForecastService(WeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecastAsync()
        {
            var forecasts = await _weatherRepository.GetAllAsync(null, null, null, null);

            //// Assuming you have a method to filter forecasts based on the startDate
            //var filteredForecasts = forecasts.Where(forecast => forecast.Date >= startDate);

            //return filteredForecasts;
            return forecasts;
        }

    }
}
