namespace UnderstandingInMemoryCache.Service
{
    public class WeatherForecastService : IWeatherForecastService
    {
        public List<WeatherForecast> GetWeatherForecasts()
        {
            return new List<WeatherForecast>() 
            {
               new WeatherForecast()
                {
                    Date = DateTime.Now,
                    Summary = "Sunny",
                    TemperatureC = 32,
                },
               new WeatherForecast() 
               {
                   Date = DateTime.Now,
                    Summary = "Cold",
                    TemperatureC = -10,
               }
            };
        }
    }
}
