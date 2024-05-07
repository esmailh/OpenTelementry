using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Configuration;
using static SampleOpenTelementry.TelementryConstants;

namespace SampleOpenTelementry
{
    public static class Registration
    {
        public static void AddTelementryRequirement(this IServiceCollection services, IConfiguration configuration)
        {
            var resource = ResourceBuilder.CreateDefault().AddService(serviceName:AppSource, serviceVersion: AppSourceVersion);
            Console.WriteLine(configuration.GetSection("tracing:jaeger:url").Value);
            services.AddOpenTelemetry().WithMetrics(metric =>
            {
                metric.SetResourceBuilder(resource)
                      .AddConsoleExporter()
                      .AddPrometheusExporter(ex =>
                      {
                          ex.ScrapeEndpointPath = "/metrics";
                      });
            }).WithTracing(trace =>
            {
                trace.AddSource(AppSource)
                     .SetResourceBuilder(resource)
                     .AddJaegerExporter(jaegerOptions =>
                     {
                         jaegerOptions.AgentHost = configuration.GetSection("tracing:jaeger:url").Value;
                         jaegerOptions.AgentPort = Convert.ToInt32(configuration.GetSection("tracing:jaeger:port").Value);
                     })
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation()
                     .SetSampler(new AlwaysOnSampler());
            });
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfiguration config = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json")
                                    .AddJsonFile($"appsettings.{env}.json")
                                    .AddEnvironmentVariables()
                                    .Build();
        }
    }
}
