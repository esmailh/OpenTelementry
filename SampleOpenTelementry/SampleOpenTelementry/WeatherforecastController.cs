using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace SampleOpenTelementry
{
    [Route("api/SampleOpenTelementry/[controller]")]
    [ApiController]
    public class WeatherForeCastController : Controller
    {
        private readonly Tracer _tracer;

        public WeatherForeCastController(Tracer tracer)
        {
            _tracer = tracer;
        }

        [HttpGet("weather")]
        public async Task<IActionResult> Index()
        {
            
            using var span = _tracer.StartActiveSpan("HttpTracer");
      
            span?.SetAttribute("httpTracer,AddTag", HttpContext.TraceIdentifier);
           
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
            span?.SetStatus(Status.Error);
            span?.AddEvent(new ($"Received Http request from {Request.Headers.UserAgent}"));

            return Ok(forecast);
        }
    }
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }
}
