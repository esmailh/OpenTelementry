using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace SampleOpenTelementry
{
    [Route("api/SampleOpenTelementry/[controller]")]
    [ApiController]
    public class WeatherForeCastController : Controller
    {
        private readonly Tracer _tracer;
        private readonly ILogger<WeatherForeCastController> _logger;
        private readonly IDatabase _redis;
        public WeatherForeCastController(Tracer tracer, ILogger<WeatherForeCastController> logger, IConnectionMultiplexer muxer)
        {
            _tracer = tracer;
            _logger = logger;
            _redis = muxer.GetDatabase();
        }

        [HttpGet("weather")]
        public async Task<IActionResult> Index()
        {
            using var span = _tracer.StartActiveSpan("RedisTracer");

            try
            {
                var rediskey = "weather";
                span?.SetAttribute("rediskey", rediskey);

                var summaries = new[]
                                    {
                 "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
                var json = JsonSerializer.Serialize(forecast);
                await _redis.StringSetAsync(rediskey, json, TimeSpan.FromSeconds(10));
                span?.SetAttribute("redisdb",_redis.Database);

                span?.AddEvent(new($"set redis value {json}"));
                span?.SetStatus(Status.Unset);

                return Ok(forecast);
            }

            catch (Exception ex)
            {
                _logger.LogError("Test error");
                throw;
            }
        }
    }
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }
}
