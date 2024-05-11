using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Text.Json;
using static SampleOpenTelementry.TelementryConstants;

namespace SampleOpenTelementry
{
    [Route("api/SampleOpenTelementry/[controller]")]
    [ApiController]
    public class WeatherForeCastController : Controller
    {
        private readonly Tracer _tracer;
        private readonly ILogger<WeatherForeCastController> _logger;
        private readonly IDatabase _redis;
        static ActivitySource activitySource;
        public WeatherForeCastController(Tracer tracer, ILogger<WeatherForeCastController> logger, IConnectionMultiplexer muxer)
        {
            _tracer = tracer;
            _logger = logger;
            _redis = muxer.GetDatabase();
            ActivitySource.AddActivityListener(new ActivityListener()
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStarted = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Start"),
                ActivityStopped = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Stop")
            });
            activitySource =new ActivitySource("SampleOpenTelementry.ManualInstrumentations.*","1.0.0");
        }

        [HttpGet("weather")]
        public async Task<IActionResult> Index()
        {
            var baggageCopy = Baggage.Current;
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
                span?.End();

                return Ok(forecast);
            }

            catch (Exception ex)
            {
                _logger.LogError("Test error");
                throw;
            }
        }

        [HttpGet("weather2")]
        public async Task<IActionResult> Index2()
        {
            var activity = activitySource.StartActivity("ActivityName");

            activity?.SetTag("http.method", "GET");
            if (activity != null && activity.IsAllDataRequested == true)
            {
                activity.SetTag("http.url", "http://www.mywebsite.com");
            }
            return Ok(activity);
        }

    }
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }
}
