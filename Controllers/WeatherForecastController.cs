using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using UnderstandingInMemoryCache.Service;

namespace UnderstandingInMemoryCache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeZoneInfo _cetTimeZone;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IWeatherForecastService weatherForecastService,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
            _memoryCache = memoryCache;
            _cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        }

        /// <summary>
        /// This method will return weatherforcast list and it will set the expire time every day around 4 am cet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            const string cacheKey = "WeatherForecasts";
            if (!_memoryCache.TryGetValue(cacheKey, out List<WeatherForecast> weatherForecasts))
            {
                weatherForecasts = _weatherForecastService.GetWeatherForecasts().ToList();
                var expirationTime = GetNext4AMCET();

                // AbsoluteExpiration it will expire the cache value based on expirationTime, 
                // Here it will expire every day 4 am cet time
                _memoryCache.Set(cacheKey, weatherForecasts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = expirationTime,
                    
                });
                _logger.LogInformation($"Cache set to expire at: {expirationTime}");
            }
            return weatherForecasts;
        }

        [HttpGet("Getweatherforcastdata")]
        public IEnumerable<WeatherForecast> GetForecasts()
        {
            const string cacheKey = "WeatherForecasts1";
            if (!_memoryCache.TryGetValue(cacheKey, out List<WeatherForecast> weatherForecasts))
            {
                weatherForecasts = _weatherForecastService.GetWeatherForecasts().ToList();
                _memoryCache.Set("WeatherForecasts1", weatherForecasts, new MemoryCacheEntryOptions
                {
                    // if cache not used within 10 minute it will expire 
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    // cache will expire in 5 minutes, even if cache accessed with 10 minutes, it will expire in 5 minutes
                    // if you don't use AbsoluteExpiration, cache will never expire until SlidingExpiration time passed
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                    // PostEvictionCallbacks will get called after cache expired when next request comes only
                    PostEvictionCallbacks =
                    {
                        new PostEvictionCallbackRegistration
                        {
                            EvictionCallback = CacheClearedMethodCalled
                        }
                    }
                }); ;
            }
            return weatherForecasts;
        }

        private void CacheClearedMethodCalled(object key, object value, EvictionReason reason, object state) => Console.WriteLine($"Cache entry '{key}' was evicted due to '{reason}'. datetime {DateTime.Now}");

        private DateTimeOffset GetNext4AMCET()
        {
            // converting current time to cet time 
            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _cetTimeZone);

            // finding next 4 am date and time
            var next4AM = new DateTimeOffset(now.Year, now.Month, now.Day, 4, 0, 0, _cetTimeZone.BaseUtcOffset);

            if (now >= next4AM)
            {
                next4AM = next4AM.AddDays(1);
            }

            return next4AM;
        }

    }
}
